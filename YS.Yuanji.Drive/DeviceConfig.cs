using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Drive
{
    public class DeviceConfig
    {
        public string MachineName { get; set; }

        public string MachineCode { get; set; }

        public string MachinePart { get; set; }

        public string ChanleType { get; set; }
        public string Ip { get; set; } = "192.168.1.1";

        public int Port { get; set; } = 8080;

        public string MachineType { get; set; }

        public bool IsEnable { get; set; } = true;

        public int Intervaltime { get; set; } = 3000;
       
        /// <summary>
        /// 设备扩展参数
        /// </summary>
        public Dictionary<string, string> ParameterDict { get; set; } = new Dictionary<string, string>();
        public bool IsContaiName { get;  set; } = false;
        public bool ControlWrite { get;  set; } = false;
    }

    public class BasicConfig
    {
        public List<DeviceConfig> DeviceConfig { get; set; } = new List<DeviceConfig>();

    }
}
