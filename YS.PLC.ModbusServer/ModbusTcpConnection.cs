using NModbus;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;

namespace YS.PLC.ModbusServer
{
    public class ModbusTcpConnection : ParentChanel
    {
        private TcpClient _tcpClient;
        private IModbusMaster _modbusMaster;
        private volatile bool _isConnected;
        private string _ip;
        private int _port = 502;
        private byte _slaveId = 1;
        private Task poolSataus;

        public override async Task<bool> ConnectAsync()
        {
            try
            {
                _tcpClient?.Dispose();
                _tcpClient = new TcpClient();
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await _tcpClient.ConnectAsync(_ip, _port, cts.Token);
                var factory = new ModbusFactory();
                // 创建Modbus主站
                _modbusMaster = factory.CreateMaster(_tcpClient);
               // LogController.Instance.Error($"Modbus TCP连接1，IP:{_ip},Port:{_port}");
                // 测试连接
                await _modbusMaster.ReadHoldingRegistersAsync(_slaveId, 0, 1);
                // 更新连接状态
                _isConnected = true;
               // LogController.Instance.Error($"Modbus TCP连接2，IP:{_ip},Port:{_port}");
            }
            catch (Exception ex)
            {
                _isConnected = false;
                LogController.Instance.Error($"Modbus TCP连接失败，IP:{_ip},Port:{_port},异常信息:{ex.Message}");
                //return false;
            }

            if (poolSataus == null)
            {
                poolSataus = Task.Factory.StartNew(AdsReconect);
            }

            return _isConnected;
        }


        public override async Task<bool> DisconnectAsync()
        {
            try
            {
                _tcpClient?.Close();
                _isConnected = false;
                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"断开连接错误: {ex.Message}");
                return false;
            }
        }

        public override Task<bool> InitalAsync()
        {
            _ip = Config.IP;
            _port = Config.Port;
            _slaveId = Config.ParameterDict.ContainsKey("SlaveId") ? byte.Parse(Config.ParameterDict["SlaveId"]) : (byte)1;
            return base.InitalAsync();
        }

        public override bool IsConnected()
        {
            return _isConnected;
        }

