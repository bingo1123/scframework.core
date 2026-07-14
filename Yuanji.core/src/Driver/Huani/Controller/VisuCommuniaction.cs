using System.Net.Sockets;
using System.Net;
using System.Text;
using Yuanji.core.src.Driver.Huani.Entity;
using YS.Yuanji.Commom;
using System.Runtime.CompilerServices;
using YS.Yuanji.Log;
using Yuanji.core.src.Basic;
using Yuanji.core.srcDriver.Huani.Controller;
using YS.Yuanji.Drive;

namespace  Yuanji.core.src.Driver.Huani.Controller
{
    public class VisuCommunication : ParentChanel
    {
        private TcpClient client;

        private NetworkStream stream;

        private string ipAddress;

        private int port;

        private CancellationTokenSource listenTaskTokenSource;

        private Task listenTask;

        private bool _isConnected = false;

        private DLMachineTypeEnum _DLMachineType;

        private int readIntervalMilliseconds = 20;

        private Task statusTask;

        private int statusCheckIntervalMilliseconds = 5000;

        private CancellationTokenSource statusTaskTokenSource;

        private readonly AsyncLock _asyncLock = new AsyncLock();

        private bool isconnecting = false;

        private string errInfo = "";

        private readonly object _dataLock = new object();

        private byte[] _zj17ProductionData;

        public VisuCommunication()
        {
            statusTaskTokenSource = new CancellationTokenSource();
            listenTaskTokenSource = new CancellationTokenSource();
        }

        public override async Task<bool> ConnectAsync()
        {
            try
            {
                if (statusTask == null)
                {
                    statusTask = Task.Run(() => ListenForReconnection(statusTaskTokenSource.Token), statusTaskTokenSource.Token);
                }
                if (client != null && client.Connected)
                {
                    return true;
                }
                stream?.Dispose();
                client?.Dispose();
                client = new TcpClient();
                isconnecting = true;
                await client.ConnectAsync(IPAddress.Parse(ipAddress), port, statusTaskTokenSource.Token);
                isconnecting = false;
                stream = client.GetStream();
                stream.ReadTimeout = 10000;
                stream.WriteTimeout = 10000;
                _isConnected = true;
                return _isConnected;
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                isconnecting = false;
                LogController.Instance.Error("连接设备:" + ipAddress + "失败: " + ex2.Message);
                return false;
            }
        }

        private async Task ListenForReconnection(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(statusCheckIntervalMilliseconds, cancellationToken);
                if ((!_isConnected || !client.Connected) && !isconnecting)
                {
                    try
                    {
                        LogController.Instance.Log("设备:" + ipAddress + "尝试重新连接...");
                        stream?.Dispose();
                        client?.Dispose();
                        client = new TcpClient();
                        await client.ConnectAsync(IPAddress.Parse(ipAddress), port, statusTaskTokenSource.Token);
                        stream = client.GetStream();
                        stream.ReadTimeout = 5000;
                        stream.WriteTimeout = 5000;
                        _isConnected = true;
                        LogController.Instance.Log("设备:" + ipAddress + "成功重新连接！");
                    }
                    catch (Exception ex)
                    {
                        Exception ex2 = ex;
                        LogController.Instance.Error("设备:" + ipAddress + "重连失败: " + ex2.Message);
						LogController.Instance.PostLog(Config.ChannelName, "设备:" + ipAddress + "重连失败: " + ex2.Message);
					}
                }
            }
        }

        public override bool IsConnected()
        {
            if (stream == null || !client.Connected)
            {
                return false;
            }
            return _isConnected;
        }

        public string GetIpAddress()
        {
            return ipAddress;
        }

        public override async Task<bool> DisconnectAsync()
        {
            try
            {
                listenTaskTokenSource?.Cancel();
                listenTaskTokenSource?.Dispose();
                statusTaskTokenSource?.Cancel();
                statusTaskTokenSource?.Dispose();
                if (listenTaskTokenSource != null)
                {
                    listenTaskTokenSource = null;
                }
                if (statusTaskTokenSource != null)
                {
                    statusTaskTokenSource = null;
                }
                if (_isConnected)
                {
                    _isConnected = false;
                    stream?.Close();
                    client?.Close();
                    LogController.Instance.Log("设备:" + ipAddress + "已断开连接。");
                }
                return true;
            }
            catch (Exception)
            {
                LogController.Instance.Log("设备:" + ipAddress + "断开报错。");
                return false;
            }
        }

