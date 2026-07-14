using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace YS.Yuanji.WPF
{
    public static class ConfigManager
    {
        private static readonly string ConfigFileName = "appconfig.json";
        private static readonly string ConfigFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);

        public static AppConfig LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath, Encoding.UTF8);
                    return JsonConvert.DeserializeObject<AppConfig>(json) ?? CreateDefaultConfig();
                }
                else
                {
                    var defaultConfig = CreateDefaultConfig();
                    SaveConfig(defaultConfig);
                    return defaultConfig;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载配置文件失败: {ex.Message}");
                return CreateDefaultConfig();
            }
        }

        public static void SaveConfig(AppConfig config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存配置文件失败: {ex.Message}");
            }
        }

        private static AppConfig CreateDefaultConfig()
        {
            return new AppConfig
            {
                Mqtt = new MqttConfig
                {
                    Server = "localhost",
                    Port = 1883,
                    ClientId = "YS.Yuanji.WPF.Client"
                },
                MachineNumbers = new List<string> { "MT-001", "MT-002", "MT-003", "MT-004", "MT-005" }
            };
        }
    }
}