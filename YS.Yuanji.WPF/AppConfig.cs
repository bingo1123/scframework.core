using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YS.Yuanji.WPF
{
    public class AppConfig
    {
        public MqttConfig Mqtt { get; set; } = new MqttConfig();
        public List<string> MachineNumbers { get; set; } = new List<string>();
    }

    public class MqttConfig
    {
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 1883;
        public string ClientId { get; set; } = "YS.Yuanji.WPF.Client";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}