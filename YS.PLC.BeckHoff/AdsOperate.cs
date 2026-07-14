using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using YS.Yuanji.Commom;
using YS.Yuanji.Commom.Common;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;

namespace YS.PLC.BeckHoff
{
    public class AdsOperate : ParentChanel
    {
        #region 变量定义
        private AdsClient adsClient;
        private AmsNetId ipAddress { get; set; }
        private int port { get; set; }
        private CancellationTokenSource token;
        private Task poolSataus;
        private string ip_Address;
        #endregion

        public AdsOperate()
        {
            token = new CancellationTokenSource();
        }

        #region 设备联机和关闭
        /// <summary>
        /// 联机方法
        /// </summary>
        /// <param name="ipAddress">IP地址倍福的IP地址样式例：127.0.0.1.1.1</param>
        /// <param name="port">端口号</param>
        /// <returns></returns>
        public override async Task<bool> ConnectAsync()
        {
            bool result = false;
            try
            {
                ipAddress = string.IsNullOrEmpty(ip_Address) ? AmsNetId.Local : AmsNetId.Parse(ip_Address);
                if (adsClient == null)
                {
                    var timeout = Task.Delay(5000);
                    adsClient = new AdsClient();
                    var task = await Task.WhenAny(adsClient.ConnectAsync(new AmsAddress(ipAddress, port), token.Token), timeout);
                    if (task == timeout)
                    {
                        LogController.Instance.Error("Ads连接失败:" + "connect timed out!");
                    }
                    else
                    {
                        if (adsClient.IsConnected)//连接成功后开启重连轮询机制
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogController.Instance.Error("Ads连接失败:" + ex.Message);
            }
            if (poolSataus == null)
            {
                poolSataus = Task.Factory.StartNew(AdsReconect);
            }

            return result;
        }
        #endregion

        #region 实现设备重连
        private async Task AdsReconect()
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!adsClient.IsConnected)
                    {
                        LogController.Instance.Log($"Ads start reconneting....");
                        adsClient?.Close();
                        var timeout = Task.Delay(5000);
                        adsClient = new AdsClient();
                        var task = await Task.WhenAny(adsClient.ConnectAsync(new AmsAddress(ipAddress, port), token.Token), timeout);
                        if (task == timeout)
                        {
                            LogController.Instance.Error("Ads重连失败:" + "connect timed out!");
                        }
                        else
                        {
                            if (adsClient.IsConnected)//连接成功后开启重连轮询机制
                            {

                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"Ads重连失败:{ex}");
                }
                finally
                {
                    await Task.Delay(3000);//每秒检测一次
                }
            }
        }

        #endregion

        #region 数据读取

