using System;
using System.Threading.Tasks;

namespace YS.Yuanji.Commom.Configuration
{
    /// <summary>
    /// 配置中心客户端接口
    /// </summary>
    public interface IConfigCenterClient
    {
        /// <summary>
        /// 从配置中心获取配置 JSON 字符串
        /// </summary>
        /// <param name="configKey">配置键名</param>
        /// <returns>配置 JSON 字符串，获取失败返回 null</returns>
        Task<string?> GetConfigAsync(string configKey);

        /// <summary>
        /// 配置中心是否可用
        /// </summary>
        Task<bool> IsAvailableAsync();
    }
}
