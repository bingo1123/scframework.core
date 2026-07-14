using  Yuanji.core.src.Driver.Huani.Controller;
using  Yuanji.core.src.Driver.Huani.Entity;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using YS.Yuanji.Log;
using System.Net.Sockets;
using System.Xml.Linq;
using System;
using System.Reflection;
using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using YS.Yuanji.Commom.helper;

namespace Yuanji.core.src
{
    public  class M5Contoller:DataCollectContoller
    {
        private ConcurrentDictionary<string, bool> StopInfoBuffer = new ConcurrentDictionary<string, bool>();

        private int _currentM5ProductionCount = 0;


        private List<string> functions = new List<string>
        {
          "2,1",
"2,2",
"2,4",
"2,5",
"2,6",
"2,9",
"2,50",
"2,51",
"2,55",
"2,56",
"2,101",
"2,102",
"2,103",
"2,104",
"2,106",
"2,107",
"3,1",
"3,2",
"3,3",
"3,4",
"3,5",
"3,7",
"3,8",
"3,9",
"3,10",
"3,12",
"3,13",
"3,50",
"3,51",
"3,55",
"3,57",
"3,101",
"3,102",
"3,104",
        };

        public M5Contoller(MqttController mqttController) : base(mqttController)
        {
        }
        private async Task M5CollectProdAndRejectDataAction()
        {
            int length = M5Const.ProductData.Length + M5Const.RejectData.Length;
            string[] symbAddress = new string[length];
            Array.Copy(M5Const.ProductData, 0, symbAddress, 0, M5Const.ProductData.Length);
            Array.Copy(M5Const.RejectData, 0, symbAddress, M5Const.ProductData.Length, M5Const.RejectData.Length);
            string[] shis = new string[] { SymbAdrEnum.SHIS.ToString() };
            await M5CollectProdAndRejectDataAct(symbAddress);
            await Task.Delay(ControllerConst.DefaultHauniIntervel);
            await M5CollectStopInfoAct(shis);
            await Task.Delay(ControllerConst.DefaultHauniIntervel);
            await M5RealTimeParameterAct();

        }

