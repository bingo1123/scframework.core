using Newtonsoft.Json;
using System.Globalization;
using System.Reflection;
using YS.Yuanji.Commom;
using YS.Yuanji.Commom.helper;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;
using Yuanji.core.src.Driver.Huani.Controller;
using Yuanji.core.src.Driver.ZB48A;

namespace Yuanji.core.src
{
    public  class ZB48AController: DataCollectContoller
    {
        public ApiClient _apiClient;
        public ApiClientConfig _apiClientConfig;
        public ZB48AController(MqttController mqttController) : base(mqttController)
        {
        }

        //ZB48AMqttCaller
        private async Task ZB48AMqttCaller()//TODO 添加一些剩余的实时数据
        {
        }


        private async Task ZB48ACollectAction()
        {
            await ZB48AProductAndStopDataCollectAct();
            await Task.Delay(ControllerConst.DefaultBZIntervel);
            await ZB48ARejectDataCollectAct();
            await Task.Delay(ControllerConst.DefaultBZIntervel);
            await ZB48ACollectRealTimeDataAct();
        }

        private string GetZB48APLCNameforYB(string code)
        {
            if (code.Contains("750"))
                return "YB518";

            else if (code.Contains("700"))
                return "YB418";

            else if (code.Contains("778"))
                return "YB618";

            else
                return code;
        }

        private async Task<bool> ZB48AProductAndStopDataCollectAct()
        {
            bool flag = false;
            var tt = DateTime.Now;
            PackMachineProductDataDto[] productDto = new PackMachineProductDataDto[3];
            PackMachineStopDataDto[] stopDataDtos = new PackMachineStopDataDto[3];
            PackMachineStopInfoDto[] stopInfoDtos = new PackMachineStopInfoDto[3];

            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = _packMachineName;


            try
            {
                bool getDataFlag = true;
                var currentShiftRunningStatus = await _apiClient.GetAsync<MachinePerformanceResponse>(ApiClient.CurrentShiftRunningStatusEndpoint, new Dictionary<string, string>());
                await Task.Delay(ControllerConst.DefaultBZIntervelPro);
                var productionData = await _apiClient.GetAsync<ProductionDataResponse>(ApiClient.ProductionDataEndpoint, new Dictionary<string, string>());
                await Task.Delay(ControllerConst.DefaultBZIntervelPro);
                var haltAnalysis = await _apiClient.GetAsync<HaltAnalysisResponse>(ApiClient.HaltAnalysisEndpoint, new Dictionary<string, string>());
                await Task.Delay(ControllerConst.DefaultBZIntervelPro);
                var speedData = await _apiClient.GetAsync<SpeedDataResponseAn>(ApiClient.SpeedEndpoint, new Dictionary<string, string>(), 8189);
                await Task.Delay(ControllerConst.DefaultBZIntervelPro);
                var machineRes = await _apiClient.GetAsync<MachineStatusResponse>(ApiClient.CurrentMachineStatusEndpoint, new Dictionary<string, string>());
                await Task.Delay(ControllerConst.DefaultBZIntervelPro);
                var stopAnaly = await _apiClient.GetAsync<HaltEquipment>(ApiClient.HaltEquipmentEndpoint, new Dictionary<string, string>());
                await Task.Delay(ControllerConst.DefaultBZIntervelPro);
                var processConsumption = await _apiClient.GetAsync<ProcessConsumptionResponse>(ApiClient.ShiftCountEndpoint, new Dictionary<string, string>());
                await Task.Delay(ControllerConst.DefaultBZIntervelPro);
                var energyConsumptio = await _apiClient.GetAsync<EnergyConsumptionResponse>(ApiClient.HourConsumptionEndpoint, new Dictionary<string, string>() { { "equipmentCode", _packMachineName.Contains("203")?"nqe718p2js": "jaqk6w3p1o" } }, 8189);//3号机Code

                #region 实时数据
                foreach (var item in currentShiftRunningStatus?.Data)
                {
                    var fields = typeof(MachinePerformanceData).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        var fieldValue = field.GetValue(item); // 当前值
                        if (fieldValue == null)
                            continue;

                        var fieldName = $"{_packMachineName}.{item.plcCode}.{field.Name.ToUpper()}"; // 字段名
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                    }
                }

                foreach (var item in speedData?.SpeedMap)
                {
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = $"{_packMachineName}.{GetZB48APLCNameforYB(item.Key)}.SPEED", Value = item.Value, IsGood = true });
                }