        public override async Task<byte[]> SendCommandAsync(byte[] data, [CallerMemberName] string? name = null)
        {
            if (!IsConnected())
            {
                LogController.Instance.Error("设备:" + ipAddress + "未连接到服务器");
                return null;
            }
            try
            {

                using (await _asyncLock.LockAsync())
                {

                    LogController.Instance.DebugfLog(name + ":发送=> " + BitConverter.ToString(data));
                    await stream.WriteAsync(data, 0, data.Length, statusTaskTokenSource.Token);
                    //await Task.Delay(50);
                    byte[] fullResponse = null;
                    if (_DLMachineType == DLMachineTypeEnum.Huani)
                    {
                        byte[] lengthBuffer = new byte[2];
                        int bytesRead = await ReadWithTimeoutAsync(lengthBuffer, 0, lengthBuffer.Length, 5000);
                        if (bytesRead < 2)
                        {
                            throw new Exception("设备:" + ipAddress + "未能读取到有效的应答长度信息");
                        }
                        ushort responseLength = BitConverter.ToUInt16(lengthBuffer, 0);
                        fullResponse = new byte[responseLength];
                        Array.Copy(lengthBuffer, fullResponse, 2);
                        int totalBytesRead = bytesRead;
                        int timeoutMilliseconds = 5000;
                        DateTime startTime = DateTime.Now;
                        while (totalBytesRead < responseLength)
                        {
                            if ((DateTime.Now - startTime).TotalMilliseconds > (double)timeoutMilliseconds)
                            {
                                throw new TimeoutException("接收应答包超时");
                            }
                            int remainingBytes = responseLength - totalBytesRead;
                            if (stream.DataAvailable)
                            {
                                int currentRead = await ReadWithTimeoutAsync(fullResponse, totalBytesRead, remainingBytes, 5000);
                                if (currentRead == 0)
                                {
                                    break;
                                }
                                totalBytesRead += currentRead;
                            }
                            else
                            {
                                await Task.Delay(readIntervalMilliseconds);
                            }
                        }
                        if (totalBytesRead < responseLength)
                        {
                            throw new Exception("应答包未完全接收");
                        }
                    }
                    else if (_DLMachineType == DLMachineTypeEnum.FxsZB48)
                    {
                        byte[] buffer = new byte[4096];
                        using MemoryStream ms = new MemoryStream();
                        int endIndex = FxsZB48Const.endSequence.Length - 1;
                        DateTime startTime2 = DateTime.Now;
                        int timeoutMilliseconds2 = 5000;
                        while (true)
                        {
                            if ((DateTime.Now - startTime2).TotalMilliseconds > (double)timeoutMilliseconds2)
                            {
                                LogController.Instance.Error("FxsZB48 接收应答包超时");
                                break;
                            }
                            int bytesRead2 = await ReadWithTimeoutAsync(buffer, 0, buffer.Length, 5000);
                            if (bytesRead2 == 0)
                            {
                                break;
                            }
                            await ms.WriteAsync(buffer, 0, bytesRead2, statusTaskTokenSource.Token);
                            bool flag = false;
                            for (int i = bytesRead2 - 1; i >= Math.Max(0, bytesRead2 - 5); i--)
                            {
                                if (buffer[i] == FxsZB48Const.endSequence[endIndex])
                                {
                                    endIndex--;
                                    if (endIndex < 0)
                                    {
                                        flag = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    endIndex = FxsZB48Const.endSequence.Length - 1;
                                }
                            }
                            if (flag)
                            {
                                break;
                            }
                            await Task.Delay(readIntervalMilliseconds);
                        }
                        byte[] completeData = ms.ToArray();
                        int dataLength = completeData.Length - FxsZB48Const.endSequence.Length;
                        fullResponse = new byte[(dataLength > 0) ? dataLength : 0];
                        if (dataLength > 0)
                        {
                            Array.Copy(completeData, fullResponse, dataLength);
                        }
                    }
                    if (fullResponse != null )
                    {
                        LogController.Instance.DebugfLog(name + ":收到=>" + BitConverter.ToString(fullResponse));
                    }
                    return fullResponse;
                }
            }
            catch (TimeoutException value)
            {
                LogController.Instance.Error($"设备:{ipAddress},caller:{name} VisuCommunication SendCommand TimeOut: {value}");
                return null;
            }
            catch (Exception value2)
            {
                LogController.Instance.Error($"设备:{ipAddress},caller:{name} visucommunication 命令发送失败: {value2},send:{BitConverter.ToString(data)}");
                return null;
            }
        }

        private async Task<int> ReadWithTimeoutAsync(byte[] buffer, int offset, int count, int timeoutMs)
        {
            using CancellationTokenSource cts = new CancellationTokenSource(timeoutMs);
            try
            {
                Task<int> readTask = stream.ReadAsync(buffer, offset, count, cts.Token);
                Task timeoutTask = Task.Delay(timeoutMs);
                if (await Task.WhenAny(readTask, timeoutTask) == readTask)
                {
                    return await readTask;
                }
                cts.Cancel();
                throw new TimeoutException($"读取操作超时 ({timeoutMs}ms)");
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"读取操作超时 ({timeoutMs}ms)");
            }
        }

