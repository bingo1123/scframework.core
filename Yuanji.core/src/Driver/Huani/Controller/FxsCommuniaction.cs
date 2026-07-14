using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using YS.Yuanji.Log;
using Yuanji.core.srcDriver.Huani.Controller;
using YS.Yuanji.Drive;
using YS.Yuanji.Commom;

namespace  Yuanji.core.src.Driver.Huani.Controller
{
    public class FxsCommuniaction : ParentChanel
    {
        private TcpClient client;

        private NetworkStream stream;

        private string ipAddress;

        private int port;

        private CancellationTokenSource listenTaskTokenSource;

        private bool _isConnected = false;

        private int readIntervalMilliseconds = 20;

        private Task statusTask;

        private int statusCheckIntervalMilliseconds = 5000;

        private CancellationTokenSource statusTaskTokenSource;

        private readonly AsyncLock _asyncLock = new AsyncLock();

        private bool isconnecting = false;

        public FxsCommuniaction()
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

        private async Task<int> ReadDataAsync(MemoryStream ms, byte[] buffer, CancellationToken token)
        {
            int bytesRead = await ReadWithTimeoutAsync(buffer, 0, buffer.Length, 5000);
            if (bytesRead > 0)
            {
                await ms.WriteAsync(buffer, 0, bytesRead, statusTaskTokenSource.Token);
            }
            return bytesRead;
        }

        public override async Task<bool> InitalAsync()
        {
            this.ipAddress = Config.IP;
            this.port = Convert.ToInt32(Config.Port);
            return true;
        }
    }
}
