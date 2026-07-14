using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Mqtt
{
    public class MqttConfig
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }

        public string Host { get; set; } = "192.168.1.1";

        public int Port { get; set; } = 1883;

        /// <summary>
        /// 扩展参数
        /// </summary>
        public Dictionary<string, string> ParameterDict { get; set; } = new Dictionary<string, string>();

    }
}
