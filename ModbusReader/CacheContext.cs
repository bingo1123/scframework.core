using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

public class CacheContext
{
    public ConnectionConfig TerminalConfig;

    public CacheContext()
    {
        TerminalConfig = new ConnectionConfig();

        //InitConfig();
    }

    public async Task<bool> InitConfig(BasicInfo basic)
    {
        try
        {
            var getConfigUrl = basic.ServerRoot + "/api/workshop/sysmanager/TerminalUpdate/GetConfig?terminalCode=" + basic.TerminalCode + "&program=" + basic.Program;
            string result = await GetAsync(getConfigUrl);
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
                TerminalConfig = JsonConvert.DeserializeObject<ConnectionConfig>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TerminalConfig.dat")));
            }
            return true;
        }
        catch (Exception ex)
        {
            LogController.Instance.Error("从接口获取配置文件时报错，请与管理员联系【" + ex.ToString() + "】");
            TerminalConfig = JsonConvert.DeserializeObject<ConnectionConfig>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TerminalConfig.dat")));
            //_logger.LogError(ex, "从接口获取配置文件时报错");
            return false;
        }
    }

    private void SetTerminal(string key, string jsonVal)
    {
        try
        {
            var item = TerminalConfig.GetType().GetProperties().FirstOrDefault(x => x.Name == key);
            if (item != null)
            {
                if (item.PropertyType == typeof(string))
                {
                    item.SetValue(TerminalConfig, jsonVal);
                }
                else if (item.PropertyType == typeof(int))
                {
                    item.SetValue(TerminalConfig, int.Parse(jsonVal));
                }
                else if (item.PropertyType == typeof(bool))
                {
                    item.SetValue(TerminalConfig, bool.Parse(jsonVal));
                }
                else if (item.PropertyType == typeof(double))
                {
                    item.SetValue(TerminalConfig, double.Parse(jsonVal));
                }
                else if (item.PropertyType == typeof(byte))
                {
                    item.SetValue(TerminalConfig, byte.Parse(jsonVal));
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.CommLogger.Error("SetTerminal:" + ex.ToString());
        }
    }

    public async Task<string> GetAsync(string url)
    {
        using (var client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // 确保响应成功
            return await response.Content.ReadAsStringAsync();
        }
    }
}
