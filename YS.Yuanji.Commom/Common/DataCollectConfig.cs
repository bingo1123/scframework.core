using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Commom
{
    public class DataCollectConfig
    {
        public string MachineName { get; set; } = "JJ101";

        public int DataCollectIntervel { get; set; } = 3000;

        //149.242.46.149
        public IPAddress MachineIPAddress { get; set; } = IPAddress.Parse("127.0.0.1");

        public int MachinePort { get; set; } = 1200;

        public DLMachineTypeEnum MachineTypeEnum { get; set; } = DLMachineTypeEnum.Huani;

        public int ConnecTimeOut { get; set; } = 3000;

        public int CommunicationTimeOut { get; set; } = 5000;

        public string MqttTopic { get; set; }

        public IPAddress MqttIPAddress { get; set; } = IPAddress.Parse("172.17.39.30");

        public int MqttPort { get; set; } = 1883;

        public string MqttUserName { get; set; } = "fine";

        public string MqttUserPassword { get; set; } = "123456";

        public ProtocolTypeEnum HuaniProtocolTypeEnum { get; set; } = ProtocolTypeEnum.M5;

        public string ZB48AipAddress { get; set; }

        public int ZB48Aport { get; set; }

        public string ZB48AApiId { get; set; }

        public string ZB48AApiSecret { get; set; }

        /// <summary>
        /// 辅料站1 IP地址
        /// </summary>
        public string ExcipientStaionIp1 { get; set; }

        /// <summary>
        /// 辅料站1 端口号
        /// </summary>
        public int ExcipientStaionPort1 { get; set; } = 1200;

        /// <summary>
        /// 辅料站2 IP地址
        /// </summary>
        public string ExcipientStaionIp2 { get; set; }

        /// <summary>
        /// 辅料站2 端口号
        /// </summary>
        public int ExcipientStaionPort2 { get; set; } = 1200;

        /// <summary>
        /// 包装机部位1 IP地址
        /// </summary>
        public string BZMachinePart1 { get; set; }

        /// <summary>
        /// 包装机部位1 端口号
        /// </summary>
        public int BZMachinePort1 { get; set; } = 2000;


        /// <summary>
        /// 包装机部位2 IP地址
        /// </summary>
        public string BZMachinePart2 { get; set; }

        /// <summary>
        /// 包装机部位2 端口号
        /// </summary>
        public int BZMachinePort2 { get; set; } = 2000;

        /// <summary>
        /// 包装机部位3 IP地址
        /// </summary>
        public string BZMachinePart3 { get; set; }

        /// <summary>
        /// 包装机部位3 端口号
        /// </summary>
        public int BZMachinePort3 { get; set; } = 2000;

        public void LoadDictionary(Dictionary<string, string> configContent)
        {
            if (configContent == null || configContent.Count <= 0)
            {
                return;
            }
            PropertyInfo[] properties = typeof(DataCollectConfig).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (string.IsNullOrEmpty(property.Name) || !configContent.ContainsKey(property.Name))
                {
                    continue;
                }
                DLMachineTypeEnum type2;
                if (property.PropertyType == typeof(int))
                {
                    if (int.TryParse(configContent[property.Name], out var Value))
                    {
                        property.SetValue(this, Value);
                    }
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(this, configContent[property.Name]);
                }
                else if (property.PropertyType == typeof(IPAddress) && !string.IsNullOrEmpty(configContent[property.Name]))
                {
                    IPAddress iP = IPAddress.Parse(configContent[property.Name]);
                    property.SetValue(this, iP);
                }
                else if (property.PropertyType == typeof(ProtocolTypeEnum) && !string.IsNullOrEmpty(configContent[property.Name]))
                {
                    if (Enum.TryParse<ProtocolTypeEnum>(configContent[property.Name], out var type))
                    {
                        property.SetValue(this, type);
                    }
                }
                else if (property.PropertyType == typeof(DLMachineTypeEnum) && !string.IsNullOrEmpty(configContent[property.Name]) && Enum.TryParse<DLMachineTypeEnum>(configContent[property.Name], out type2))
                {
                    property.SetValue(this, type2);
                }
            }
        }
    }
}
