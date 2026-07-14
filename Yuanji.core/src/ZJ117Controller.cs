using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;
using YS.Yuanji.Commom.helper;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;
using  Yuanji.core.src.Driver.Huani.Controller;
using  Yuanji.core.src.Driver.Huani.Entity;
using  Yuanji.core.src.Driver.ZB48A;

namespace Yuanji.core.src
{
    public  class ZJ117Controller:DataCollectContoller
    {
        public ZJ117Controller(MqttController mqttController) : base(mqttController)
        {
        }
        private async Task ZJ117DataCollectAction()
        {
            await ZJ117DataCollectAct();
        }

        private async Task<bool> ZJ117DataCollectAct()
        {
            var tt = DateTime.Now;
            bool flag = false;
            CigaretteProductDto productDto = null;
            CigratteRejectDto rejectDto = null;
            CigratteStopDataDto stopDataDto = null;
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = config.MachineName;
            var command = new byte[4];
            var resp =await _chanle.SendCommandAsync(command);
            var res = HuaniCommand.GetZJ17ProductionData(resp);
                if (res != null)
                {
                    try
                    {
                        productDto = new CigaretteProductDto();
                        productDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                        productDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                        productDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                        productDto.SWJDM = string.Empty;
                        productDto.XWJDM = config.MachineName;
                        productDto.NO = 1;
                        productDto.TOTALPRODUCTION = (int)res.Production * 1000;
                        productDto.TOTALWASTE = res.TotalWas;
                        productDto.TOTALWASTEPCT = (float)res.WasteRate;
                        productDto.PRODUCTIONSTARTTIME = res.PSTime;

                        productDto.EFFMACHINE = (float)res.Efficent;
                        productDto.EFFPRODUCTION = (float)res.UsageRate;
                        productDto.TOTALSTOPCNT = res.HaltNum;
                        productDto.TOTALSTOPTIME = VisuHelper.HHmmssToTotalSecondsZJ17(res.HaltTime);
                        productDto.RUNTIME = VisuHelper.HHmmssToTotalSecondsZJ17(res.TotalRunTime);

                        productDto.MACHINESPEED = (int)res.CurrentSpeed;

                        productDto.batch = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                        productDto.FILTERTIP = res.MaxPaper;
                        productDto.PAPER = res.SEPaperLen1;
                        productDto.WPAPER = res.MaxPaperLen1;

                        productDto.PAPERPAN = res.Filter; //嘴耗
                        productDto.WPAPERPAN = res.MaxPaper;
                        productDto.GATHERTYPE = "A";

                    #region 剔除数据
                    rejectDto = new CigratteRejectDto();
                        rejectDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                        rejectDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                        rejectDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                        rejectDto.PM_MP_MACHINEPART_ID = "0";
                        rejectDto.XWJDM = config.MachineName;


                        rejectDto.REJECTS = new List<RejectInfo>();
                        RejectInfo reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "烟支太轻", REJECTVALUE = res.TooLight };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "烟支太重", REJECTVALUE = res.TooHeavy };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "软点", REJECTVALUE = res.SoftSpot };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "硬点", REJECTVALUE = res.HardSpot };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "轻烟端", REJECTVALUE = res.LightEnd };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "漏气", REJECTVALUE = res.Airleakage };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "松头", REJECTVALUE = res.LooseEnd };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "缺少滤嘴", REJECTVALUE = res.MissFilter };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "OTIS剔除", REJECTVALUE = res.OTIS };
                        rejectDto.REJECTS.Add(reject);
                        rejectDto.GATHERTYPE = "A";
                    #endregion
                    #region 停止数据
                    stopDataDto = new CigratteStopDataDto();
                        stopDataDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                        stopDataDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                        stopDataDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                        stopDataDto.PLANWORKORDERNO = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                        stopDataDto.PM_MP_MACHINEPART_ID = "0";
                        stopDataDto.XWJDM = config.MachineName;
                        stopDataDto.NO = 1;
                        stopDataDto.GATHERTYPE = "A";
                        stopDataDto.STOPS = new List<MachineGd110DetailModel>();
                        foreach (var item in res.StopReasonInfos)
                        {
                            if (string.IsNullOrEmpty(item?.StopReason))
                            {
                                continue;
                            }
                            MachineGd110DetailModel model = new MachineGd110DetailModel();
                            model.STOPTIME = VisuHelper.HHmmssToTotalSeconds(item.StopTime);
                            model.STOPCNT = item.StopCount;
                            model.PM_MP_STOPCODE_ID = item.StopReason;
                            stopDataDto.STOPS.Add(model);
                        }

                        var fields = typeof(ZJ17ProductionData).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            if (field.Name == nameof(ZJ17ProductionData.StopReasonInfos))
                                continue;

                            var fieldValue = field.GetValue(res); // 当前值
                            if (fieldValue == null)
                                continue;

                            var fieldName = $"{config.MachineName}.{field.Name}"; // 字段名

                            if (fieldValue != null)
                                realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                        }
#endregion

                        flag = true;
                    }
                    catch (Exception ex)
                    {
                        flag = false;
                        LogController.Instance.Error($"in ZJ117DataCollectAct occur exception:{ex}");
                    }

                }
                else
                    flag = false;

            if (flag)
            {
                var mqttFlag = await _mqttController.ConnectAsync();
                mqttFlag = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicProductData}", JsonConvert.SerializeObject(productDto));
                mqttFlag = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicReject}", JsonConvert.SerializeObject(rejectDto));
                mqttFlag = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicStopData}", JsonConvert.SerializeObject(stopDataDto));

                if (realTimeDataDto.Details.Count > 0)
                    mqttFlag = await _mqttController.PublishMessageAsync($"{MqttConst.MqttTopicRealTime}/{config.MachineName}", JsonConvert.SerializeObject(realTimeDataDto), false);
            }
            return flag;
        }
    }
}
