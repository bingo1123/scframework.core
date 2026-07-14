using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using YS.Yuanji.Log;

namespace YS.Yuanji.Commom.Configuration
{
    /// <summary>
    /// HTTP 配置中心客户端
    /// </summary>
    public class HttpConfigCenterClient : IConfigCenterClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _configCenterUrl;
        private readonly string _appId;
        private readonly string _environment;
        private readonly int _timeoutSeconds;

        public HttpConfigCenterClient(string configCenterUrl, string appId, string environment, int timeoutSeconds = 10)
        {
            _configCenterUrl = configCenterUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(configCenterUrl));
            _appId = appId ?? throw new ArgumentNullException(nameof(appId));
            _environment = environment ?? "dev";
            _timeoutSeconds = timeoutSeconds;

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };
        }

        /// <summary>
        /// 从配置中心获取配置
        /// </summary>
        public async Task<string?> GetConfigAsync(string configKey)
        {
            try
            {
                var url = $"{_configCenterUrl}/api/config/{_appId}/{_environment}/{configKey}";
                LogController.Instance.Log($"[ConfigCenter] 正在从配置中心获取配置: {url}");

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    LogController.Instance.Log($"[ConfigCenter] 获取配置失败: HTTP {(int)response.StatusCode}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();

                // 解析配置中心返回的数据结构
                // 假设返回格式: { "code": 0, "data": { "content": "{...}" }, "message": "ok" }
                try
                {
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("code", out var codeElement) && codeElement.GetInt32() != 0)
                    {
                        var message = root.TryGetProperty("message", out var msgElement)
                            ? msgElement.GetString()
                            : "未知错误";
                        LogController.Instance.Log($"[ConfigCenter] 配置中心返回错误: {message}");
                        return null;
                    }

                    if (root.TryGetProperty("data", out var dataElement))
                    {
                        // 如果 data 直接是配置内容字符串
                        if (dataElement.ValueKind == JsonValueKind.String)
                        {
                            return dataElement.GetString();
                        }

                        // 如果 data 是对象，尝试获取 content 字段
                        if (dataElement.ValueKind == JsonValueKind.Object &&
                            dataElement.TryGetProperty("content", out var contentElement))
                        {
                            return contentElement.GetString();
                        }

                        // 否则将整个 data 对象序列化为 JSON
                        return dataElement.GetRawText();
                    }

                    // 如果没有 data 字段，返回整个响应
                    return content;
                }
                catch (JsonException ex)
                {
                    LogController.Instance.Log($"[ConfigCenter] JSON 解析失败，返回原始内容: {ex.Message}");
                    return content; // 返回原始内容
                }
            }
            catch (TaskCanceledException)
            {
                LogController.Instance.Error($"[ConfigCenter] 获取配置超时 ({_timeoutSeconds}秒)");
                return null;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"[ConfigCenter] 获取配置异常: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 检查配置中心是否可用
        /// </summary>
        public async Task<bool> IsAvailableAsync()
        {
            try
            {
                var url = $"{_configCenterUrl}/health";
                var response = await _httpClient.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