        public async Task<(bool, string, Dictionary<string, object>)> ReadAsync(List<ParAddress> pars)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();
            string errInfo = string.Empty;
            //首先创建句柄 
            if (adsClient != null && adsClient.IsConnected)
            {
                foreach (var item in pars)
                {
                    //创建句柄的时候，如果plc中不存在的符号变量创建不成功句柄并且会报异常
                    try
                    {
                        //var handle = await adsClient.CreateVariableHandleAsync(item.AddressName,token.Token);
                        var result = adsClient.TryReadValue(item.AddressName, item.ParType, out var _value);

                        if (result.Succeeded() && _value != null)
                        {
                            results.Add(item.AddressName, _value);
                        }
                        else
                        {
                            LogController.Instance.Error($"read name:{item.AddressName} is error:{result.ThrowOnError()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogController.Instance.Error($"注册失败{item.AddressName}:{ex},");
                        //handles.Add(item.AddressName, 0);
                        errInfo += $"注册失败{item.AddressName},";
                    }
                }

                return (results.Count() > 0, errInfo, results);
            }
            else
            {
                return (false, "设备未联机", results);
            }
        }
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="symNames">标签名称</param>
        /// <param name="types">数据类型</param>
        /// <param name="sleep">休眠时间</param>

        public Dictionary<string, object> Read(List<ParAddress> parInfos, int sleep = 10)
        {
            Dictionary<string, object> values = new System.Collections.Generic.Dictionary<string, object>();
            if (adsClient != null && adsClient.IsConnected)
            {
                if (parInfos != null && parInfos.Count > 0)
                {
                    foreach (var item in parInfos)
                    {
                        var obj = accordingTypeGetValue(item);
                        if (obj != null)
                        {
                            if (!values.ContainsKey(item.ParName))
                            {
                                values.Add(item.ParName, obj);
                            }
                        }
                        Thread.Sleep(sleep);
                    }
                }
            }
            return values;
        }


        public override async Task<(bool,string)> WriteAsync(Item parAddress, object value, [CallerMemberName] string? name = null)
        {
            string errInfo = string.Empty;
            bool result = false;
            if (adsClient != null && adsClient.IsConnected)
            {
                try
                {
                    var writeResult = adsClient.TryWriteValue(parAddress.Address, value);
                    if (writeResult.Succeeded())
                    {
                        result = true;
                    }
                    else
                    {
                        writeResult.ThrowOnError();
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"写入失败{parAddress.Address}:{ex}");
                    errInfo = ex.Message;
                }
            }
            else
            {
                errInfo = "Aads设备未联机";
            }
            return (result, errInfo);
        }

        private async Task<object> accordingTypeGetValue(ParAddress par)
        {
            object obj = null;
            try
            {
                var hIndex = await adsClient.CreateVariableHandleAsync(par.ParName, token.Token);//根据名称创建索引
                obj = adsClient.ReadAny(hIndex.Handle, par.ParType);
            }
            catch (Exception ex)
            {
            }
            return obj;


        }

        public override async Task<bool> DisconnectAsync()
        {
            try
            {
                if (token != null) 
                {
                    try
                    {
                        token.Cancel();//取消
                        token.Dispose();//释放掉
                        Task.Delay(2000);
                    }
                    catch (Exception ex)
                    {
                        LogController.Instance.Error("ads close error:" + ex.Message);
                    }
                }
                if(adsClient != null)
               return await adsClient.DisconnectAsync(CancellationToken.None);//销毁对象

            }
            catch (Exception ex)
            {
                LogController.Instance.Error("ads close error:" + ex.Message);
            }
            finally
            {
            }
            return false;
        }

        public override bool IsConnected()
        {
            return adsClient?.IsConnected ?? false;
        }

        public override Task<bool> InitalAsync()
        {
            ip_Address = Config.IP;
            port = Config.Port;
            return base.InitalAsync();
        }

        public override Task<(bool, string, Dictionary<Item, object>)> ReadAsync(List<Item> items, [CallerMemberName] string? name = null)
        {
            var paras = items;// ConvertParAddress(items);
            Dictionary<Item, object> results = new Dictionary<Item, object>();
            string errInfo = string.Empty;
            //首先创建句柄 
            if (adsClient != null && adsClient.IsConnected)
            {
                foreach (var item in paras)
                {
                    //创建句柄的时候，如果plc中不存在的符号变量创建不成功句柄并且会报异常
                    try
                    {
                        var tpye = ConvertDataType.GetSystemType(item.ValueType);
                        //var handle = await adsClient.CreateVariableHandleAsync(item.AddressName,token.Token);
                        var result =adsClient.TryReadValue(item.Address, tpye, out var _value);

                        if (result.Succeeded() && _value != null)
                        {
                            results.Add(item, _value);
                        }
                        else
                        {
                            LogController.Instance.Error($"read name:{item.Address} is error:{result.ThrowOnError()}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogController.Instance.Error($"注册失败{item.Address}:{ex},");
                        //handles.Add(item.AddressName, 0);
                        errInfo += $"注册失败{item.Address},";
                    }
                }

                return Task.FromResult((results.Count() > 0, errInfo, results));
            }
            else
            {
                return Task.FromResult((false, "设备未联机", results));
            }

        }
        #endregion
         
        private List<ParAddress> ConvertParAddress(List<Item> items)
        {
            var paras = items.Select(l => new ParAddress
            {
                ParName = l.Name,
                ParType = ConvertDataType.GetSystemType(l.ValueType),
                AddressName = l.Address,
                Len = l.AddressLen
            }).ToList();
            return paras;
        }
    }

    public class ParAddress
    {
        public string ParName { get; set; }
        public string AddressName { get; set; }

        public Type ParType { get; set; }
        public int Len { get; set; }//数据长度，字符串需要，其他类型暂时不需要
    }

         
}