                foreach (var item in productionData?.Data)
                {
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = $"{_packMachineName}.Standard.{item.Key.ToUpper()}", Value = item.Value.Count, IsGood = true });

                }

                foreach (var item in machineRes?.Data)
                {
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = $"{_packMachineName}.{GetZB48APLCNameforYB(item.PlcCode)}.MACHINESTATUS", Value = item.status, IsGood = true });

                }

                foreach (var item in processConsumption?.Data)
                {
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = $"{_packMachineName}.Senior.{item.Key.ToUpper()}", Value = item.Value.Count, IsGood = true });
                }
                try
                {
                    energyConsumptio?.Data.Where(x => x.IsToday == true).ToList().GroupBy(Data => Data.SampleCode).ToList().ForEach(x =>
                    {
                        var item  = x.LastOrDefault();
                        if (item == null)
                            return;
                        var fields = typeof(EnergyConsumptionData).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            var fieldValue = field.GetValue(item); // 当前值
                            if (fieldValue == null)
                                continue;

                            var fieldName = $"{_packMachineName}.{GetZB48APLCNameforYB(item.SampleCode)}.{field.Name.ToUpper()}"; // 字段名
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                        }
                    });
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"in ZB48AProductDataCollectAct occur exception:{ex}");
                }

                getDataFlag = true;

                #endregion

                for (int i = 0; i < currentShiftRunningStatus?.Data?.Count; i++)
                {
                    productDto[i] = new PackMachineProductDataDto();
                    productDto[i].PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                    productDto[i].PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    productDto[i].PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    productDto[i].SWJDM = string.Empty;
                    productDto[i].XWJDM = _packMachineName;
                    productDto[i].NO = 1;
                    productDto[i].GATHERTYPE = "A";

                    productDto[i].PM_MP_MACHINEPART_ID = currentShiftRunningStatus.Data[i].plcCode;

                    productDto[i].PLANWORKORDERNO = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;

                    productDto[i].STOPCNT = currentShiftRunningStatus.Data[i].stopCount;

                    productDto[i].RUNTIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].runningTime);//运行累计时间

                    productDto[i].PREPARATIONTIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].presettime);

                    productDto[i].EXTERNALSTOPTIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].outStopTime);

                    productDto[i].INTERNALSTOPTIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].insideStopTime);


                    productDto[i].PRODUCTIONSTARTTIME = haltAnalysis.Data.Any(x => x.plcName.Equals(productDto[i].PM_MP_MACHINEPART_ID, StringComparison.OrdinalIgnoreCase)) ?
                haltAnalysis.Data.First(x => x.plcName.Equals(productDto[i].PM_MP_MACHINEPART_ID, StringComparison.OrdinalIgnoreCase)).shiftStartTime?.ToString(MqttConst.DateTimeFormat) : null;

                    productDto[i].POWEROFFTIME = 0;

                    productDto[i].PRODUCETIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].maxRunningTime);//生产时间

                    productDto[i].WAITINGMATERIALTIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].waitTime);//

                    productDto[i].MAXRUNTIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].maxRunningTime);

                    productDto[i].AVGRUNTIME = VisuHelper.HHmmssToTotalSeconds(currentShiftRunningStatus.Data[i].avgRunningTime);

                    productDto[i].THEORETICALPRODUCTION = currentShiftRunningStatus.Data[i].theoreticalYield;


                    productDto[i].EFFPRODUCTION = currentShiftRunningStatus.Data[i].efficiency;

                    productDto[i].EFFMACHINE = currentShiftRunningStatus.Data[i].overallEfficiency;

                    //700-YB418，750-YB518，778-YB618 , 又因为包装机的顺序为小包、小透、条包,
                    if (productDto[i].PM_MP_MACHINEPART_ID == ApiClient.YB418Name)
                    {

                        productDto[i].MACHINESPEED = speedData.SpeedMap.ContainsKey("700") ? speedData.SpeedMap["700"] : 0;
                        productDto[i].NORMALSPEED = speedData.SpeedMap.ContainsKey("700") ? speedData.SpeedMap["700"] : 0;

                        productDto[i].REALPRODUCTION = productionData.Data.ContainsKey(ApiClient.xx_exit_pdt) ? (int?)productionData.Data[ApiClient.xx_exit_pdt].Count : 0;//实际产量

                        productDto[i].REJECTPRODUCTION = productionData.Data.ContainsKey(ApiClient.xx_total_reject) ? (int?)productionData.Data[ApiClient.xx_total_reject].Count : 0;
                    }

                    else if (productDto[i].PM_MP_MACHINEPART_ID == ApiClient.YB518Name)
                    {
                        productDto[i].MACHINESPEED = speedData.SpeedMap.ContainsKey("750") ? speedData.SpeedMap["750"] : 0;
                        productDto[i].NORMALSPEED = speedData.SpeedMap.ContainsKey("750") ? speedData.SpeedMap["750"] : 0;

                        productDto[i].REALPRODUCTION = productionData.Data.ContainsKey(ApiClient.ch_exit_pdt) ? (int?)productionData.Data[ApiClient.ch_exit_pdt].Count : 0;//实际产量

                        productDto[i].REJECTPRODUCTION = productionData.Data.ContainsKey(ApiClient.ch_reject) ? (int?)productionData.Data[ApiClient.ch_reject].Count : 0;//实际产量
                    }

                    else if (productDto[i].PM_MP_MACHINEPART_ID == ApiClient.YB618Name)
                    {
                        productDto[i].MACHINESPEED = speedData.SpeedMap.ContainsKey("778") ? speedData.SpeedMap["778"] : 0;
                        productDto[i].NORMALSPEED = speedData.SpeedMap.ContainsKey("778") ? speedData.SpeedMap["778"] : 0;
                        productDto[i].REALPRODUCTION = productionData.Data.ContainsKey(ApiClient.ct_pdt) ? (int?)productionData.Data[ApiClient.ct_pdt].Count : 0;//实际产量

                        productDto[i].REJECTPRODUCTION = productionData.Data.ContainsKey(ApiClient.ct_reject) ? (int?)productionData.Data[ApiClient.ct_reject].Count : 0;//实际产量
                    }

                    productDto[i].BARPRODUCTION = productionData.Data.ContainsKey(nameof(ApiClient.ct_pdt)) ? (int?)productionData.Data[nameof(ApiClient.ct_pdt)].Count : 0;//

                    var nn = productionData.Data.ContainsKey(nameof(ApiClient.ct_label_paper_total_expend)) ? (int?)productionData.Data[nameof(ApiClient.ct_label_paper_total_expend)].Count : 0;//
                    productDto[i].LABELPAPER = productionData.Data.ContainsKey(nameof( ApiClient.xx_label_paper_total_expend)) ? (int?)productionData.Data[nameof(ApiClient.xx_label_paper_total_expend)].Count!=0
                        ? (int?)productionData.Data[nameof(ApiClient.xx_label_paper_total_expend)].Count: nn *10 : nn * 10;//商标纸消耗量

                    
                }
                #region 停机数据
                for (int i = 0; i < 3; i++)
                {
                    stopDataDtos[i] = new PackMachineStopDataDto();
                    stopDataDtos[i].PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                    stopDataDtos[i].PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    stopDataDtos[i].PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    stopDataDtos[i].PM_MP_MACHINEPART_ID = ApiClient.PartNames[i];
                    stopDataDtos[i].SWJDM = string.Empty;
                    stopDataDtos[i].XWJDM = _packMachineName;
                    stopDataDtos[i].NO = 1;
                    stopDataDtos[i].GATHERTYPE = "A";
                    stopDataDtos[i].STOPS = new List<MachineGd110DetailModel>();
                    var result = haltAnalysis.Data.Where(x => x.plcName == stopDataDtos[i].PM_MP_MACHINEPART_ID).ToList();
                    if (result != null && result?.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            MachineGd110DetailModel machineGd110 = new MachineGd110DetailModel();
                            machineGd110.PM_MP_STOPCODE_ID = item.LabelChinese;
                            machineGd110.STOPTIME = VisuHelper.HHmmssToTotalSeconds(item.totalTime);
                            machineGd110.STOPCNT = item.count;
                            stopDataDtos[i].STOPS.Add(machineGd110);
                        }
                    }
                }
                #endregion

                #region 停机流水
                for (int i = 0; i < 3; i++)
                {
                    stopInfoDtos[i] = new PackMachineStopInfoDto();
                    stopInfoDtos[i].PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                    stopInfoDtos[i].PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    stopInfoDtos[i].PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    stopInfoDtos[i].PM_MP_MACHINEPART_ID = ApiClient.PartNames[i];
                    stopInfoDtos[i].SWJDM = string.Empty;
                    stopInfoDtos[i].XWJDM = _packMachineName;
                    stopInfoDtos[i].NO = 1;
                    stopInfoDtos[i].GATHERTYPE = "A";
                    stopInfoDtos[i].STOPS = new List<GdStopInfo>();
                    var result = stopAnaly.Data.Where(x => x.plcName == stopInfoDtos[i].PM_MP_MACHINEPART_ID).ToList();
                    if (result != null && result?.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            GdStopInfo gdStopInfo = new GdStopInfo();
                            gdStopInfo.PM_MP_STOPCODE_ID = item.pauseReason;
                            gdStopInfo.STOPENDTIME = item.endTime;
                            gdStopInfo.STOPSTARTTIME = item.startTime;
                            try
                            {
                                gdStopInfo.STOPTIME = (int)(DateTime.Parse(item.endTime, CultureInfo.InvariantCulture) -
                               DateTime.Parse(item.startTime, CultureInfo.InvariantCulture)).TotalSeconds; ;
                            }
                            catch (Exception)
                            {
                                gdStopInfo.STOPTIME = 100; ;
                            }
                           
                            stopInfoDtos[i].STOPS.Add(gdStopInfo);
                        }
                    }
                }
                #endregion


                flag = true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in ZB48AProductDataCollectAct occur exception:{ex}");
                flag = false;
            }

            if (flag)
            {
                try
                {
                    var res = await _mqttController.ConnectAsync();
                    if (res)
                    {
                        foreach (var item in productDto)
                        {
                            if (item != null)
                                res = await _mqttController.PublishMessageAsync($"{_packMachineName}/{MqttConst.MqttTopicProductData}", JsonConvert.SerializeObject(item), false, AtMostOnce: true);
                        }
                        foreach (var item in stopDataDtos)
                        {
                            if (item != null && item?.STOPS?.Count > 0)
                                res = await _mqttController.PublishMessageAsync($"{_packMachineName}/{MqttConst.MqttTopicStopData}", JsonConvert.SerializeObject(item), false, AtMostOnce: true);
                        }
                        foreach (var item in stopInfoDtos)
                        {
                            if (item != null && item?.STOPS?.Count > 0)
                                res = await _mqttController.PublishMessageAsync($"{_packMachineName}/{MqttConst.MqttTopicStopInfo}", JsonConvert.SerializeObject(item), false, AtMostOnce: true);
                        }

                        if (realTimeDataDto.Details.Count > 0)
                            res = await _mqttController.PublishMessageAsync($"{MqttConst.MqttTopicRealTime}/{_packMachineName}", JsonConvert.SerializeObject(realTimeDataDto), false,AtMostOnce:true);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return flag;
        }

        private async Task<bool> ZB48ARejectDataCollectAct()
        {
            bool flag = false;
            var tt = DateTime.Now;

            PackMachineRejectDto[] rejectDtos = new PackMachineRejectDto[3];

            try
            {
                bool getDataFlag = true;
                var rejectData = await _apiClient.GetAsync<RejectDataResponse>(ApiClient.RejectShiftCountsEndpoint, new Dictionary<string, string>());

                for (int i = 0; i < ApiClient.PartNames.Length; i++)
                {
                    rejectDtos[i] = new PackMachineRejectDto();
                    rejectDtos[i].PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                    rejectDtos[i].PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    rejectDtos[i].PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    rejectDtos[i].PM_MP_MACHINEPART_ID = ApiClient.PartNames[i];
                    rejectDtos[i].SWJDM = string.Empty;
                    rejectDtos[i].XWJDM = _packMachineName;
                    rejectDtos[i].NO = 1;
                    rejectDtos[i].GATHERTYPE = "A";
                    rejectDtos[i].REJECTS = new List<PackRejectInfo>();
                    var result = rejectData.Data.Where(x => x.PlcCode == rejectDtos[i].PM_MP_MACHINEPART_ID && x.Value > 0).ToList();
                    if (result != null && result?.Count > 0)
                    {
                        foreach (var item in result)
                        {
                            PackRejectInfo rejectInfo = new PackRejectInfo();
                            rejectInfo.PM_MP_REJECTCODE_ID = item.CounterName;
                            rejectInfo.REJECTVALUE = item.Value.ToString();
                            rejectDtos[i].REJECTS.Add(rejectInfo);
                        }
                    }
                }

                flag = true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in ZB48ARejectDataCollectAct occur exception:{ex}");
                flag = false;
            }
            if (flag)
            {
                try
                {
                    var res = await _mqttController.ConnectAsync();
                    if (res)
                    {
                        foreach (var item in rejectDtos)
                        {
                            if (item != null && item?.REJECTS?.Count > 0)
                                res = await _mqttController.PublishMessageAsync($"{_packMachineName}/{MqttConst.MqttTopicReject}", JsonConvert.SerializeObject(item));
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return flag;
        }

        private async  Task<bool> ZB48ACollectRealTimeDataAct()
        {
            bool flag = false;
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = _packMachineName;

            try
            {
                var parameter = await _apiClient.GetAsync(ApiClient.ParameterEquipmentEndpoint, new Dictionary<string, string>());
                var parameterResponse = JsonConvert.DeserializeObject<DeviceParameterResponse>(parameter);
                var res =parameterResponse.Data.Distinct().ToList();
                int i = 0;
                res.ForEach(x =>
                {
                   var fieldName = $"{_packMachineName}.{x.PlcCode}.P{i++.ToString("D4")}"; // 字段名
                   realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = x.ValCurrent, IsGood = true });
                
                });
                flag = true;

            }
            catch (Exception)
            {

            }

            if (flag)
            {
                try
                {
                    var res = await _mqttController.ConnectAsync();
                    if (res)
                    {
                        if (realTimeDataDto.Details.Count > 0)
                            res = await _mqttController.PublishMessageAsync($"{MqttConst.MqttTopicRealTime}/{_packMachineName}", JsonConvert.SerializeObject(realTimeDataDto), false,AtMostOnce:true);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return flag;
        }



        /// <summary>
        /// 读取ZB48A数据
        /// </summary>
        /// <param name="prop"></param>
        private async Task ReadZB48AData(string prop)
        {
            try
            {
                var performanceRes = await _apiClient.GetAsync(ApiClient.CurrentMachineStatusEndpoint, new Dictionary<string, string>());
                var res = JsonConvert.DeserializeObject<MachineStatusResponse>(performanceRes);

                var performanceRes1 = await _apiClient.GetAsync(ApiClient.CurrentShiftRunningStatusEndpoint, new Dictionary<string, string>());
                var res1 = JsonConvert.DeserializeObject<MachinePerformanceResponse>(performanceRes1);

                var performanceRes2 = await _apiClient.GetAsync(ApiClient.HaltEquipmentEndpoint, new Dictionary<string, string>());
                var res2 = JsonConvert.DeserializeObject<DeviceFaultResponse>(performanceRes2);

                var performanceRes3 = await _apiClient.GetAsync(ApiClient.ParameterEquipmentEndpoint, new Dictionary<string, string>());
                var res3 = JsonConvert.DeserializeObject<DeviceParameterResponse>(performanceRes3);


                var performanceRes4 = await _apiClient.GetAsync(ApiClient.HaltAnalysisEndpoint, new Dictionary<string, string>());
                var res4 = JsonConvert.DeserializeObject<HaltAnalysisResponse>(performanceRes4);

                //var performanceRes = await _apiClient.GetAsync(ApiClient.HourConsumptionEndpoint, new Dictionary<string, string>() { { "equipmentCode", "jaqk6w3p1o" } }, 8189);//4号机Code
                var performanceRes5 = await _apiClient.GetAsync(ApiClient.HourConsumptionEndpoint, new Dictionary<string, string>() { { "equipmentCode", "nqe718p2js" } }, 8189);//3号机Code
                EnergyConsumptionResponse res5 = null;
                try
                {
                    res5 = JsonConvert.DeserializeObject<EnergyConsumptionResponse>(performanceRes5);
                }
                catch (Exception ex)
                {
                    LogController.Instance.Log($"ZB48A Test {ApiClient.HourConsumptionEndpoint} ,response :{performanceRes5},exception:{ex}");
                }

                var performanceRes6 = await _apiClient.GetAsync(ApiClient.malfunctionEquipmentEndpoint, new Dictionary<string, string>());
                var res6 = JsonConvert.DeserializeObject<MalfunctionDataResponse>(performanceRes6);

                var parameters = new Dictionary<string, string>
                                {
                                    { "pageNum", 0.ToString() },
                                    { "pageSize", 10.ToString() },
                                    { "orderBy", "id" },
                                    { "direction", "DESC" }
                                };
                var performanceRes7 = await _apiClient.PostAsync(ApiClient.ModifyRecordEndpoint, parameters);
                var res7 = JsonConvert.DeserializeObject<ParameterModificationResponse>(performanceRes7);

                var performanceRes8 = await _apiClient.GetAsync(ApiClient.ShiftCountEndpoint, new Dictionary<string, string>());
                var res8 = JsonConvert.DeserializeObject<ProcessConsumptionResponse>(performanceRes8);
                if (res8 != null && res8?.Data != null && res8?.Data.Count > 0)
                {
                    var flag = res8.Data.ContainsKey(nameof(ApiClient.xx_label_paper_total_expend));
                    if (flag)
                    {
                        var count = res8.Data[nameof(ApiClient.xx_label_paper_total_expend)].Count;
                    }
                    flag = res8.Data.ContainsKey(nameof(ApiClient.ct_label_paper_total_expend));
                    if (flag)
                    {
                        var count = res8.Data[nameof(ApiClient.ct_label_paper_total_expend)].Count;

                    }
                }

                var performanceRes9 = await _apiClient.GetAsync(ApiClient.ProductionDataEndpoint, new Dictionary<string, string>());


                var performanceRes10 = await _apiClient.GetAsync(ApiClient.RejectShiftCountsEndpoint, new Dictionary<string, string>());
                var res10 = JsonConvert.DeserializeObject<RejectDataResponse>(performanceRes10);

                string str = "{\"sendTime\":1736750109000,\"speedMap\":{\"750\":0,\"700\":0,\"778\":0},\"equipmentCode\":\"nqe718p2js\"}";
                var info = JsonConvert.DeserializeObject<SpeedDataResponseAn>(str);
                var performanceRes11 = await _apiClient.GetAsync(ApiClient.SpeedEndpoint, new Dictionary<string, string>(), 8189);
                SpeedDataResponseAn res11 = null;
                try
                {
                    res11 = JsonConvert.DeserializeObject<SpeedDataResponseAn>(performanceRes11);
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"ZB48A Test {ApiClient.SpeedEndpoint} ,response :{performanceRes11},exception:{ex}");
                }

            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"ZB48A Test,exception:{ex}");

            }
        }
    }
}

