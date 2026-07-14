using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Yuanji.Manage;
using Microsoft.Extensions.Configuration;
using YS.Yuanji.Commom;
using Microsoft.Extensions.Logging;
using YS.Yuanji.Mqtt;
using YS.Yuanji.Commom.Configuration;
using YS.Yuanji.Log;

namespace YS.Yuanji.Start
{
    public class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    // 1. 首先加载 appsettings.json（必须存在，包含配置中心参数）
                    configHost.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    // 2. 获取已加载的配置（包含 appsettings.json 的内容）
                    var initialConfig = configApp.Build();

                    // 3. 读取配置中心设置
                    var configCenterSection = initialConfig.GetSection("ConfigCenter");
                    var configCenterEnabled = configCenterSection.GetValue<bool>("Enabled");

                    if (configCenterEnabled)
                    {
                        // 配置中心已启用
                        var configCenterUrl = configCenterSection["Url"];
                        var appId = configCenterSection["AppId"] ?? "PtMachine";
                        var environment = configCenterSection["Environment"] ?? "prod";
                        var reloadOnChange = configCenterSection.GetValue<bool>("ReloadOnChange", true);

                        // 设备配置键名
                        var deviceConfigKey = initialConfig["DeviceConfigKey"] ?? "basicconfig";
                        var dataConfigKey = initialConfig["DataConfigKey"] ?? "dataconfig";

                        LogController.Instance.Log($"[Program] 配置中心已启用: {configCenterUrl}");

                        // 使用配置中心加载配置，失败时自动回退到本地文件
                        configApp.AddConfigCenter(
                            configKey: deviceConfigKey,
                            localFilePath: "basicconfig.json",
                            configCenterUrl: configCenterUrl,
                            appId: appId,
                            environment: environment,
                            reloadOnChange: reloadOnChange);

                        configApp.AddConfigCenter(
                            configKey: dataConfigKey,
                            localFilePath: "dataconfig.json",
                            configCenterUrl: configCenterUrl,
                            appId: appId,
                            environment: environment,
                            reloadOnChange: reloadOnChange);
                    }
                    else
                    {
                        // 配置中心未启用，使用本地文件
                        LogController.Instance.Log($"[Program] 配置中心未启用，使用本地配置文件");
                        configApp.AddJsonFile("basicconfig.json", optional: true, reloadOnChange: true);
                        configApp.AddJsonFile("dataconfig.json", optional: true, reloadOnChange: true);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(configure =>
                    {
                        configure.ClearProviders();
                        configure.AddConsole();
                    });
                    services.Configure<MqttConfig>(hostContext.Configuration.GetSection("MqttConfig"));
                    services.RegisterService();
                });
    }
}
