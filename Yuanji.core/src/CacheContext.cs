using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using YS.Yuanji.Drive;
using YS.Yuanji.Commom;
using YS.Yuanji.Log;

namespace Yuanji.core.Cahce
{
    public class CacheContext
    {
        public Dictionary<string, string> TerminalConfig { get; private set; }

        public BasicConfig BasicConfig { get; private set; }
        public DataConfig DataConfig { get; private set; }

        private static readonly object Locker = new object();

        private static CacheContext _instance;

        public static CacheContext Instance
        {
            get
            {
                if (_instance == null)
                    lock (Locker)
                    {
                        if (_instance == null)
                            _instance = new CacheContext();
                    }
                return _instance;
            }
        }

        public CacheContext()
        {
            TerminalConfig = new Dictionary<string, string>();

            //InitConfig();
        }

        public bool InitConfig(string ConfigUrl)
        {
            try
            {
                var getConfigUrl = ConfigUrl;
                string result = GetAsync(getConfigUrl).GetAwaiter().GetResult();
                // 使用 JObject 解析 JSON 字符串
                JObject json = JObject.Parse(result);

                // 获取 "result" 键的值
                if ((bool)json["success"])
                {
                    var resultValue = (JArray)json["result"];
                    foreach (JObject item in ((JArray)json["result"]))
                    {
                        var key = item.GetValue("configKey").ToString();
                        var val = item.GetValue("configVal").ToString();
                        this.SetTerminal(key, val);
                    }
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TerminalConfig.dat"), JsonConvert.SerializeObject(TerminalConfig));
                }
                else
                {
                    LogController.Instance.Error("从接口获取配置返回格式错误,请与管理员联系【" + (string)json["error"] + "】");
                    TerminalConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TerminalConfig.dat")));
                }
                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error("从接口获取配置文件时报错，请与管理员联系【" + ex.ToString() + "】");
                TerminalConfig = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TerminalConfig.dat")));
                //_logger.LogError(ex, "从接口获取配置文件时报错");
                return false;
            }
        }

        private void SetTerminal(string key, string jsonVal)
        {
            if (TerminalConfig.ContainsKey(key))
            {
                TerminalConfig[key] = jsonVal;
            }
            else
            {
                TerminalConfig.Add(key, jsonVal);
            }
        }

        public string GetTerminal(string key)
        {
            if (TerminalConfig.ContainsKey(key))
            {
                return TerminalConfig[key];
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<string> GetAsync(string url)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(10); // 设置超时时间
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token; // 设置取消令牌
                HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // 确保响应成功
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
