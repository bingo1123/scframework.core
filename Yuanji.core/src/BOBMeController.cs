using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using  Yuanji.core.src.Driver.Huani.Controller;
using  Yuanji.core.src.Driver.Huani.Entity;
using YS.Yuanji.Log;

namespace Yuanji.core.src
{
    public  class BOBMeController:DataCollectContoller
    {
        public BOBMeController(MqttController mqttController) : base(mqttController)
        {
        }

        private async Task BOBMeStationCaller(int id)
        {

        }


        private async Task<bool> BOBMeCollectDataAct(int id)//TODO 设备参数的为添加
        {
            if (_chanle?.IsConnected() != true)
            {
                return false;
            }

            var tt = DateTime.Now;
            bool flag = false;
            BOBMeShiftData? shift = null;
            BOBMeCurrentData? currentData = null;
            List<Pvalue> brandParams = new List<Pvalue>();
            string bobName = id == 0 ? "BOBME1" : "BOBME2";

                byte[] command = null;
                byte[] response = null;

                byte[] CurCommand = null;
                byte[] CurResponse = null;

                byte[] brandCommand = null;
                byte[] brandResponse = null;
                try
                {
                    command = HuaniCommand.PackCommandBySymbArray(1, 2, BoBMeConst.BobProductData, ProtocolTypeEnum.BOBME);

                    response = await _chanle.SendCommandAsync(command);

                    await Task.Delay(ControllerConst.DefaultHauniIntervel);

                    //CurCommand = HuaniCommand.PackCommandBySymbArray(2, 1, BoBMeConst.BobHMIData, HuaniProtocolTypeEnum.BOBME);
                    //CurResponse = _bobme.SendCommand(CurCommand); //读取 2，1 不稳定，已经取消

                    brandCommand = HuaniCommand.PackCommand(0, 0
       , VisuCommandNameEnum.ReadBrandParameter, ProtocolTypeEnum.BOBME, numberParam: 0).Item1;
                    brandResponse = await _chanle.SendCommandAsync(brandCommand);

                    if (HuaniCommand.CheckIfResponsIsValid(response, nameof(BoBMeConst.BobProductData)))
                    {
                        shift = BOBMeDataParser.BOBProductDataParse(response);
                    }

                    //if (HuaniCommand.CheckIfResponsIsValid(CurResponse, nameof(BoBMeConst.BobHMIData)))
                    //{
                    //    currentData = BOBMeDataParser.BOBCurrentDataParse(CurResponse);
                    //}

                    if (HuaniCommand.CheckIfResponsIsValid(brandResponse, nameof(VisuCommandNameEnum.ReadBrandParameter)))
                    {
                        brandParams = HuaniCommand.ParseBrandPvalue(brandResponse, ProtocolTypeEnum.BOBME);
                    }
                    flag = true;
                }
                catch (Exception ex)
                {
                    flag = false;
                    LogController.Instance.Error($"BOBMeCollectDataAct occur exception:{ex},{ex?.StackTrace}");
                }

            if (flag)
            {
                RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
                realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                realTimeDataDto.Details = new List<DataItemDetailDto>();
                realTimeDataDto.MachineId = config.MachineName;
                try
                {
                    if (shift != null && shift.HasValue)
                    {
                        var fields = typeof(BOBMeShiftData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            var fieldName = $"{config.MachineName}.{bobName}.{field.Name}"; // 字段名
                            var fieldValue = field.GetValue(shift); // 当前值
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                        }
                    }

                    //if (currentData != null && currentData.HasValue)
                    //{
                    //    var fields = typeof(BOBMeCurrentData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    //    foreach (var field in fields)
                    //    {
                    //        var fieldName = $"{config.MachineName}.{bobName}.{field.Name}"; // 字段名
                    //        var fieldValue = field.GetValue(currentData); // 当前值
                    //        realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                    //    }
                    //}

                    if (brandParams != null && brandParams?.Count > 0)
                    {
                        foreach (var item in brandParams)
                        {
                            if (item.ParameterValue == null)
                                continue;
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item.ParameterValue, Code = $"{config.MachineName}.{bobName}.Brand.{item.ParaNumerName}" });
                        }
                    }

                    if (realTimeDataDto.Details != null && realTimeDataDto.Details.Count > 0)
                    {
                        var res = await _mqttController.ConnectAsync();
                        res = await _mqttController.PublishMessageAsync($"{MqttConst.MqttTopicRealTime}/{config.MachineName}", JsonConvert.SerializeObject(realTimeDataDto),false);
                        LogController.Instance.Log($"MQTT publish realTime status:{res}");
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Log($"in BOBMeCollectDataAct occur exception:{ex}");
                }
            }

            return flag;
        }
    }
}
