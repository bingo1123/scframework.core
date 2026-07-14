using System;
using Microsoft.Extensions.Configuration;
using YS.Yuanji.Log;

namespace YS.Yuanji.Commom.Configuration
{
    /// <summary>
    /// 配置中心扩展方法
    /// </summary>
    public static class ConfigCenterExtensions
    {
        /// <summary>
        /// 添加配置中心配置
        /// </summary>
        /// <param name="builder">配置构建器</param>
        /// <param name="configKey">配置中心的配置键名</param>
        /// <param name="localFilePath">本地回退文件路径</param>
        /// <param name="configCenterUrl">配置中心地址</param>
        /// <param name="appId">应用ID</param>
        /// <param name="environment">环境名称</param>
        /// <param name="reloadOnChange">是否启用热重载</param>
        /// <returns>配置构建器</returns>
        public static IConfigurationBuilder AddConfigCenter(
            this IConfigurationBuilder builder,
            string configKey,
            string localFilePath,
            string configCenterUrl,
            string appId,
            string environment = "dev",
            bool reloadOnChange = false)
        {
            if (string.IsNullOrEmpty(configCenterUrl))
            {
                LogController.Instance.Log($"[ConfigCenter] 配置中心地址为空，使用本地文件: {localFilePath}");
                builder.AddJsonFile(localFilePath, optional: true, reloadOnChange: reloadOnChange);
                return builder;
            }

            var client = new HttpConfigCenterClient(configCenterUrl, appId, environment);
            var source = new ConfigCenterConfigurationSource(client, configKey, localFilePath, reloadOnChange);
            builder.Add(source);

            return builder;
        }

        /// <summary>
        /// 添加配置中心配置（使用已存在的客户端）
        /// </summary>
        /// <param name="builder">配置构建器</param>
        /// <param name="configCenterClient">配置中心客户端</param>
        /// <param name="configKey">配置中心的配置键名</param>
        /// <param name="localFilePath">本地回退文件路径</param>
        /// <param name="reloadOnChange">是否启用热重载</param>
        /// <returns>配置构建器</returns>
        public static IConfigurationBuilder AddConfigCenter(
            this IConfigurationBuilder builder,
            IConfigCenterClient configCenterClient,
            string configKey,
            string localFilePath,
            bool reloadOnChange = false)
        {
            if (configCenterClient == null)
            {
                LogController.Instance.Log($"[ConfigCenter] 配置中心客户端为空，使用本地文件: {localFilePath}");
                builder.AddJsonFile(localFilePath, optional: true, reloadOnChange: reloadOnChange);
                return builder;
            }

            var source = new ConfigCenterConfigurationSource(configCenterClient, configKey, localFilePath, reloadOnChange);
            builder.Add(source);

            return builder;
        }

        /// <summary>
        /// 从环境变量或配置中读取配置中心设置
        /// </summary>
        /// <param name="builder">配置构建器</param>
        /// <param name="configKey">配置键名</param>
        /// <param name="localFilePath">本地回退文件路径</param>
        /// <param name="reloadOnChange">是否启用热重载</param>
        /// <returns>配置构建器</returns>
        public static IConfigurationBuilder AddConfigCenterWithFallback(
            this IConfigurationBuilder builder,
            string configKey,
            string localFilePath,
            bool reloadOnChange = false)
        {
            // 从环境变量读取配置中心参数
            var configCenterUrl = Environment.GetEnvironmentVariable("CONFIG_CENTER_URL");
            var appId = Environment.GetEnvironmentVariable("CONFIG_CENTER_APPID") ?? "PtMachine";
            var environment = Environment.GetEnvironmentVariable("CONFIG_CENTER_ENV") ?? "prod";

            if (string.IsNullOrEmpty(configCenterUrl))
            {
                LogController.Instance.Log($"[ConfigCenter] 未配置环境变量 CONFIG_CENTER_URL，使用本地文件: {localFilePath}");
                builder.AddJsonFile(localFilePath, optional: true, reloadOnChange: reloadOnChange);
                return builder;
            }

            return builder.AddConfigCenter(configKey, localFilePath, configCenterUrl, appId, environment, reloadOnChange);
        }
    }
}
