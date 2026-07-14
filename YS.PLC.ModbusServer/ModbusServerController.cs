using NModbus;
using System.Diagnostics.Tracing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;
using Yuanji.Manage;

namespace YS.PLC.ModbusServer
{
    public class ModbusServerController : DataCollectContoller
    {
        private readonly IModbusFactory _modbusFactory;
        private TcpListener _tcpListener;
        private IModbusSlaveNetwork _slaveNetwork;
        private readonly Dictionary<ushort, ushort> _dataStore;
        private CancellationTokenSource _cancellationTokenSource;
        private ControllerManage manage;
        private Dictionary<ushort, DataMode> addressMap = new Dictionary<ushort, DataMode>();
        private string deviceName;
        public ModbusServerController(MqttController mqttController, ControllerManage dataManage) : base(mqttController)
        {
            _modbusFactory = new ModbusFactory();
            _dataStore = new Dictionary<ushort, ushort>();
            manage = dataManage;
        }

        public async Task StartAsync(string ipAddress, int port)
        {
            await Task.Delay(3000);
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _tcpListener.Start();

            _slaveNetwork = _modbusFactory.CreateSlaveNetwork(_tcpListener); 

            // 创建从站 (Slave ID = 1)
            var slave = _modbusFactory.CreateSlave(1, new ModbusDataStore(_dataStore));
            _slaveNetwork.AddSlave(slave);

            _cancellationTokenSource = new CancellationTokenSource();

            LogController.Instance.Log($"Modbus TCP 服务端已启动，监听 {ipAddress}:{port}");

            // 启动监听
            Task.Run(async()=>  await _slaveNetwork.ListenAsync(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _tcpListener?.Stop();
            LogController.Instance.Log("Modbus TCP 服务端已停止");
        }

        public void UpdateFloat(ushort address, float value)
        {
            /// 将float转换为字节数组
            byte[] bytes = BitConverter.GetBytes(value);

            // 按照Modbus RTU标准，使用大端序存储
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            // 将4个字节拆分成两个16位寄存器
            ushort highRegister = (ushort)((bytes[0] << 8) | bytes[1]);
            ushort lowRegister = (ushort)((bytes[2] << 8) | bytes[3]);
            _dataStore[address] = highRegister;     // 高位寄存器
            _dataStore[(ushort)(address + 1)] = lowRegister;  // 低位寄存器
        }

        public void UpdateInt(ushort address, ushort value)
        {
            if (_dataStore.ContainsKey(address))
            {
                _dataStore[address] = value;
            }
            else
            {
                _dataStore.Add(address, value);
            }
        }

        public void UpdateBool(ushort address,ushort status)
        {
            if (_dataStore.ContainsKey(address))
            {
                _dataStore[address] = status;
            }
            else
            {
                _dataStore.Add(address, status);
            }
        }

        protected override Task RealtimeAsync(List<Item> items)
        {
            var ma = manage.GetController(deviceName) as DataCollectContoller;

            foreach(var item in addressMap)
            {
                var dv = ma?.DataManage?.GetData(deviceName, item.Value.Name)?.Value;
                if (dv != null)
                {
                    SettData(item.Key, dv);
                }
                else
                {
                    //LogController.Instance.Log($"设备{deviceName}数据{item.Value.Name}不存在:采用默认值{item.Value.Value}");
                    SettData(item.Key, item.Value.Value);
                }
            }

            return base.RealtimeAsync(items);
        }

        public override async Task Initialization()
        {
            deviceName = DeviceConfig.ParameterDict["DeviceName"];
            InitData();
            await StartAsync(DeviceConfig.Ip, DeviceConfig.Port);
        }
        private void InitData()
        {
            try
            {
                foreach (var ivs in interItems.Values)
                {
                    if (ivs?.Params == null || ivs.Params.Count == 0)
                    {
                        continue;
                    }

                    foreach (var config in ivs.Params)
                    {
                        if (!string.IsNullOrEmpty(config.Extended))
                        {
                            var file = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, config.Extended);
                            if (File.Exists(file))
                            {
                                var item = File.ReadAllLines(file);
                                foreach (var line in item)
                                {
                                    if (!string.IsNullOrWhiteSpace(line))
                                    {
                                        var args = line.Split('\t');
                                        if (args.Length > 0)
                                        {
                                            addressMap.Add(Convert.ToUInt16(args[0]), new DataMode 
                                            { 
                                                Name = args[1],
                                                Value = args.Length>2 ? args[2]:0,
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogController.Instance.Error("InitData:" + e.Message);
            }
        }

        private void SettData(ushort key,object value)
        {
            try
            {
                //if (value is bool b)
                //{
                //    UpdateBool(key, b ? (ushort)1 : (ushort)0);
                //}
                //else
                //if (value is ushort|| value is UInt16)
                //{
                //    var us = Convert.ToUInt16(value);
                //    UpdateInt(key,  us);
                //}
                //else
                //if (value is float fl)
                //{
                //    UpdateFloat(key, fl);
                //}
                ////else
                ////if(value is uint || value is int)
                ////{
                ////    UpdateFloat(key, value);
                ////}
                //else
                //{
                //    throw new NotSupportedException($"地址4{key.ToString("0000")}不支持的数据类型");
                //}
                UpdateValue(key, value);
            }
            catch(Exception e)
            {
                LogController.Instance.Warning("SettData:" + e.Message);
            }
        }

        public void UpdateValue<T>(ushort address, T value) where T : struct
        {
            byte[] bytes;
            Type type = typeof(T);

            // 处理不同类型
            if (type == typeof(bool))
            {
                _dataStore[address] = (bool)(object)value ? (ushort)1 : (ushort)0;
                return;
            }
            else if (type == typeof(byte))
            {
                _dataStore[address] =(byte)(object)value;
                return;
            }
            else if (type == typeof(short) || type == typeof(ushort) || type == typeof(Int16) || type == typeof(UInt16))
            {
                bytes = type == typeof(short) ?
                    BitConverter.GetBytes((short)(object)value) :
                    BitConverter.GetBytes((ushort)(object)value);
            }
            else if (type == typeof(int) || type == typeof(uint))
            {
                bytes = type == typeof(int) ?
                    BitConverter.GetBytes((int)(object)value) :
                    BitConverter.GetBytes((uint)(object)value);
            }
            else if (type == typeof(float))
            {
                bytes = BitConverter.GetBytes((float)(object)value);
            }
            else if (type == typeof(long) || type == typeof(ulong))
            {
                bytes = type == typeof(long) ?
                    BitConverter.GetBytes((long)(object)value) :
                    BitConverter.GetBytes((ulong)(object)value);
            }
            else if (type == typeof(double))
            {
                bytes = BitConverter.GetBytes((double)(object)value);
            }
            else
            {
                throw new ArgumentException($"地址4{address.ToString("0000")}不支持的数据类型: {type.Name}");
            }

            // 统一的大端序处理
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            // 计算需要的寄存器数量
            int registerCount = bytes.Length / 2;

            for (int i = 0; i < registerCount; i++)
            {
                ushort register = (ushort)((bytes[i * 2] << 8) | bytes[i * 2 + 1]);
                _dataStore[(ushort)(address + i)] = register;
            }
        }

        public void UpdateValue(ushort address, object value)
        {
            byte[] bytes;
            Type type = value.GetType();
            // 处理不同类型
            if (type == typeof(bool))
            {
                _dataStore[address] = (bool)(object)value ? (ushort)1 : (ushort)0;
                return;
            }
            else if (type == typeof(Byte))
            {
                bytes = BitConverter.GetBytes((Byte)(object)value);
            }
            else if (type == typeof(short) || type == typeof(ushort) || type == typeof(Int16) || type == typeof(UInt16))
            {
                bytes = type == typeof(short)|| type == typeof(Int16) ?
                    BitConverter.GetBytes((short)(object)value) :
                    BitConverter.GetBytes((ushort)(object)value);
            }
            else if (type == typeof(int) || type == typeof(uint))
            {
                bytes = type == typeof(int) ?
                    BitConverter.GetBytes((int)(object)value) :
                    BitConverter.GetBytes((uint)(object)value);
            }
            else if (type == typeof(float)|| type == typeof(Single))
            {
                bytes = BitConverter.GetBytes((float)(object)value);
            }
            else if (type == typeof(long) || type == typeof(ulong))
            {
                bytes = type == typeof(long) ?
                    BitConverter.GetBytes((long)(object)value) :
                    BitConverter.GetBytes((ulong)(object)value);
            }
            else if (type == typeof(double))
            {
                bytes = BitConverter.GetBytes((double)(object)value);
            }
            else
            {
                throw new ArgumentException($"地址4{address.ToString("0000")}不支持的数据类型: {type.Name}");
            }

            //LogController.Instance.Log("UpdateValue:" + address.ToString("0000") + ":" + value.ToString());
            // 统一的大端序处理
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            // 计算需要的寄存器数量
            int registerCount = bytes.Length / 2;

            for (int i = 0; i < registerCount; i++)
            {
                ushort register = (ushort)((bytes[i * 2] << 8) | bytes[i * 2 + 1]);
                _dataStore[(ushort)(address + i)] = register;
            }
        }
    }

   
}