        private async Task<bool> M5CollectProdAndRejectDataAct(string[] symbs)
        {
            CigaretteProductDto cigaretteProductDto = new CigaretteProductDto();
            CigratteRejectDto rejectDto = new CigratteRejectDto();
            var tt = DateTime.Now;
            int offset = 73 + (74 * M5Const.ProductData.Length);
            bool flag = false;

                byte[] command = null;
                byte[] response = null;
                try
                {
                    command = HuaniCommand.PackCommandBySymbArray(1, 2, symbs, config.HuaniProtocolTypeEnum);
                    response = await _chanle.SendCommandAsync(command);
                    if (!HuaniCommand.CheckIfResponsIsValid(response, nameof(M5Const.ProductData) + nameof(M5Const.RejectData)))
                        return false;

                    var productData = M5ShiftDataParser.M5ProductDataParse(response);

                    LogController.Instance.Log($"收到M5的生产数据，功能码2，M5ProductDataParse数据为{JsonConvert.SerializeObject(productData)}");

                    tt = DateTime.Now;
                    cigaretteProductDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                    cigaretteProductDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    cigaretteProductDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    cigaretteProductDto.XWJDM = config.MachineName;
                    cigaretteProductDto.NO = 1;
                    cigaretteProductDto.TOTALPRODUCTION = (int)productData.ProductionTotal;
                    cigaretteProductDto.TOTALWASTE = (int)productData.TotalWasteCount;
                    cigaretteProductDto.TOTALWASTEPCT = productData.TotalWastePercent;

                    cigaretteProductDto.EFFMACHINE = (int)productData.EffMachineP;
                    cigaretteProductDto.EFFPRODUCTION = (int)productData.EffProductionP;
                    cigaretteProductDto.TOTALSTOPCNT = (int)productData.StopCnt;
                    cigaretteProductDto.TOTALSTOPTIME = (int)productData.StopTime;
                    cigaretteProductDto.RUNTIME = (int)productData.RunTime;
                    cigaretteProductDto.PRODUCTIONSTARTTIME = VisuHelper.M5Uint32ToDateTime(productData.ShiftStart);
                    cigaretteProductDto.batch = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                    cigaretteProductDto.FILTERTIP = productData.FilterSheetsUsed;
                    cigaretteProductDto.PAPERPAN = productData.SeUsedBobbins;
                    cigaretteProductDto.WPAPERPAN = productData.MaxUsedBobbins;

                    // 
                    var command1 = HuaniCommand.PackCommandBySymbArray(3, 0x34, new string[] { "MachineSpeed" }, config.HuaniProtocolTypeEnum);
                    var response1 = await _chanle.SendCommandAsync(command1);
                    if (HuaniCommand.CheckIfResponsIsValid(response1, "MachineSpeed"))
                    {
                        var devicepara = M5ShiftDataParser.DeviceParameterData(response1);
                        cigaretteProductDto.MACHINESPEED = (int)devicepara.MachineSpeed;
                    }


                    cigaretteProductDto.GATHERTYPE = "A";

                #region 剔除数据
                var rejectData = M5ShiftDataParser.M5RejectDataParse(response, offset);

                //LogController.Instance.Log($"收到M5的剔除数据，功能码2，M5RejectDataParse数据为{JsonConvert.SerializeObject(rejectData)}");
                rejectDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                rejectDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                rejectDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                rejectDto.PM_MP_MACHINEPART_ID = "0";
                rejectDto.XWJDM = config.MachineName;

                rejectDto.REJECTS = new List<RejectInfo>();
                RejectInfo reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道烟支太轻", REJECTVALUE = (int)rejectData.LightWeightFront };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道烟支太轻", REJECTVALUE = (int)rejectData.LightWeightRear };
                rejectDto.REJECTS.Add(reject);


                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道烟支太重", REJECTVALUE = (int)rejectData.HeavyWeightFront };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道烟支太重", REJECTVALUE = (int)rejectData.HeavyWeightRear };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道硬点", REJECTVALUE = (int)rejectData.HardSpotFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道硬点", REJECTVALUE = (int)rejectData.HardSpotRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道软点", REJECTVALUE = (int)rejectData.HardSpotFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道软点", REJECTVALUE = (int)rejectData.HardSpotRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道轻烟端", REJECTVALUE = (int)rejectData.LightEndsFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道轻烟端", REJECTVALUE = (int)rejectData.LightEndsRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道气密性", REJECTVALUE = (int)rejectData.AirtightnessFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道气密性", REJECTVALUE = (int)rejectData.AirtightnessRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道总通气性", REJECTVALUE = (int)rejectData.TotalVentilationFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道总通气性", REJECTVALUE = (int)rejectData.TotalVentilationRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道吸阻", REJECTVALUE = (int)rejectData.PressureDropFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道吸阻", REJECTVALUE = (int)rejectData.PressureDropRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道滤嘴故障", REJECTVALUE = (int)rejectData.FilterFaultFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道滤嘴故障", REJECTVALUE = (int)rejectData.FilterFaultRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道烟支形状", REJECTVALUE = (int)rejectData.CigaretteShapeFrontCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道烟支形状", REJECTVALUE = (int)rejectData.CigaretteShapeRearCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "香烟纸接缝总废品", REJECTVALUE = (int)rejectData.CigPaperSpliceTotalCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "封口接缝总废品", REJECTVALUE = (int)rejectData.TippingSpliceTotalCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "缺失滤嘴废品", REJECTVALUE = (int)rejectData.MissingFilterTotalCnt };
                rejectDto.REJECTS.Add(reject);

                reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "纸张切割废品总数量", REJECTVALUE = (int)rejectData.PaperCutTotalCnt };
                rejectDto.REJECTS.Add(reject);

                rejectDto.XWJDM = config.MachineName;

                rejectDto.GATHERTYPE = "A";

                #endregion
                flag = true;
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"在 M5CollectProdAndRejectDataAct occur exception:{ex}");
                    //LogController.Instance.Error($"Send command:{BitConverter.ToString(command)}");
                    //LogController.Instance.Error($"Response,recvLength:{response.Length},Recv:{BitConverter.ToString(response)}");
                    flag = false;
                }
            if (flag)
            {
                var res = await _mqttController.ConnectAsync();
                res = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicProductData}", JsonConvert.SerializeObject(cigaretteProductDto));
                res = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicReject}", JsonConvert.SerializeObject(rejectDto));
            }
            return true;
        }

        private async Task<bool> M5CollectStopInfoAct(string[] symb)
        {
            CigratteStopInfoDto stopInfoDto = new CigratteStopInfoDto();
            bool flag = false;

            byte[] command = null;
            byte[] response = null;

            try
            {
                command = HuaniCommand.PackCommandBySymbArray(1, 1, symb, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command);
                if (!HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.SHIS)))
                    return false;

                var stopInfos = SHISParser.Parse(response, config.HuaniProtocolTypeEnum);
                LogController.Instance.Log($"收到M5的停止数据，功能码1，M5RejectDataParse数据为{JsonConvert.SerializeObject(stopInfos)}");
                stopInfoDto.PRODUCEDATE_IN = DateTime.Now.ToString(MqttConst.CollectDateFormat);
                stopInfoDto.STOPS = new List<StopInfo>();
                stopInfoDto.SWJDM = string.Empty;
                stopInfoDto.XWJDM = config.MachineName;
                stopInfoDto.PLANWORKORDERNO = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                stopInfoDto.PM_MP_MACHINEPART_ID = "0";
                //stopInfoDto.PLANNO = "";
                stopInfoDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                stopInfoDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                stopInfoDto.GATHERTYPE = "A";
                stopInfoDto.NO = 1;
                for (int i = 0; i < stopInfos.Index; i++)
                {
                    StopInfo stopInfo = new StopInfo();

                    stopInfo.PM_MP_STOPCODE_ID = VisuHelper.GetM5StopCodeForSHIS(stopInfos.Stops[i].No);
                    //stopInfo.STOPSTARTTIME = VisuHelper.M5Uint32ToDateTime(stopInfos.Stops[i].StartTime);
                    //stopInfo.STOPENDTIME = VisuHelper.M5Uint32ToDateTime(stopInfos.Stops[i].EndTime);
                    stopInfo.STOPSTARTTIME = VisuHelper.M5ExtendDatatimeToDateTime(stopInfos.Stops[i].StartTimeX);
                    stopInfo.STOPENDTIME = VisuHelper.M5ExtendDatatimeToDateTime(stopInfos.Stops[i].EndTimeX);

                    stopInfo.STOPTIME = (int)stopInfos.Stops[i].Downtime;

                    if (!StopInfoBuffer.ContainsKey(stopInfo.STOPSTARTTIME))
                        stopInfoDto.STOPS.Add(stopInfo);
                }
                if (stopInfoDto.STOPS.Count > 0)
                    flag = true;
                else
                    flag = false;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"在 M5CollectStopInfoAct occur exception:{ex}");
                LogController.Instance.Error($"Send command:{BitConverter.ToString(command)}");
                LogController.Instance.Error($"Response,recvLength:{response.Length},Recv:{BitConverter.ToString(response)}");
                flag = false;
            }

            if (flag)
            {
                await _mqttController.ConnectAsync();
                var res = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicStopInfo}", JsonConvert.SerializeObject(stopInfoDto));
                if (res)
                {
                    foreach (var item in stopInfoDto.STOPS)
                        StopInfoBuffer.TryAdd(item.STOPSTARTTIME, true);
                }
            }
            return true;
        }

        private async Task<bool> M5RealTimeParameterAct()//TODO 设备参数的为添加
        {
            bool flag = false;
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = config.MachineName;

            try
            {
                var command = HuaniCommand.PackCommand(0, 0
  , VisuCommandNameEnum.ReadBrandParameter, ProtocolTypeEnum.M5, numberParam: 0).Item1;
                var response = await _chanle.SendCommandAsync(command);

                await Task.Delay(ControllerConst.DefaultHauniIntervel);

                byte[] totalShiftData = HuaniCommand.PackCommandBySymbArray(1, 2, M5Const.TotalShiftDataArray, config.HuaniProtocolTypeEnum);
                byte[] totalShiftResponse = await _chanle.SendCommandAsync(totalShiftData);

                await Task.Delay(ControllerConst.DefaultHauniIntervel);
                byte[] productCounterData = HuaniCommand.PackCommandBySymbArray(1, 2, new string[] { "ProductionCounter" }, config.HuaniProtocolTypeEnum);
                byte[] productCounterResponse = await _chanle.SendCommandAsync(productCounterData);

                await Task.Delay(ControllerConst.DefaultHauniIntervel);

                if (HuaniCommand.CheckIfResponsIsValid(response, nameof(VisuCommandNameEnum.ReadBrandParameter)))
                {
                   var brandParams = HuaniCommand.ParseBrandPvalue(response, ProtocolTypeEnum.M5);
                    if (brandParams != null && brandParams?.Count > 0)
                    {
                        foreach (var item in brandParams)
                        {
                            if (item.ParameterValue == null)
                                continue;
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item.ParameterValue, Code = $"{config.MachineName}.Brand.{item.ParaNumerName}" });
                        }
                    }
                }

                if (HuaniCommand.CheckIfResponsIsValid(totalShiftResponse, nameof(M5Const.TotalShiftDataArray)))
                {
                    Dictionary<string, object> res = M5ShiftDataParser.M5ParseTotalShiftData(totalShiftResponse);
                    if (res != null && res.Count > 0)
                    {
                        foreach (var item in res)
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item.Value, Code = $"{config.MachineName}.{item.Key}" });
                    }
                }

                if (HuaniCommand.CheckIfResponsIsValid(productCounterResponse, "ProductionCounter"))
                {
                    ProductionCounter productionCounter = ProductionCounterParser.Parse(productCounterResponse);

                    var fields = typeof(ProductionCounter).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        var fieldName = $"{config.MachineName}.{field.Name}"; // 字段名
                        var fieldValue = field.GetValue(productionCounter); // 当前值
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                    }
                }

                await Task.Delay(ControllerConst.DefaultHauniIntervel);
                var command1 = HuaniCommand.PackCommandBySymbArray(3, 0x34, new string[] { "MachineSpeed" }, config.HuaniProtocolTypeEnum);
                var response1 = await _chanle.SendCommandAsync(command1);
                if (HuaniCommand.CheckIfResponsIsValid(response1, "MachineSpeed"))
                {
                    var devicepara = M5ShiftDataParser.DeviceParameterData(response1);
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = $"{config.MachineName}.MachineSpeed", Value = devicepara.MachineSpeed, IsGood = true });
                }
                await Task.Delay(ControllerConst.DefaultHauniIntervel);
                var command2 = HuaniCommand.PackCommandBySymbArrayTemp(2, 0x68, new string[] { "ServiceTabaccoRodFormation" }, config.HuaniProtocolTypeEnum);
                var response2 = await _chanle.SendCommandAsync(command2);
                if (HuaniCommand.CheckIfResponsIsValid(response2, "ServiceTabaccoRodFormation"))
                {
                    var devicepara = M5ShiftDataParser.DeviceParameteTemprData(response2);
                    var fields = typeof(DeviceParameterTempData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        var fieldName = $"{config.MachineName}.{field.Name}"; // 字段名
                        var fieldValue = field.GetValue(devicepara); // 当前值
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                    }

                }
                await Task.Delay(ControllerConst.DefaultHauniIntervel);
                var command4 = HuaniCommand.PackCommandBySymbArrayTemp(3, 9, new string[] { "st_rollblock_heater_data" }, config.HuaniProtocolTypeEnum, 8);
                var response4 = await _chanle.SendCommandAsync(command4);
                if (HuaniCommand.CheckIfResponsIsValid(response4, "st_rollblock_heater_data"))
                {
                    var devicepara = M5ShiftDataParser.DeviceParameterData4(response4);
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = $"{config.MachineName}.{nameof(devicepara.Temperature4)}", Value = devicepara.Temperature4, IsGood = true });

                }
                await Task.Delay(ControllerConst.DefaultHauniIntervel);
                var command5 = HuaniCommand.PackCommandBySymbArrayTemp(3, 8, new string[] { "tipping_heater_data_1" }, config.HuaniProtocolTypeEnum, 8);
                var response5 = await _chanle.SendCommandAsync(command5);
                if (HuaniCommand.CheckIfResponsIsValid(response5, "tipping_heater_data_1"))
                {
                    var devicepara = M5ShiftDataParser.DeviceParameterData5(response5);
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = $"{config.MachineName}.{nameof(devicepara.Temperature5)}", Value = devicepara.Temperature5, IsGood = true });
                }
                await Task.Delay(ControllerConst.DefaultHauniIntervel);
                /// 设备参数
                for (int i = 0; i < functions.Count; i++)
                {
                    var nf = functions[i].Split(",");
                    var command3 = HuaniCommand.PackCommand(Convert.ToByte(nf[0]), Convert.ToByte(nf[1]), VisuCommandNameEnum.ReadMachineParameter, ProtocolTypeEnum.M5, numberParam: 0).Item1;
                    var response3 = await _chanle.SendCommandAsync(command3);

                    if (HuaniCommand.CheckIfResponsIsValid(response3, nameof(VisuCommandNameEnum.ReadMachineParameter)))
                    {
                       var ReadMachineParameter = HuaniCommand.PasreDevicePvalue(response3, ProtocolTypeEnum.M5);
                        if (ReadMachineParameter != null && ReadMachineParameter?.Count > 0)
                        {
                            foreach (var item in ReadMachineParameter)
                            {
                                if (item.ParameterValue == null)
                                    continue;
                                realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item.ParameterValue, Code = $"{config.MachineName}.Machine.{item.ParaNumerName}" });
                            }
                        }
                    }

                    await Task.Delay(20);
                }

                flag = true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in M5RealTimeParameterAct ocuur exception:{ex}, trace:{ex?.StackTrace}");
            }

            if (flag)
            {
                if (realTimeDataDto.Details.Count > 0)
                {
                    var res = await _mqttController.ConnectAsync();
                    res = await _mqttController.PublishMessageAsync($"{MqttConst.MqttTopicRealTime}/{config.MachineName}", JsonConvert.SerializeObject(realTimeDataDto), true);
                }
            }
            return true;
        }
    }
}
