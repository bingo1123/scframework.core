using System;
using Microsoft.Extensions.Configuration;

namespace YS.Yuanji.Commom.Configuration
{
    /// <summary>
    /// 配置中心配置源
    /// </summary>
    public class ConfigCenterConfigurationSource : IConfigurationSource
    {
        private readonly IConfigCenterClient _configCenterClient;
        private readonly string _configKey;
        private readonly string _localFilePath;
        private readonly bool _reloadOnChange;

        public ConfigCenterConfigurationSource(
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

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ConfigCenterConfigurationProvider(_configCenterClient, _configKey, _localFilePath, _reloadOnChange);
        }
    }
}
