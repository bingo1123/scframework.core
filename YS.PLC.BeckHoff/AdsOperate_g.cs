//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TwinCAT.Ads.SumCommand;
//using TwinCAT.Ads;
//using YS.Yuanji.Log;

//namespace YS.PLC.BeckHoff
//{
//    public class AdsOperate
//    {
//        #region 变量定义
//        private TwinCAT.Ads.TcAdsClient adsClient = null;
//        private AmsNetId ipAddress { get; set; }
//        private int port { get; set; }
//        #endregion

//        #region 设备联机和关闭
//        /// <summary>
//        /// 联机方法
//        /// </summary>
//        /// <param name="ipAddress">IP地址倍福的IP地址样式例：127.0.0.1.1.1</param>
//        /// <param name="port">端口号</param>
//        /// <returns></returns>
//        public bool Connect(string ip_Address, int _port = 851)
//        {
//            bool result = false;
//            try
//            {
//                ipAddress = string.IsNullOrEmpty(ip_Address) ? AmsNetId.Local : AmsNetId.Parse(ip_Address);
//                port = _port;
//                if (adsClient != null) { this.Close(); }
//                if (adsClient == null)
//                {
//                    adsClient = new TwinCAT.Ads.TcAdsClient();
//                    adsClient.Connect(ipAddress, port);
//                    if (adsClient.IsConnected)//连接成功后开启重连轮询机制
//                    {
//                        result = true;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogController.Instance.Error("Ads连接失败:" + ex.Message);
//            }
//            if (token == null)
//            {
//                token = new CancellationTokenSource();
//                poolSataus = Task.Factory.StartNew(AdsReconect, TaskCreationOptions.LongRunning);
//            }

//            return result;
//        }
//        public string Close()
//        {
//            string err = "";
//            try
//            {
//                if (token != null)
//                {
//                    try
//                    {
//                        token.Cancel();//取消
//                        token.Dispose();//释放掉
//                        Task.Delay(2000);
//                    }
//                    catch (Exception ex)
//                    {
//                        System.Diagnostics.Debug.WriteLine(ex.Message);
//                    }
//                }
//                adsClient?.Close();//实现设备关闭
//                adsClient?.Dispose();//销毁对象
//            }
//            catch (Exception ex)
//            {
//                err = ex.Message;
//            }
//            finally
//            {
//                adsClient = null;
//            }
//            return err;
//        }
//        #endregion

//        #region 实现设备重连
//        private async Task AdsReconect()
//        {
//            while (!token.IsCancellationRequested)
//            {
//                try
//                {
//                    StateInfo? status = null;
//                    try
//                    {
//                        status = adsClient?.ReadState();
//                    }
//                    catch
//                    {

//                    }
//                    if (status?.DeviceState != 0)
//                    {
//                        try
//                        {
//                            LogController.Instance.Log($"Ads start reconneting....");
//                            adsClient?.Close();
//                            adsClient?.Dispose();
//                            adsClient = null;
//                            if (adsClient == null)
//                            {
//                                adsClient = new TwinCAT.Ads.TcAdsClient();
//                                adsClient.Connect(ipAddress, port);
//                                if (adsClient.IsConnected)//连接成功后开启重连轮询机制
//                                {

//                                }
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            LogController.Instance.Error($"Ads重连失败:{ex}");
//                        }

//                    }
//                }
//                catch (Exception ex)
//                {
//                    LogController.Instance.Error($"Ads重连失败:{ex}");
//                }
//                finally
//                {
//                    await Task.Delay(3000);//每秒检测一次
//                }
//            }
//        }

//        private CancellationTokenSource token;
//        private Task poolSataus = null;
//        #endregion

//        #region 数据读取
//        /// <summary>
//        /// 读取数据
//        /// </summary>
//        /// <param name="symNames">所有的符号名称</param>
//        /// <param name="types">符号类型(bool)</param>
//        /// <returns></returns>
//        Dictionary<string, uint> handles = new Dictionary<string, uint>();
//        List<Type> types = new List<Type>();
//        List<ParAddress> oldParses = null;
//        private readonly object obgReadLock = new object();
//        public (bool, string, Dictionary<string, object>) Read(List<ParAddress> pars)
//        {
//            Dictionary<string, object> results = new Dictionary<string, object>();
//            //首先创建句柄 
//            if (adsClient != null && adsClient.IsConnected)
//            {
//                string errInfo = "";
//                if (oldParses == null || (!oldParses[0].ParName.Equals(pars[0].ParName) || !oldParses[oldParses.Count - 1].ParName.Equals(pars[pars.Count - 1].ParName)))
//                {
//                    handles.Clear();//清除
//                    types.Clear();//清除数据类型
//                    foreach (var item in pars)
//                    {
//                        //创建句柄的时候，如果plc中不存在的符号变量创建不成功句柄并且会报异常
//                        try
//                        {
//                            if (!handles.ContainsKey(item.AddressName))
//                            {
//                                handles.Add(item.AddressName, (uint)adsClient.CreateVariableHandle(item.AddressName));
//                                types.Add(item.ParType);
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            LogController.Instance.Error($"注册失败{item.AddressName}:{ex},");
//                            //handles.Add(item.AddressName, 0);
//                            errInfo += $"注册失败{item.AddressName},";
//                        }
//                    }
//                    oldParses = pars;
//                }
//                if (handles.Count > 0)
//                {
//                    Object[] resultRead = null;
//                    lock (obgReadLock)
//                    {
//                        SumHandleRead readCommand = new SumHandleRead(adsClient, handles.Values.ToArray(), types.ToArray());
//                        resultRead = readCommand.Read();
//                    }
//                    if (resultRead != null && resultRead.Length > 0)
//                    {
//                        int cunt = 0;
//                        foreach (var key in handles.Keys)
//                        {
//                            results.Add(key, resultRead[cunt++]);
//                        }
//                    }
//                    else
//                    {
//                        return (false, "未读到数据", results);
//                    }
//                }
//                return (errInfo.Trim().Length == 0, errInfo, results);
//            }
//            else
//            {
//                return (false, "设备未联机", results);
//            }
//        }
//        /// <summary>
//        /// 读取
//        /// </summary>
//        /// <param name="symNames">标签名称</param>
//        /// <param name="types">数据类型</param>
//        /// <param name="sleep">休眠时间</param>

