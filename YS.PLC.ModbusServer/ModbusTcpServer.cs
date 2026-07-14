using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using YS.PLC.ModbusServer;

public class ModbusTcpServer
{
    private readonly IModbusFactory _modbusFactory;
    private TcpListener _tcpListener;
    private IModbusSlaveNetwork _slaveNetwork;
    private readonly Dictionary<ushort, ushort> _dataStore;
    private CancellationTokenSource _cancellationTokenSource;

    // 模拟的温度和速度参数地址
    private const ushort Temperature1Address = 0;    // 温度1 (寄存器 0)
    private const ushort Temperature2Address = 1;    // 温度2 (寄存器 1)
    private const ushort Speed1Address = 2;          // 速度1 (寄存器 2)
    private const ushort Speed2Address = 3;          // 速度2 (寄存器 3)
    private const ushort StatusAddress = 4;          // 状态 (寄存器 4)

    public ModbusTcpServer()
    {
        _modbusFactory = new ModbusFactory();
        _dataStore = new Dictionary<ushort, ushort>();
        InitializeDataStore();
    }

    private void InitializeDataStore()
    {
        // 初始化默认值
        _dataStore[Temperature1Address] = 250;  // 25.0°C (放大10倍存储)
        _dataStore[Temperature2Address] = 300;  // 30.0°C
        _dataStore[Speed1Address] = 1500;       // 1500 RPM
        _dataStore[Speed2Address] = 1200;       // 1200 RPM
        _dataStore[StatusAddress] = 1;          // 运行状态
    }

    public async Task StartAsync(IPAddress ipAddress, int port)
    {
        _tcpListener = new TcpListener(ipAddress, port);
        _tcpListener.Start();

        _slaveNetwork = _modbusFactory.CreateSlaveNetwork(_tcpListener);

        // 创建从站 (Slave ID = 1)
        var slave = _modbusFactory.CreateSlave(1, new ModbusDataStore(_dataStore));
        _slaveNetwork.AddSlave(slave);

        _cancellationTokenSource = new CancellationTokenSource();

        Console.WriteLine($"Modbus TCP 服务端已启动，监听 {ipAddress}:{port}");

        // 启动监听
        await _slaveNetwork.ListenAsync(_cancellationTokenSource.Token);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _tcpListener?.Stop();
        Console.WriteLine("Modbus TCP 服务端已停止");
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

    public void UpdateBool(ushort status)
    {
        if (_dataStore.ContainsKey(StatusAddress))
        {
            _dataStore[StatusAddress] = status;
        }
        else
        {
            _dataStore.Add(StatusAddress, status);
        }
    }

    // 模拟数据变化
    public async Task SimulateDataChangesAsync()
    {
        var random = new Random();
        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            try
            {
                // 模拟温度在 20-40°C 之间波动
                float temp1 = 20.0f + (float)(random.NextDouble() * 20);
                float temp2 = 22.0f + (float)(random.NextDouble() * 18);

                // 模拟速度在 1000-2000 RPM 之间波动
                ushort speed1 = (ushort)(1000 + random.Next(1000));
                ushort speed2 = (ushort)(1000 + random.Next(1000));

                UpdateFloat(Temperature1Address, temp1);
                UpdateFloat(Temperature2Address, temp2);
                UpdateInt(Speed1Address, speed1);
                UpdateInt(Speed2Address, speed2);

                Console.WriteLine($"更新数据 - 温度1: {temp1:F1}°C, 温度2: {temp2:F1}°C, " +
                                $"速度1: {speed1} RPM, 速度2: {speed2} RPM");

                await Task.Delay(5000, _cancellationTokenSource.Token); // 每5秒更新一次
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}