        public override async Task<(bool, string, Dictionary<Item, object>)> ReadAsync(List<Item> items, [CallerMemberName] string? name = null)
        {
            Dictionary<Item, object> results = new Dictionary<Item, object>();
            //foreach (Item item in items)
            //{
            //    try
            //    {
            //        var value = ReadPointAsync(item).Result;
            //        results[item] = value;
            //    }
            //    catch (Exception ex)
            //    {
            //        LogController.Instance.Error($"读取点位失败，地址:{item.Address},异常信息:{ex.Message}");
            //    }
            //}
            var modbusmodels = ConvertModbusModel(items).GroupBy(l => l.RegisterType);

            foreach (var group in modbusmodels)
            {
                try
                {
                    if (group.Key == RegisterType.Coil || group.Key == RegisterType.DiscreteInput)
                    {
                        var coilResults = await ReadMultipleCoilAsync(group, group.Key);
                        for (int i = 0; i < group.Count(); i++)
                        {
                            var item = group.ElementAt(i);
                            results[items.First(l => ConvertModbusModel(l).Address == item.Address && ConvertModbusModel(l).RegisterType == item.RegisterType)] = coilResults[item.Address];
                        }
                    }
                    else if (group.Key == RegisterType.HoldingRegister || group.Key == RegisterType.InputRegister)
                    {
                        var registerResults = await ReadMultipleRegisterAsync(group, group.Key);
                        for (int i = 0; i < group.Count(); i++)
                        {
                            var item = group.ElementAt(i);

                            // 检查地址范围
                            if (item.Address + item.Length > registerResults.Length)
                            {
                                LogController.Instance.Error($"寄存器地址{item.Address}，超出范围");
                                continue;
                            }

                            ushort[] subRegisters = new ushort[item.Length];
                            Array.Copy(registerResults, item.Address, subRegisters, 0, item.Length);
                            var value = ConvertRegisters(subRegisters, item.DataType, item.ByteOrder);
                            results[items.First(l => ConvertModbusModel(l).Address == item.Address && ConvertModbusModel(l).RegisterType == item.RegisterType)] = value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"批量读取点位失败，寄存器类型:{group.Key},异常信息:{ex.Message}");
                }
            }

            if (results.Count > 0)
            {
                return (true, "读取成功", results);
            }
            else
            {
                return (false, "部分点位读取失败", results);
            }

        }

        public override async Task<(bool, string)> WriteAsync(Item item, object value, [CallerMemberName] string? name = null)
        {
            byte slaveId = _slaveId;
            var modbusmodel = ConvertModbusModel(item);
            switch (modbusmodel.RegisterType)
            {
                case RegisterType.Coil:
                    bool coilValue = Convert.ToBoolean(value);
                    await WriteSingleCoilAsync(slaveId, modbusmodel.Address, coilValue);
                    return (true, "写入成功");

                case RegisterType.DiscreteInput:
                    throw new NotSupportedException("无法写入输入点");
                case RegisterType.InputRegister:
                    throw new NotSupportedException("无法写入输入寄存器");
                case RegisterType.HoldingRegister:

                    if (modbusmodel.DataType == DataType.Word || modbusmodel.DataType == DataType.UInt16 || modbusmodel.DataType == DataType.Byte)
                    {
                        if(!ushort.TryParse(value.ToString(),out var _v))
                        {
                            return (false, "写入失败,数据不是有效的格式.");
                        }
                        await WriteSingleRegisterAsync(slaveId, modbusmodel.Address, _v);
                    }
                    else
                    {
                        throw new NotSupportedException("不支持的数据类型");
                    }

                    return (true, "写入成功");
                default:
                    break;
            }

            return (false, "写入失败,不是有效的数据地址类型.");
        }

        // 读取单个点位
        private async Task<object> ReadPointAsync(Item point)
        {
            byte slaveId = _slaveId;
            var modbusmodel = ConvertModbusModel(point);
            switch (modbusmodel.RegisterType)
            {
                case RegisterType.Coil:
                    bool[] coils = await _modbusMaster.ReadCoilsAsync(slaveId, modbusmodel.Address, 1);
                    return coils[0];
                case RegisterType.DiscreteInput:
                    bool[] inputs = await _modbusMaster.ReadInputsAsync(slaveId, modbusmodel.Address, 1);
                    return inputs[0];
                case RegisterType.InputRegister:
                case RegisterType.HoldingRegister:
                    ushort[] registers = await _modbusMaster.ReadHoldingRegistersAsync(slaveId, modbusmodel.Address, (ushort)modbusmodel.Length);
                    //return ParseRegisterValue(registers, modbusmodel.DataType);
                    return registers[0];
                default:
                    throw new NotSupportedException($"不支持的寄存器类型: {modbusmodel.RegisterType}");
            }
        }

        // 读取多个点位
        private async Task<ushort[]> ReadMultipleRegisterAsync(IEnumerable<ModbusModel> items, RegisterType registerType)
        {
            byte slaveId = _slaveId;
            var modbusmodel = items;
            switch (registerType)
            {
                case RegisterType.InputRegister:
                case RegisterType.HoldingRegister:
                    ushort[] registers = await _modbusMaster.ReadHoldingRegistersAsync(slaveId, modbusmodel.Last().Address, (ushort)(modbusmodel.First().Address + modbusmodel.First().Length));
                    return registers;
                default:
                    throw new NotSupportedException($"不支持的寄存器类型: {registerType}");
            }
        }

        // 读取多个点位
        private async Task<bool[]> ReadMultipleCoilAsync(IEnumerable<ModbusModel> items, RegisterType registerType)
        {
            byte slaveId = _slaveId;
            var modbusmodel = items;
            switch (registerType)
            {
                case RegisterType.Coil:
                    bool[] coils = await _modbusMaster.ReadCoilsAsync(slaveId, modbusmodel.Last().Address, modbusmodel.First().Address);
                    return coils;
                case RegisterType.DiscreteInput:
                    bool[] inputs = await _modbusMaster.ReadInputsAsync(slaveId, modbusmodel.Last().Address, modbusmodel.First().Address);
                    return inputs;
                default:
                    throw new NotSupportedException($"不支持的寄存器类型: {registerType}");
            }
        }

        // 解析寄存器值
        private object ParseRegisterValue(ushort[] registers, string dataType)
        {
            switch (dataType)
            {
                case "BYTE":
                    return registers[0];

                case "WORD":
                case "UInt16":
                    return registers[0];
                case "Int16":
                    return (short)registers[0];

                case "DWORD":
                case "UInt32":
                    return ((uint)registers[0] << 16) | registers[1];

                case "Int32":
                    return ((int)registers[0] << 16) | registers[1];
                case "REAL":
                case "Float":
                    byte[] bytes = {
                    (byte)(registers[0] >> 8),
                    (byte)registers[0],
                    (byte)(registers[1] >> 8),
                    (byte)registers[1]
                  };
                    // 转换为浮点数（.NET 默认使用小端序，需要处理字节顺序）
                    if (BitConverter.IsLittleEndian)
                    {
                        // 反转字节顺序以适应小端序系统
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToSingle(bytes, 0);

                case "Double":
                    byte[] bytes2 = {
                    (byte)(registers[0] >> 8),
                    (byte)registers[0],
                    (byte)(registers[1] >> 8),
                    (byte)registers[1],
                    (byte)(registers[2] >> 8),
                    (byte)registers[2],
                    (byte)(registers[3] >> 8),
                    (byte)registers[3]
                  };
                    // 转换为浮点数（.NET 默认使用小端序，需要处理字节顺序）
                    if (BitConverter.IsLittleEndian)
                    {
                        // 反转字节顺序以适应小端序系统
                        Array.Reverse(bytes2);
                    }
                    return BitConverter.ToDouble(bytes2, 0);

                default:
                    return registers[0];
            }
        }

        // 获取数据类型所需寄存器数量
        private ushort GetDataTypeStep(string dataType)
        {
            return dataType switch
            {
                "BYTE" => 1,
                "UInt16" => 1,
                "Int16" => 1,
                "WORD" => 1,
                "UInt32" => 2,
                "Int32" => 2,
                "REAL" => 2,
                "DWORD" => 2,
                "Float" => 2,
                "Double" => 4,
                _ => 1
            };
        }

        private DataType GetDataType(string dataType)
        {
            return dataType switch
            {
                "BYTE" => DataType.Byte,
                "UInt16" => DataType.UInt16,
                "Int16" => DataType.Int16,
                "WORD" => DataType.UInt16,
                "UInt32" => DataType.UInt32,
                "Int32" => DataType.Int32,
                "REAL" => DataType.Float,
                "DWORD" => DataType.UInt32,
                "Float" => DataType.Float,
                "Double" => DataType.Double,
                _ => DataType.UInt16
            };
        }

        private ByteOrder GetByteOrder(string dataType)
        {
            return dataType switch
            {
                "Little" => ByteOrder.LittleEndian,
                "Little-S" => ByteOrder.LittleEndianSwap,
                "Big" => ByteOrder.BigEndian,
                "Big-S" => ByteOrder.BigEndianSwap,
                _ => ByteOrder.BigEndian
            };
        }

        private ModbusModel ConvertModbusModel(Item item)
        {
            return new ModbusModel
            {
                Address = (ushort)(Convert.ToUInt16(item.Address[1..]) - 1),
                RegisterType = item.Address[0] switch
                {
                    '0' => RegisterType.Coil,
                    '1' => RegisterType.DiscreteInput,
                    '4' => RegisterType.HoldingRegister,
                    '3' => RegisterType.InputRegister,
                    _ => throw new ArgumentException("无效的寄存器类型")
                },
                DataType = GetDataType(item.ValueType),
                Length = GetDataTypeStep(item.ValueType),
                ByteOrder = GetByteOrder(item.AddressType)

            };
        }

        private List<ModbusModel> ConvertModbusModel(List<Item> items)
        {
            return items.Select(item => ConvertModbusModel(item)).OrderByDescending(l => l.Address).ToList();
        }

        #region 数据类型转换辅助方法

        // 将两个 ushort 转换为 float (IEEE 754)
        public float ConvertToFloat(ushort highWord, ushort lowWord)
        {
            byte[] bytes = new byte[4];
            BitConverter.GetBytes(highWord).CopyTo(bytes, 0);
            BitConverter.GetBytes(lowWord).CopyTo(bytes, 2);
            return BitConverter.ToSingle(bytes, 0);
        }

        // 将四个 ushort 转换为 double (IEEE 754)
        public double ConvertToDouble(ushort word1, ushort word2, ushort word3, ushort word4)
        {
            byte[] bytes = new byte[8];
            BitConverter.GetBytes(word1).CopyTo(bytes, 0);
            BitConverter.GetBytes(word2).CopyTo(bytes, 2);
            BitConverter.GetBytes(word3).CopyTo(bytes, 4);
            BitConverter.GetBytes(word4).CopyTo(bytes, 6);
            return BitConverter.ToDouble(bytes, 0);
        }

        // 将两个 ushort 转换为 32位整数
        public int ConvertToInt32(ushort highWord, ushort lowWord)
        {
            return (highWord << 16) | lowWord;
        }

        // 将四个 ushort 转换为 64位整数
        public long ConvertToInt64(ushort word1, ushort word2, ushort word3, ushort word4)
        {
            ulong value = ((ulong)word1 << 48) | ((ulong)word2 << 32) | ((ulong)word3 << 16) | word4;
            return (long)value;
        }

        #endregion

        #region 高级数据类型读取

        // 读取单个 float 值
        public async Task<float> ReadFloatAsync(byte slaveId, ushort startAddress)
        {
            try
            {
                ushort[] registers = await _modbusMaster.ReadHoldingRegistersAsync(slaveId, startAddress, 2);
                // 注意字节序，可能需要交换高低字
                return ConvertToFloat(registers[1], registers[0]); // Little Endian
                                                                   // return ConvertToFloat(registers[0], registers[1]); // Big Endian
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取Float失败: {ex.Message}");
                return 0.0f;
            }
        }

        // 读取单个 double 值
        public async Task<double> ReadDoubleAsync(byte slaveId, ushort startAddress)
        {
            try
            {
                ushort[] registers = await _modbusMaster.ReadHoldingRegistersAsync(slaveId, startAddress, 4);
                return ConvertToDouble(registers[3], registers[2], registers[1], registers[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取Double失败: {ex.Message}");
                return 0.0;
            }
        }

        // 读取 32位整数
        public async Task<int> ReadInt32Async(byte slaveId, ushort startAddress)
        {
            try
            {
                ushort[] registers = _modbusMaster.ReadHoldingRegisters(slaveId, startAddress, 2);
                return ConvertToInt32(registers[1], registers[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取Int32失败: {ex.Message}");
                return 0;
            }
        }

        // 读取 64位整数
        public long ReadInt64(byte slaveId, ushort startAddress)
        {
            try
            {
                ushort[] registers = _modbusMaster.ReadHoldingRegisters(slaveId, startAddress, 4);
                return ConvertToInt64(registers[3], registers[2], registers[1], registers[0]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取Int64失败: {ex.Message}");
                return 0;
            }
        }

        // 读取字符串 (假设每个字符占用一个字节，两个字符存储在一个寄存器中)
        public string ReadString(byte slaveId, ushort startAddress, ushort length)
        {
            try
            {
                // 计算需要读取的寄存器数量
                int registerCount = (length + 1) / 2;
                ushort[] registers = _modbusMaster.ReadHoldingRegisters(slaveId, startAddress, (ushort)registerCount);

                byte[] bytes = new byte[length];
                for (int i = 0; i < registers.Length && i * 2 < length; i++)
                {
                    bytes[i * 2] = (byte)(registers[i] >> 8);
                    if (i * 2 + 1 < length)
                        bytes[i * 2 + 1] = (byte)(registers[i] & 0xFF);
                }

                // 移除尾部的空字符
                int actualLength = Array.IndexOf(bytes, (byte)0);
                if (actualLength == -1) actualLength = bytes.Length;

                return System.Text.Encoding.ASCII.GetString(bytes, 0, actualLength);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取字符串失败: {ex.Message}");
                return string.Empty;
            }
        }
        #endregion

        #region 高级数据类型写入

        // 写入单个线圈
        public async Task<bool> WriteSingleCoilAsync(byte slaveId, ushort address, bool value)
        {
            try
            {
                await _modbusMaster.WriteSingleCoilAsync(slaveId, address, value);
                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入线圈失败: {ex.Message}");
                return false;
            }
        }

        // 写入多个线圈
        public async Task<bool> WriteMultipleCoilsAsync(byte slaveId, ushort startAddress, bool[] values)
        {
            try
            {
                await _modbusMaster.WriteMultipleCoilsAsync(slaveId, startAddress, values);
                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入多个线圈失败: {ex.Message}");
                return false;
            }
        }

        // 写入单个保持寄存器
        public async Task<bool> WriteSingleRegisterAsync(byte slaveId, ushort address, ushort value)
        {
            await _modbusMaster.WriteSingleRegisterAsync(slaveId, address, value);
            return true;
        }

        // 写入多个保持寄存器
        public async Task<bool> WriteMultipleRegistersAsync(byte slaveId, ushort startAddress, ushort[] values)
        {
            try
            {
                await _modbusMaster.WriteMultipleRegistersAsync(slaveId, startAddress, values);
                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入多个寄存器失败: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region 高级数据类型写入

        // 写入 Float 值
        public async Task<bool> WriteFloatAsync(byte slaveId, ushort startAddress, float value)
        {
            try
            {
                byte[] bytes = BitConverter.GetBytes(value);
                ushort[] registers = new ushort[2];

                // Little Endian 格式
                registers[0] = BitConverter.ToUInt16(bytes, 2); // 高字
                registers[1] = BitConverter.ToUInt16(bytes, 0); // 低字

                // Big Endian 格式 (如果需要)
                // registers[0] = BitConverter.ToUInt16(bytes, 0); // 高字
                // registers[1] = BitConverter.ToUInt16(bytes, 2); // 低字

                return await WriteMultipleRegistersAsync(slaveId, startAddress, registers);
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入Float失败: {ex.Message}");
                return false;
            }
        }

        // 写入 Double 值
        public async Task<bool> WriteDoubleAsync(byte slaveId, ushort startAddress, double value)
        {
            try
            {
                byte[] bytes = BitConverter.GetBytes(value);
                ushort[] registers = new ushort[4];

                // Little Endian 格式
                registers[0] = BitConverter.ToUInt16(bytes, 6);
                registers[1] = BitConverter.ToUInt16(bytes, 4);
                registers[2] = BitConverter.ToUInt16(bytes, 2);
                registers[3] = BitConverter.ToUInt16(bytes, 0);

                return await WriteMultipleRegistersAsync(slaveId, startAddress, registers);
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入Double失败: {ex.Message}");
                return false;
            }
        }

        // 写入 32位整数
        public async Task<bool> WriteInt32Async(byte slaveId, ushort startAddress, int value)
        {
            try
            {
                ushort[] registers = new ushort[2];
                registers[0] = (ushort)(value >> 16); // 高字
                registers[1] = (ushort)(value & 0xFFFF); // 低字

                return await WriteMultipleRegistersAsync(slaveId, startAddress, registers);
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入Int32失败: {ex.Message}");
                return false;
            }
        }

        // 写入 64位整数
        public async Task<bool> WriteInt64(byte slaveId, ushort startAddress, long value)
        {
            try
            {
                ushort[] registers = new ushort[4];
                ulong ulongValue = (ulong)value;

                registers[0] = (ushort)(ulongValue >> 48);
                registers[1] = (ushort)((ulongValue >> 32) & 0xFFFF);
                registers[2] = (ushort)((ulongValue >> 16) & 0xFFFF);
                registers[3] = (ushort)(ulongValue & 0xFFFF);

                return await WriteMultipleRegistersAsync(slaveId, startAddress, registers);
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入Int64失败: {ex.Message}");
                return false;
            }
        }

        // 写入字符串
        public async Task<bool> WriteStringAsync(byte slaveId, ushort startAddress, string value)
        {
            try
            {
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(value);
                // 确保字节数组长度为偶数
                if (bytes.Length % 2 != 0)
                {
                    Array.Resize(ref bytes, bytes.Length + 1);
                    bytes[bytes.Length - 1] = 0; // 填充空字符
                }

                ushort[] registers = new ushort[bytes.Length / 2];
                for (int i = 0; i < registers.Length; i++)
                {
                    registers[i] = (ushort)((bytes[i * 2] << 8) | bytes[i * 2 + 1]);
                }

                return await WriteMultipleRegistersAsync(slaveId, startAddress, registers);
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"写入字符串失败: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region  数据转换辅助类
        // 修正字节序处理
        private object ConvertRegisters(ushort[] registers, DataType dataType, ByteOrder byteOrder)
        {
            int requiredRegisters = GetRegisterCount(dataType);
            if (registers.Length < requiredRegisters)
                throw new ArgumentException($"寄存器数量不足");

            // 处理字节序：仅调整寄存器顺序，不反转整个数组
            ushort[] orderedRegisters = BitConverter.IsLittleEndian ? ((byteOrder == ByteOrder.BigEndian || byteOrder == ByteOrder.BigEndianSwap)&& requiredRegisters > 1)
                ? registers.Reverse().ToArray()
                : registers : registers;

            byte[] bytes = orderedRegisters
                .SelectMany(r => byteOrder == ByteOrder.LittleEndianSwap || byteOrder == ByteOrder.BigEndianSwap
                    ? BitConverter.GetBytes(r).Reverse().ToArray() // 寄存器内部字节序
                    : BitConverter.GetBytes(r))
                .ToArray();
            switch (dataType)
            {
                case DataType.Float:
                    return BitConverter.ToSingle(bytes, 0);
                case DataType.Byte:
                    return bytes[0];
                case DataType.Int32:
                    return BitConverter.ToInt32(bytes, 0);
                case DataType.UInt32:
                    return BitConverter.ToUInt32(bytes, 0);
                case DataType.Int16:
                    return BitConverter.ToInt16(bytes, 0);
                case DataType.UInt16:
                    return BitConverter.ToUInt16(bytes, 0);
                case DataType.Double:
                    return BitConverter.ToDouble(bytes, 0);
                case DataType.DWord: // 新增DWORD处理
                    return BitConverter.ToUInt32(bytes, 0);
                default:
                    throw new ArgumentException("不支持的数据类型");
            }
        }

        // 更新：增加DWORD类型支持
        private int GetRegisterCount(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Float:
                case DataType.Int32:
                case DataType.UInt32:
                    return 2;
                case DataType.Byte:
                case DataType.Int16:
                case DataType.UInt16:
                case DataType.Word:
                    return 1;
                case DataType.Double:
                    return 4;
                case DataType.DWord: // DWORD=2个寄存器
                    return 2;
                default:
                    throw new ArgumentException("无效数据类型");
            }
        }

        // 更新枚举
        #endregion

        private async Task AdsReconect()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_isConnected)
                    {
                        await _modbusMaster.ReadHoldingRegistersAsync(_slaveId, 0, 1);
                    }
                    else
                    {
                        await ConnectAsync();
                    }
                }
                catch (Exception ex)
                {
                    _isConnected = false;
                }
                finally
                {
                    await Task.Delay(5000);
                }
            }
        }
    }
}
