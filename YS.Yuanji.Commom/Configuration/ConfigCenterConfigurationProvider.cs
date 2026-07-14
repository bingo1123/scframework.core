using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using YS.Yuanji.Log;

namespace YS.Yuanji.Commom.Configuration
{
    /// <summary>
    /// 配置中心配置提供程序
    /// 优先从配置中心读取，失败时回退到本地文件
    /// </summary>
    public class ConfigCenterConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly IConfigCenterClient _configCenterClient;
        private readonly string _configKey;
        private readonly string _localFilePath;
        private readonly bool _reloadOnChange;
        private Timer? _reloadTimer;
        private string? _lastConfigHash;

        public ConfigCenterConfigurationProvider(
            IConfigCenterClient configCenterClient,
            string configKey,
            string localFilePath,
            bool reloadOnChange = false)
        {
            _configCenterClient = configCenterClient ?? throw new ArgumentNullException(nameof(configCenterClient));
            _configKey = configKey ?? throw new ArgumentNullException(nameof(configKey));
            _localFilePath = localFilePath ?? throw new ArgumentNullException(nameof(localFilePath));
            _reloadOnChange = reloadOnChange;
        }

        /// <summary>
        /// 加载配置
        /// </summary>
        public override void Load()
        {
            LoadAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// 异步加载配置
        /// </summary>
        private async System.Threading.Tasks.Task LoadAsync()
        {
            // 首先尝试从配置中心获取
            var configJson = await _configCenterClient.GetConfigAsync(_configKey);

            if (!string.IsNullOrEmpty(configJson))
            {
                LogController.Instance.Log($"[ConfigCenter] 成功从配置中心加载配置: {_configKey}");
                ParseConfigJson(configJson);

                // 如果配置热重载开启，启动定时器
                if (_reloadOnChange)
                {
                    StartReloadTimer();
                }
                return;
            }

            // 配置中心获取失败，尝试读取本地文件作为回退
            LogController.Instance.Log($"[ConfigCenter] 配置中心不可用，回退到本地文件: {_localFilePath}");
            LoadFromLocalFile();
        }

        /// <summary>
        /// 从本地文件加载配置
        /// </summary>
        private void LoadFromLocalFile()
        {
            if (!File.Exists(_localFilePath))
            {
                LogController.Instance.Error($"[ConfigCenter] 本地配置文件不存在: {_localFilePath}");
                return;
            }

            try
            {
                var json = File.ReadAllText(_localFilePath);
                ParseConfigJson(json);
                LogController.Instance.Log($"[ConfigCenter] 成功从本地文件加载配置: {_localFilePath}");

                // 如果配置热重载开启，监视文件变化
                if (_reloadOnChange)
                {
                    StartFileWatcher();
                }
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"[ConfigCenter] 加载本地配置文件失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 解析配置 JSON
        /// </summary>
        private void ParseConfigJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

                FlattenJsonElement(root, data, "");

                Data = data;
                _lastConfigHash = CalculateHash(json);
            }
            catch (JsonException ex)
            {
                LogController.Instance.Error($"[ConfigCenter] JSON 解析失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 扁平化 JSON 元素
        /// </summary>
        private void FlattenJsonElement(JsonElement element, Dictionary<string, string?> data, string prefix)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var property in element.EnumerateObject())
                    {
                        var key = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}:{property.Name}";
                        FlattenJsonElement(property.Value, data, key);
                    }
                    break;

                case JsonValueKind.Array:
                    var index = 0;
                    foreach (var item in element.EnumerateArray())
                    {
                        var key = $"{prefix}:{index}";
                        FlattenJsonElement(item, data, key);
                        index++;
                    }
                    break;

                case JsonValueKind.String:
                    data[prefix] = element.GetString();
                    break;

                case JsonValueKind.Number:
                    data[prefix] = element.ToString();
                    break;

                case JsonValueKind.True:
                    data[prefix] = "true";
                    break;

                case JsonValueKind.False:
                    data[prefix] = "false";
                    break;

                case JsonValueKind.Null:
                    data[prefix] = null;
                    break;
            }
        }

        /// <summary>
        /// 启动定时重载
        /// </summary>
        private void StartReloadTimer()
        {
            _reloadTimer = new Timer(async _ =>
            {
                try
                {
                    var configJson = await _configCenterClient.GetConfigAsync(_configKey);
                    if (!string.IsNullOrEmpty(configJson))
                    {
                        var newHash = CalculateHash(configJson);
                        if (newHash != _lastConfigHash)
                        {
                            LogController.Instance.Log($"[ConfigCenter] 配置已更新，正在重载...");
                            ParseConfigJson(configJson);
                            OnReload();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"[ConfigCenter] 重载配置失败: {ex.Message}");
                }
            }, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5)); // 每5分钟检查一次
        }

        /// <summary>
        /// 启动文件监视器
        /// </summary>
        private void StartFileWatcher()
        {
            var directory = Path.GetDirectoryName(_localFilePath);
            var fileName = Path.GetFileName(_localFilePath);

            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(fileName))
            {
                return;
            }

            var watcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            watcher.Changed += (sender, args) =>
            {
                try
                {
                    LogController.Instance.Log($"[ConfigCenter] 本地配置文件已修改，正在重载...");
                    LoadFromLocalFile();
                    OnReload();
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"[ConfigCenter] 重载本地配置失败: {ex.Message}");
                }
            };

            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 计算内容哈希
        /// </summary>
        private string CalculateHash(string content)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hashBytes);
        }

        public void Dispose()
        {
            _reloadTimer?.Dispose();
        }
    }
}
