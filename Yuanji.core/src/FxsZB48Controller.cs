using Newtonsoft.Json;
using Yuanji.core.src.Driver.Huani.Controller;
using YS.Yuanji.Log;
using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using YS.Yuanji.Commom.helper;

namespace Yuanji.core.src
{
    public class FxsZB48Controller : DataCollectContoller
    {
        public FockeTypeEnum FockeType { get; set; }
        public FxsZB48Controller(MqttController mqttController) : base(mqttController)
        {
        }

        public override Task Initialization()
        {
            FockeType = Enum.TryParse<FockeTypeEnum>(DeviceConfig.MachinePart, out var _ftype) ? _ftype : FockeTypeEnum.None;
            return base.Initialization();
        }

        private int StopCount;

        private string GetRealTimePartName(FockeTypeEnum type)
        {
            return type.ToString().Replace("M", "D");
        }

        protected async override Task OthersAsync(List<YS.Yuanji.Commom.Item> items)
        {

        }

        protected override async Task RealtimeAsync(List<YS.Yuanji.Commom.Item> items)
        {
            //var type = this.FockeType;
            //await FxsZB48PresetRealTimeDataCollectAct(type);
        }

        protected async override Task ProductAsync(List<YS.Yuanji.Commom.Item> items)
        {
            var type = this.FockeType;
            await FxsZB48DataCollectAct(type);
            await Task.Delay(ControllerConst.DefaultBZIntervel);
            await FxsZB48RejectDataCollectAct(type);
            await Task.Delay(ControllerConst.DefaultBZIntervel);
            await FxsZB48PresetRealTimeDataCollectAct(type);
            await Task.Delay(ControllerConst.DefaultBZIntervel);
            await FxsZB48StopDataCollectAct(type);
        }

