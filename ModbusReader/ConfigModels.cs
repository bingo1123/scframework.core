using System.Collections.Generic;

public class AppConfig
{
    public ConnectionConfig Connection { get; set; }
    public List<PointConfig> Points { get; set; }
    public MqttConfig Mqtt { get; set; }

    public BasicInfo Basic { get; set; }
}

public class ConnectionConfig
{
    public string MachineIPAddress { get; set; } = "127.0.0.1";
    public int MachinePort { get; set; } = 502;
    public byte SlaveId { get; set; } = 1;
    public bool AutoReconnect { get; set; } = true;
    public int MaxReconnectAttempts { get; set; } = 5;
    public int ReconnectDelay { get; set; } = 2000;
    public int HeartbeatInterval { get; set; } = 5000;
    public int ReadInterval { get; set; } = 1000;
    public string MqttIPAddress { get; set; } = "172.17.39.30";
    public int MqttPort { get; set; } = 1883;
    public string MqttUserName { get; set; } = "fine";
    public string MqttUserPassword { get; set; } = "123456";
    public string MachineCode { get; set; } = "SFY01";

    public string MachineName { get; set; } = "JJ209";
}

public class MqttConfig
{
    public string IpAddress { get; set; } = "172.17.39.30";
    public int Port { get; set; } = 1883;
    public string User { get; set; } = "fine";
    public string Password { get; set; } = "123456";

}

public class PointConfig
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string RegisterType { get; set; } = "HoldingRegister";
    public ushort Address { get; set; }
    public string DataType { get; set; } = "UInt16";
    public string Unit { get; set; } = "";
}

public class BasicInfo
{ 
    public string ServerRoot { get; set; } = "";

    public string TerminalCode { get; set; } = "";

    public string Program { get; set; } = "";

}

public class DeviceConfig
{ 
    public string MachineName { get; set; } = "JJ209";
    public string MachineIPAddress { get; set; } = "192.168.10.57";
    public string MachinePort { get; set; } = "502";
    public string MqttIPAddress { get; set; } = "172.17.39.30";
    public string MqttUserName { get; set; } = "fine";
    public string MqttUserPassword { get; set; } = "123456";
    public string MachineCode { get; set; } = "SFY01";
}