        public void Listen()
        {
            listenTask?.Dispose();
            listenTask = Task.Factory.StartNew(() => ListenLoop(listenTaskTokenSource.Token), listenTaskTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private async Task ListenLoop(CancellationToken token)
        {
            byte[] buffer = new byte[4096];
            MemoryStream ms = new MemoryStream();
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (IsConnected())
                    {
                        if (await ReadDataAsync(ms, buffer, token) != 0 || ms.Length != 0)
                        {
                            await ProcessMessages(ms);
                            errInfo = "";
                            goto end_IL_007d;
                        }
                        await Task.Delay(2000, token);
                        continue;
                    }
                    await Task.Delay(statusCheckIntervalMilliseconds, token);
                end_IL_007d:;
                }
                catch (Exception ex)
                {
                    Exception ex2 = ex;
                    if (!ex2.Message.Equals(errInfo))
                    {
                        errInfo = ex2.Message;
                        LogController.Instance.Error($"设备:{ipAddress} ,visuCommunication ListenLoop error: {ex2}");
                    }
                }
                finally
                {
                    await Task.Delay(100, token);
                }
            }
            ms?.Dispose();
        }

        private async Task<int> ReadDataAsync(MemoryStream ms, byte[] buffer, CancellationToken token)
        {
            int bytesRead = await ReadWithTimeoutAsync(buffer, 0, buffer.Length, 5000);
            if (bytesRead > 0)
            {
                await ms.WriteAsync(buffer, 0, bytesRead, statusTaskTokenSource.Token);
            }
            return bytesRead;
        }

        private async Task ProcessMessages(MemoryStream ms)
        {
            byte[] buffer = ms.ToArray();
            int startIdx = 0;
            while (startIdx < buffer.Length)
            {
                int startIndex = Array.IndexOf(buffer, (byte)1, startIdx);
                if (startIndex == -1)
                {
                    break;
                }
                int endIndex = Array.IndexOf(buffer, (byte)26, startIndex + 1);
                if (endIndex == -1)
                {
                    break;
                }
                byte[] message = new byte[endIndex - startIndex + 1];
                Array.Copy(buffer, startIndex, message, 0, message.Length);
                HandleMessage(message.Skip(1).Take(message.Length - 2).ToArray());
                startIdx = endIndex + 1;
                ms.SetLength(0L);
                await ms.WriteAsync(buffer, startIdx, buffer.Length - startIdx);
            }
        }

        private void HandleMessage(byte[] message)
        {
            _zj17ProductionData = message;
        }

        public override async Task<bool> InitalAsync()
        {
            this.ipAddress = Config.IP;
            this.port = Convert.ToInt32(Config.Port);
            this._DLMachineType = Config.ChannelType.Contains("Fxs") ? DLMachineTypeEnum.FxsZB48 : DLMachineTypeEnum.Huani;
            return true;
        }
    }
}