        private async Task<bool> FxsZB48DataCollectAct(FockeTypeEnum type)
        {
            bool flag = false;
            var tt = DateTime.Now;
            PackMachineProductDataDto productDto = new PackMachineProductDataDto();
            PackMachineStopInfoDto stopInfoDto = new PackMachineStopInfoDto();
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.MachineId = MachineCode;
            realTimeDataDto.Details = new List<DataItemDetailDto>();

            byte[] command = null;
            byte[] response = null;
            byte[] effCommand = null;
            byte[] EffResponse = null;
            byte[] Tommand = null;
            byte[] TimeResponse = null;

            byte[] speedCommand = null;
            byte[] speedResponse = null;

            byte[] speedTimeCommand = null;
            byte[] speedTimeResponse = null;

            byte[] basicInfoCommand = null;
            byte[] basicInfoResponse = null;

            try
            {
                command = HuaniCommand.PackFxsZB48ProductionDataCmd(type);
                response = await _chanle.SendCommandAsync(command);
                await Task.Delay(ControllerConst.DefaultBZIntervel);
                effCommand = HuaniCommand.PackFxsZB48EfficiencyCmd(type);
                EffResponse = await _chanle.SendCommandAsync(effCommand);
                await Task.Delay(ControllerConst.DefaultBZIntervel);
                Tommand = HuaniCommand.PackFxsZB48TimesCmd(type);
                TimeResponse = await _chanle.SendCommandAsync(Tommand);
                await Task.Delay(ControllerConst.DefaultBZIntervel);
                speedCommand = HuaniCommand.PackFxsZB48SpeedCmd(type);
                speedResponse = await _chanle.SendCommandAsync(speedCommand);
                await Task.Delay(ControllerConst.DefaultBZIntervel);
                speedTimeCommand = HuaniCommand.PackFxsZB48SpeedTimeCostCmd(type);
                speedTimeResponse = await _chanle.SendCommandAsync(speedTimeCommand);
                await Task.Delay(ControllerConst.DefaultBZIntervel);
                basicInfoCommand = HuaniCommand.PackFxsZB48BasicInfoCmd(type);
                basicInfoResponse = await _chanle.SendCommandAsync(basicInfoCommand);

                string[] productRes = HuaniCommand.ParseFxsZB48Response(response, "Product");
                string[] efficiencyRes = HuaniCommand.ParseFxsZB48Response(EffResponse, "Eff");
                string[] timeRes = HuaniCommand.ParseFxsZB48Response(TimeResponse, "Time");
                string[] MaxAndCurrentSpeed = HuaniCommand.ParseFxsZB48Response(speedResponse, "speed");
                string[] SpeedTime = HuaniCommand.ParseFxsZB48Response(speedTimeResponse, "speedTime");
                string[] basicInfo = HuaniCommand.ParseFxsZB48Response(basicInfoResponse, "basicInfo");

                for (int i = 0; i < productRes.Length; i++)
                { 
                    var letter = (char)('A' + i);
                    if (i == 2)
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = VisuHelper.ToFxsZB48DateTime(productRes[i]), Code = $"{MachineCode}.{GetRealTimePartName(type)}.{letter}29" });
                    else if (i == 3)
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = productRes[i], Code = $"{MachineCode}.{GetRealTimePartName(type)}.{letter}29" });
                    else
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = productRes[i], Code = $"{MachineCode}.{GetRealTimePartName(type)}.{letter}29" });
                }

                for (int i = 0; i < efficiencyRes.Length; i++)
                {
                    if (i >= 9 && i <= efficiencyRes.Length - 2)
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = VisuHelper.ToFxsZB48HHmmss(efficiencyRes[i]), Code = $"{MachineCode}.{GetRealTimePartName(type)}.P{i + 2}" });
                    else
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = efficiencyRes[i], Code = $"{MachineCode}.{GetRealTimePartName(type)}.P{i + 2}" });
                }


                for (int i = 0; i < timeRes.Length; i++)
                {
                    if (i == timeRes.Length - 1)
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = VisuHelper.ToFxsZB48DateTime(timeRes[i]), Code = $"{MachineCode}.{GetRealTimePartName(type)}.A{i + 13}" });
                    else
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = VisuHelper.ToFxsZB48HHmmss(timeRes[i]), Code = $"{MachineCode}.{GetRealTimePartName(type)}.A{i + 13}" });
                }

                for (int i = 0; i < MaxAndCurrentSpeed.Length; i++)
                {
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = MaxAndCurrentSpeed[i], Code = $"{MachineCode}.{GetRealTimePartName(type)}.D{i + 22}" });
                    MachineStatus = i == 1 ? Convert.ToInt32(MaxAndCurrentSpeed[i]) > 0 ? MachineStatusEnum.Running : MachineStatusEnum.Stop : MachineStatus;
                }

                for (int i = 0; i < SpeedTime.Length; i++)
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = SpeedTime[i], Code = $"{MachineCode}.{GetRealTimePartName(type)}.D{i + 13}" });

                for (int i = 0; i < basicInfo.Length; i++)
                {
                    if (i == basicInfo.Length - 1)
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = VisuHelper.ToFxsZB48DateTime(basicInfo[i]), Code = $"{MachineCode}.{GetRealTimePartName(type)}.A{i + 1}" });

                    else if (i == basicInfo.Length - 2)
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = VisuHelper.ToFxsZB48DateTime(basicInfo[i]), Code = $"{MachineCode}.{GetRealTimePartName(type)}.A{i + 1}" });
                    else
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = basicInfo[i], Code = $"{MachineCode}.{GetRealTimePartName(type)}.A{i + 1}" });
                }
                try
                {
                    productDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                    productDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    productDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    productDto.SWJDM = string.Empty;
                    productDto.XWJDM = MachineCode;
                    productDto.NO = 1;
                    productDto.GATHERTYPE = "A";
                    productDto.PM_MP_MACHINEPART_ID = type.ToString();//部位
                    productDto.PLANWORKORDERNO = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;//生产计划工单号

                    productDto.PRODUCTIONSTARTTIME = VisuHelper.ToFxsZB48DateTime(productRes[2]);//开始生产时间

                    productDto.POWEROFFTIME = VisuHelper.ToFxsZB48HHmmss((efficiencyRes[13]));//关电源时间

                    productDto.PREPARATIONTIME = VisuHelper.ToFxsZB48HHmmss(timeRes[5]);

                    productDto.EXTERNALSTOPTIME = VisuHelper.ToFxsZB48HHmmss(timeRes[4]);
                    productDto.INTERNALSTOPTIME = VisuHelper.ToFxsZB48HHmmss(timeRes[3]);

                    productDto.RUNTIME = VisuHelper.ToFxsZB48HHmmss(timeRes[0]);//运行累计时间

                    productDto.PRODUCETIME = VisuHelper.ToFxsZB48HHmmss(timeRes[2]);//生产时间

                    productDto.WAITINGMATERIALTIME = VisuHelper.ToFxsZB48HHmmss(timeRes[4]);//
                    productDto.MAXRUNTIME = VisuHelper.ToFxsZB48HHmmss(SpeedTime[SpeedTime.Length - 1]);
                    productDto.AVGRUNTIME = VisuHelper.ToFxsZB48HHmmss(SpeedTime[SpeedTime.Length - 1]);

                    productDto.THEORETICALPRODUCTION = Convert.ToInt32(efficiencyRes[0]);
                    productDto.REALPRODUCTION = Convert.ToInt32(efficiencyRes[2]);//实际产量
                    productDto.REJECTPRODUCTION = Convert.ToInt32(efficiencyRes[3]);

                    productDto.EFFPRODUCTION = Convert.ToSingle(efficiencyRes[5]) * 100;
                    productDto.EFFMACHINE = Convert.ToSingle(efficiencyRes[6]) * 100;


                    productDto.MACHINESPEED = Convert.ToInt32(MaxAndCurrentSpeed[1]);
                    productDto.NORMALSPEED = Convert.ToInt32(MaxAndCurrentSpeed[0]);
                    productDto.STOPCNT = StopCount;
                    productDto.BARPRODUCTION = 0;
                    productDto.LABELPAPER = Convert.ToInt32(productRes[9]);
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"in FxsZB48DataCollectAct productDto occur exception:{ex}");
                }

                try
                {

                    #region 停机流水
                    stopInfoDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                    stopInfoDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    stopInfoDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    stopInfoDto.PM_MP_MACHINEPART_ID = type.ToString();
                    stopInfoDto.SWJDM = string.Empty;
                    stopInfoDto.XWJDM = MachineCode;
                    stopInfoDto.NO = 1;
                    stopInfoDto.GATHERTYPE = "A";
                    stopInfoDto.STOPS = new List<GdStopInfo>();

                    for (int i = 0; i < 10; i++)
                    {
                        //GdStopInfo gdStopInfo = new GdStopInfo();
                        //gdStopInfo.PM_MP_STOPCODE_ID = "FXS" + i.ToString("D3");
                        ////gdStopInfo.STOPENDTIME = item.endTime;
                        ////gdStopInfo.STOPSTARTTIME = item.startTime;
                        //try
                        //{
                        //    gdStopInfo.STOPTIME = VisuHelper.HHmmssToTotalSeconds(efficiencyRes[i + 13]);
                        //}
                        //catch (Exception)
                        //{
                        //    gdStopInfo.STOPTIME = 100; ;
                        //}

                        //stopInfoDto.STOPS.Add(gdStopInfo);
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"in FxsZB48DataCollectAct stopInfoDto occur exception:{ex}");
                }

                #endregion

                flag = true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in FxsZB48DataCollectAct occur exception:{ex}");
                flag = false;
            }

            if (flag)
            {
               await PublishDto($"{MachineCode}/{MqttConst.MqttTopicProductData}", productDto);

               await PublishRealtimeData(realTimeDataDto);
                
            }
            return true;
        }

        private async Task<bool> FxsZB48StopDataCollectAct(FockeTypeEnum type)
        {
            bool flag = false;
            var tt = DateTime.Now;
            PackMachineStopDataDto stopDataDto = new PackMachineStopDataDto();
            //RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            //realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //realTimeDataDto.MachineId = MachineCode;
            //realTimeDataDto.Details = new List<DataItemDetailDto>();

            byte[] command = null;
            byte[] response = null;

            try
            {
                var stop = GetStopAnaInfo(type, IsContaiName);
                if (string.IsNullOrEmpty(stop))
                {
                    return true;
                }
                command = HuaniCommand.PackFxsZB48Command(type,stop);
                response = await _chanle.SendCommandAsync(command);
                var values = HuaniCommand.ParseMutTripleDataFxsZB48Response(response, "Stop");
                var result = values;
                var  stoplist = result.Where(l => Convert.ToInt32(l.Last()) > 0);
                stopDataDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                stopDataDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                stopDataDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                stopDataDto.PM_MP_MACHINEPART_ID = type.ToString();
                stopDataDto.SWJDM = string.Empty;
                stopDataDto.XWJDM = MachineCode;
                stopDataDto.NO = 1;
                stopDataDto.GATHERTYPE = "A";
                stopDataDto.STOPS = new List<MachineGd110DetailModel>();
                foreach (var item in stoplist)
                {
                    MachineGd110DetailModel machineGd110 = new MachineGd110DetailModel();
                    var code = IsContaiName ? item[1].ToString() : item[0].ToString();
                    var uitem = item.TakeLast(2).ToArray();
                    machineGd110.PM_MP_STOPCODE_ID =$"{type.ToString()}.{code}";
                    machineGd110.STOPTIME = VisuHelper.HHmmssToTotalSeconds(uitem[0]);
                    machineGd110.STOPCNT = Convert.ToInt32(uitem[1]);
                    stopDataDto.STOPS.Add(machineGd110);

                    //realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = VisuHelper.ToFxsZB48Time(item.Item2), Code = $"{MachineCode}.{GetRealTimePartName(type)}.Warnings.{item.Item1}" });

                }
                flag = true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in FxsZB48StopDataCollectAct occur exception:{ex}");
            }

            if (flag)
            {
                await PublishDto($"{MachineCode}/{MqttConst.MqttTopicStopData}", stopDataDto);

                //if (realTimeDataDto.Details.Count > 0)
                //{
                //    LogDisPlayAction?.Invoke(realTimeDataDto);
                //    res = await _mqttController.PublishMessageAsync($"{MqttConst.MqttTopicRealTime}/{MachineCode}", JsonConvert.SerializeObject(realTimeDataDto), false, AtMostOnce: true);

                //}
            }
            return flag;
        }

        private async Task<bool> FxsZB48RejectDataCollectAct(FockeTypeEnum type)
        {
            bool flag = false;
            var tt = DateTime.Now;
            PackMachineRejectDto rejectDataDto = new PackMachineRejectDto();
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.MachineId = MachineCode;
            realTimeDataDto.Details = new List<DataItemDetailDto>();

            byte[] command = null;
            byte[] response = null;
            try
            {
                var reject = GetRejectInfo(type,IsContaiName);
                if (string.IsNullOrEmpty(reject))
                {
                    return true;
                }
                command = HuaniCommand.PackFxsZB48Command(type, reject);
                response = await _chanle.SendCommandAsync(command);
                var values = HuaniCommand.ParseMutTripleDataFxsZB48Response(response, "RejectInfo");
                var result = values;

                rejectDataDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                rejectDataDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                rejectDataDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                rejectDataDto.PM_MP_MACHINEPART_ID = type.ToString();
                rejectDataDto.SWJDM = string.Empty;
                rejectDataDto.XWJDM = MachineCode;
                rejectDataDto.NO = 1;
                rejectDataDto.GATHERTYPE = "A";
                rejectDataDto.REJECTS = new List<PackRejectInfo>();
                int nr = IsContaiName ? 1 : 0 ;
                foreach (var item in result)
                {
                    PackRejectInfo rejectInfo = new PackRejectInfo();
                    rejectInfo.PM_MP_REJECTCODE_ID = item[nr].ToString();
                    rejectInfo.REJECTVALUE = item[nr+2].ToString();
                    rejectDataDto.REJECTS.Add(rejectInfo);
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item[nr + 1], Code = $"{MachineCode}.{GetRealTimePartName(type)}.Reject.{item[nr]}",Name = IsContaiName ? item[0]:string.Empty});
                }
                flag = true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in FxsZB48RejectDataCollectAct occur exception:{ex}");
            }

            if (flag)
            {
                await PublishDto($"{MachineCode}/{MqttConst.MqttTopicReject}", rejectDataDto);
                await PublishRealtimeData(realTimeDataDto);
            }
            return flag;
        }

        private async Task<bool> FxsZB48PresetRealTimeDataCollectAct(FockeTypeEnum type)
        {
            bool flag = false;
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.MachineId = MachineCode;
            realTimeDataDto.Details = new List<DataItemDetailDto>();

            byte[] command = null;
            byte[] response = null;
            try
            {
                var preset = GetPresetInfo(type,IsContaiName);
                if (string.IsNullOrEmpty(preset))
                {
                    return true;
                }
                command = HuaniCommand.PackFxsZB48Command(type, preset);
                response = await _chanle.SendCommandAsync(command);

                var values = HuaniCommand.ParseMutTripleDataFxsZB48Response(response, "PresetInfo");
                var result = values;
                int nr = IsContaiName ? 1 : 0;
                foreach (var item in result)
                {
                    realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item[nr+2], Code = $"{MachineCode}.{GetRealTimePartName(type)}.Preset{item[nr]}" ,Name = IsContaiName ? item[0]:string.Empty});

                }
                await Task.Delay(100);

                flag = true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in FxsZB48PresetRealTimeDataCollectAct occur exception:{ex}");
            }
            if (flag)
            {
                await PublishRealtimeData(realTimeDataDto);
            }
            return flag;
        }

        private string GetPresetInfo(FockeTypeEnum type, bool name = false)
        {
            string Q = name ? "P" : "Q";
            return type switch
            {
                FockeTypeEnum.M700 => $"{Q}216-S233",
                FockeTypeEnum.M703 => $"{Q}216-S231",
                FockeTypeEnum.M750 => $"{Q}216-S335",
                FockeTypeEnum.M753 => $"{Q}216-S390",
                FockeTypeEnum.M778 => $"{Q}216-S283",
                FockeTypeEnum.M779 => $"{Q}216-S345",
                FockeTypeEnum.M404 => $"{Q}216-S305",
                FockeTypeEnum.M502 => $"{Q}216-S305",
                //FockeTypeEnum.M542 => "Q216-S305",
                //FockeTypeEnum.M556 => "Q216-S305",
                //FockeTypeEnum.M993 => "Q216-S305",
                _ => string.Empty
            };
        }

        private string GetRejectInfo(FockeTypeEnum type,bool name = false)
        {
            string M = name ? "L" : "M";
            return type switch
            {
                FockeTypeEnum.M700 =>$"{M}41-O300",
                FockeTypeEnum.M703 =>$"{M}41-O300",
                FockeTypeEnum.M750 =>$"{M}41-O300",
                FockeTypeEnum.M753 =>$"{M}41-O300",
                FockeTypeEnum.M778 =>$"{M}41-O300",
                FockeTypeEnum.M779 =>$"{M}41-O300",
                FockeTypeEnum.M404 =>$"{M}41-O98",
                FockeTypeEnum.M502 =>$"{M}41-O73",
                FockeTypeEnum.M556 =>$"{M}41-O136",
                FockeTypeEnum.M993 =>$"{M}41-O136",
                _ => string.Empty
            };
        }

        private string GetStopAnaInfo(FockeTypeEnum type, bool name = false)
        {
            string M = type == FockeTypeEnum.M700 ?  name ? "A" : "B": name ? "AL" : "AM";
            return type switch
            {
                FockeTypeEnum.M700 => $"{M}41-D440",
                FockeTypeEnum.M703 => $"{M}41-AP48",
                FockeTypeEnum.M404 => $"{M}41-AP48",
                _ => string.Empty
            };
        }

    }
}