//        public Dictionary<string, object> Read(List<ParAddress> parInfos, int sleep = 10)
//        {
//            Dictionary<string, object> values = new System.Collections.Generic.Dictionary<string, object>();
//            if (adsClient != null && adsClient.IsConnected)
//            {
//                if (parInfos != null && parInfos.Count > 0)
//                {
//                    foreach (var item in parInfos)
//                    {
//                        var obj = accordingTypeGetValue(item);
//                        if (obj != null)
//                        {
//                            if (!values.ContainsKey(item.ParName))
//                            {
//                                values.Add(item.ParName, obj);
//                            }
//                        }
//                        Thread.Sleep(sleep);
//                    }
//                }
//            }
//            return values;
//        }
//        private object accordingTypeGetValue(ParAddress par)
//        {
//            object obj = null;
//            try
//            {
//                int hIndex = adsClient.CreateVariableHandle(par.ParName);//根据名称创建索引
//                obj = adsClient.ReadAny(hIndex, par.ParType);
//                //if (par.ParType is bool || par.ParType is Boolean)
//                //{
//                //    AdsStream adsStream1 = new AdsStream(1);
//                //    BinaryReader binread = new BinaryReader(adsStream1);
//                //    adsStream1.Position = 0;
//                //    adsClient.ReadAny(hIndex, par.ParType);
//                //    obj = binread.ReadBoolean();
//                //}
//                //else if (par.ParType is float)
//                //{
//                //    AdsStream adsStream1 = new AdsStream(4);
//                //    BinaryReader binread = new BinaryReader(adsStream1);
//                //    adsStream1.Position = 0;
//                //    adsClient.Read(hIndex, adsStream1);
//                //    obj = binread.ReadSingle();
//                //}
//                //else if (par.ParType is Int16)
//                //{
//                //    AdsStream adsStream1 = new AdsStream(2);
//                //    BinaryReader binread = new BinaryReader(adsStream1);
//                //    adsStream1.Position = 0;
//                //    adsClient.Read(hIndex, adsStream1);
//                //    obj = binread.ReadInt16();
//                //}
//                //else if (par.ParType is Int32)
//                //{
//                //    AdsStream adsStream1 = new AdsStream(4);
//                //    BinaryReader binread = new BinaryReader(adsStream1);
//                //    adsStream1.Position = 0;
//                //    adsClient.Read(hIndex, adsStream1);
//                //    obj = binread.ReadInt32();
//                //}
//                //else if (par.ParType is Int64)
//                //{
//                //    AdsStream adsStream1 = new AdsStream(8);
//                //    BinaryReader binread = new BinaryReader(adsStream1);
//                //    adsStream1.Position = 0;
//                //    adsClient.Read(hIndex, adsStream1);
//                //    obj = binread.ReadInt64();
//                //}
//                //else if (par.ParType is double)
//                //{
//                //    AdsStream adsStream1 = new AdsStream(8);
//                //    BinaryReader binread = new BinaryReader(adsStream1);
//                //    adsStream1.Position = 0;
//                //    adsClient.Read(hIndex, adsStream1);
//                //    obj = binread.ReadDouble();
//                //}
//                //else if (par.ParType is string)
//                //{
//                //    AdsStream adsStream1 = new AdsStream(par.Len);
//                //    BinaryReader binread = new BinaryReader(adsStream1);
//                //    adsStream1.Position = 0;
//                //    adsClient.Read(hIndex, adsStream1);
//                //    obj = binread.ReadString();
//                //}
//            }
//            catch (Exception ex)
//            {
//            }
//            return obj;


//        }
//        #endregion
//    }

//    public class ParAddress
//    {
//        public string ParName { get; set; }
//        public string AddressName { get; set; }

//        public Type ParType { get; set; }
//        public int Len { get; set; }//数据长度，字符串需要，其他类型暂时不需要
//    }
//}
