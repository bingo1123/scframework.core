using Microsoft.Extensions.Configuration;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;
using YS.Yuanji.Mqtt;

namespace Yuanji.Manage
{
    public class StarupSevice
    {
        private readonly ControllerManage _controllers;
        private Dictionary<string, List<ParameterItems>> _dataconfig;
        private List<DeviceConfig> _deviceConfig;
        private MqttConfig _mqttConfig;
        private Func<string,Type, object> _func;
        private MqttController _mqtt;
        private IConfiguration _config;
        public StarupSevice(ControllerManage controllerManage ,Func<string, Type, object> func, MqttController mqtt,IConfiguration configuration)
        {
            _controllers = controllerManage;
            _func = func;
            _mqtt = mqtt;
            _config = configuration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Initial();
                var mqtt =new  Task(async () =>
                {
                    _mqtt.LoadConfig();
                    await _mqtt.ConnectAsync();
                });

                var yu =new Task(async () =>
                {
                    foreach (var device in _deviceConfig)
                    {
                        try
                        {
                            if (!device.IsEnable)
                            {
                                continue;
                            }

                            var targetType = _func(device.MachineType,typeof(IDevice)) as IDevice;
                            var chanle = _func(device.ChanleType, typeof(IChanlel)) as IChanlel;
                            if (targetType != null)
                            {
                                var key = string.IsNullOrEmpty(device.MachinePart) ? device.MachineCode:device.MachineCode + "." + device.MachinePart;
                                _controllers.AddController(key, targetType);
                                _dataconfig.TryGetValue(key, out var data);
                                await targetType.InitAsync(chanle??new ParentChanel(), device, data);
                                await targetType.StartAsync();
                            }
                            else
                            {
                                // 可选：记录日志或处理未找到类型的情况
                                LogController.Instance?.Error($"未找到匹配的控制器类型: {device.MachineType}");
                            }
                        }
                        catch (Exception ex)
                        {
                            // 可选：记录日志或处理异常的情况
                            LogController.Instance?.Error($"处理设备时发生异常: {ex}");
                        }
                    }
                });

                mqtt.Start();
                yu.Start();
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"读取设备参数出错...");
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var controller in _controllers.GetAllControllers())
            {
                try
                {
                   await controller.StoptAsync();
                }
                catch (Exception ex)
                {
                    LogController.Instance?.Error($"停止控制器时发生异常: {ex.Message}");
                }
            }
            _mqtt.OnStop();
            _mqtt.Dispose();
            LogController.Instance?.Log("服务已停止");

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        private void Initial()
        {
            //var exeDir = AppDomain.CurrentDomain.BaseDirectory;
            //var path = Path.Combine(exeDir, ControllerConst.DataConfig);
            //var dataconfig = VisuHelper.ReadFileToObject<DataConfig>(path);
            //var dev = Path.Combine(exeDir, ControllerConst.DeviceConfig);
            //var config = VisuHelper.ReadFileToObject<BasicConfig>(path);

            _deviceConfig = _config.GetSection("DeviceConfig").Get<List<DeviceConfig>>() ?? new  List<DeviceConfig>();
            _dataconfig = _config.GetSection("Params").Get<Dictionary<string,List<ParameterItems>>>() ?? new Dictionary<string, List<ParameterItems>>();

        }
            
    }
}
