using MQTTnet.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Log;
using YS.Yuanji.Commom;
using Yuanji.core.src.Driver.Huani.Controller;
using Yuanji.core.src.Driver.Huani.Entity;
using YS.Yuanji.Drive;
using YS.Yuanji.Commom.helper;

namespace Yuanji.core.src
{

    #region 读取数据，ReadData,首先先去看协议，理解了协议后再进行处理
    #region 发送的命令
    //发送命令的第一个数据：占两个字节，含义帧长度 2byte
    //发送命令的第二个数据：占一个字节，帧类型 1byte
    //发送命令的第三个数据：3-11总共9个字节，含义：符号地址，9byte
    //发送命令的第三个数据的中具体每个数据的含义：
    //第一个数据：占一个字节，含义：功能码，1byte
    //第二个数据：占四个字节，含义：符号地址名称 4byte
    //第三个数据：占两个字节，含义：数据从符号地址的那个偏移未知开始读，2byte
    //第四个数据：占两个字节，含义：读取字节数，如果为0xffff时，则从0开始，到地址结束。2byte
    //对于单个字符则发送的数据量为：
    //SendTotalCommand=2byte+1byte+9byte=12byte

    #endregion

    #region 服务端返回的数据包
    //这部分的协议是针对ZJ118也就zj116而言，对其他的适不适合需要看协议返回的是不是一样的，其是大端在前，小端在后，采用的ascll编码格式
    //对于数据返回过来的字节进行一次详细的解析，
    //返回过来的第一个数据：占两个字节，含义为：帧长度 2byte
    //返回过来的第二个数据：占一个字节，含义为：帧类型（读，写，读品牌，读参数等）1byte
    //返回过来的第三个数据：请求的1个符号地址数据，该数据又是一个结构
    //第三个数据包含的数据含义：
    //第一个数据:占一个字节,   含义:功能码 1byte
    //第二个数据:占4个字节,   含义：符号地址名称，4byte
    //第三个数据:占2个字节,    含义：读取数据的符号地址偏移量，2byte
    //第四个数据:占2个字节,   含义:读取的字节数，2 byte
    //第五个数据：**** ，     含义：数据位，占的字节根据数据符号定义的数据来的，
    //所以读取的数据的开始地址为：totalBytes=2byte+1byte+1byte+4byte+2byte+2byte=12byte,注意，读取的是所有的数据地址
    //所以为了提升效率，可以一次读取多个符号变量，然后统一解析，对于超出总的数据长度时，可以通过进行分组来实现，这样可以节省读取次数，但是需要注意的一点就是
    //在读取出错的时候，需要对读取ok的数据进行解析，并进行上抛动作
    #endregion

    #endregion

    public class ZJ118Controller : DataCollectContoller
    {
        private ConcurrentDictionary<string, bool> ZJ118StopInfoBuffer = new ConcurrentDictionary<string, bool>();

        private int _currentZJ118ProductionCounter = 0;
        private string _brandname = string.Empty;
        private PROC ZJ118proc = new PROC();

        private List<byte> functions118 = new List<byte>()
        {
            0x21,0x22,0x23,0x25,0x27,0x32,0x40,0xB0,0xB1
        };

        public ZJ118Controller(MqttController mqttController) : base(mqttController)
        {
        }


        private async Task<bool> ZJ118CollectProdAndRejectDataAct(string[] symbs)
        {
            CigaretteProductDto productDto = new CigaretteProductDto();
            CigratteRejectDto rejectDto = new CigratteRejectDto();
            DateTime tt = DateTime.Now;
            bool flag = false;

            try
            {
                byte[] command = HuaniCommand.PackCommandBySymbArray(0, 240, symbs, config.HuaniProtocolTypeEnum);
                byte[] response = await _chanle.SendCommandAsync(command, "ZJ118CollectProdAndRejectDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "SHFD", "ZJ118CollectProdAndRejectDataAct"))
                {
                    if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ118)
                    {
                        SHFD ZJ118 = VisuHelper.ParseStruct<SHFD>(response, 12);
                        productDto.PRODUCEDATE_IN = tt.ToString("yyyy-MM-dd");
                        productDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                        productDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                        productDto.SWJDM = string.Empty;
                        productDto.XWJDM = config.MachineName;
                        productDto.NO = 1;
                        productDto.TOTALPRODUCTION = (int)ZJ118.TotalProduction;
                        productDto.TOTALWASTE = (int)ZJ118proc.TotalWasteCount;
                        productDto.TOTALWASTEPCT = ZJ118proc.TotalWastePercent;
                        productDto.EFFMACHINE = ZJ118.EffMachine;
                        productDto.EFFPRODUCTION = ZJ118.EffProduction;
                        productDto.RUNTIME = (int)ZJ118.RunTime;
                        productDto.TOTALSTOPCNT = (int)ZJ118.Stops;
                        productDto.TOTALSTOPTIME = (int)ZJ118.StopTime;
                        productDto.batch = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                        productDto.PRODUCTIONSTARTTIME = VisuHelper.P18Uint32ToDateTime(ZJ118.ShiftStart);
                        productDto.PAPER = ZJ118.Tobacco_Cons;
                        productDto.WPAPER = ZJ118.Filter_Cons;
                        productDto.TobaccoSilk = ZJ118.MLP_Air_Count;
                        productDto.FILTERTIP = (int)ZJ118.Paper_Cons;
                        productDto.PAPERPAN = (int)ZJ118.MLP_FilterCount;
                        productDto.WPAPERPAN = (int)ZJ118.MLP_MAXBOB;
                        productDto.PAOTIAOS = ZJ118.MLP_CurrentDrain;
                        productDto.GATHERTYPE = "A";

                        rejectDto.PRODUCEDATE_IN = tt.ToString("yyyy-MM-dd");
                        rejectDto.PB_SHIFT_ID = productDto.PB_SHIFT_ID;
                        rejectDto.PB_PRODUCT_ID = productDto.PB_PRODUCT_ID;
                        rejectDto.PM_MP_MACHINEPART_ID = "0";
                        rejectDto.SWJDM = string.Empty;
                        rejectDto.XWJDM = config.MachineName;
                        rejectDto.REJECTS = new List<RejectInfo>();
                        RejectInfo reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道太轻",
                            REJECTVALUE = (int)ZJ118.LightWeightCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道太重",
                            REJECTVALUE = (int)ZJ118.HeavyWeightCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前软点",
                            REJECTVALUE = (int)ZJ118.SoftSpotsCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前硬点",
                            REJECTVALUE = (int)ZJ118.HardSpotsCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前轻烟端",
                            REJECTVALUE = (int)ZJ118.LightEndsCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                        {
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "前道异物剔除",
                                REJECTVALUE = (int)ZJ118.ForeignBodyCnt_f
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "前道钢印偏移",
                                REJECTVALUE = (int)ZJ118.ORIS_PaintOffsetCnt_f
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 前道暗污点",
                                REJECTVALUE = (int)ZJ118.ORIS_DarkPointCnt_f
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 前道亮污点",
                                REJECTVALUE = (int)ZJ118.ORIS_LightPointCnt_f
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 前道暗钢印",
                                REJECTVALUE = (int)ZJ118.ORIS_DarkPaintCnt_f
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 前道亮钢印",
                                REJECTVALUE = (int)ZJ118.ORIS_LightPaintCnt_f
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "前道无胶水",
                                REJECTVALUE = (int)ZJ118.NoGlueCnt_f
                            };
                            rejectDto.REJECTS.Add(reject);
                        }
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道空头(LES)",
                            REJECTVALUE = (int)ZJ118.LESCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道烟支外观(OTIS)",
                            REJECTVALUE = (int)ZJ118.OTISCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道漏气",
                            REJECTVALUE = (int)ZJ118.AirtightnessCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道通风度",
                            REJECTVALUE = (int)ZJ118.TotalVentilationCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道吸阻",
                            REJECTVALUE = (int)ZJ118.PressureDropCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道ORIS废品",
                            REJECTVALUE = (int)ZJ118.ORISCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道缺少滤嘴",
                            REJECTVALUE = (int)ZJ118.MissingFilterCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "前道杂质",
                            REJECTVALUE = (int)ZJ118.METISCnt_f
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道太轻",
                            REJECTVALUE = (int)ZJ118.LightWeightCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道太重",
                            REJECTVALUE = (int)ZJ118.HeavyWeightCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道软点",
                            REJECTVALUE = (int)ZJ118.SoftSpotsCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道硬点",
                            REJECTVALUE = (int)ZJ118.HardSpotsCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道轻烟端",
                            REJECTVALUE = (int)ZJ118.LightEndsCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                        {
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "后道异物剔除",
                                REJECTVALUE = (int)ZJ118.ForeignBodyCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "后道钢印偏移",
                                REJECTVALUE = (int)ZJ118.ORIS_PaintOffsetCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "后道ORIS废品",
                                REJECTVALUE = (int)ZJ118.ORISCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 后道暗污点",
                                REJECTVALUE = (int)ZJ118.ORIS_DarkPointCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 后道亮污点",
                                REJECTVALUE = (int)ZJ118.ORIS_LightPointCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 后道暗钢印",
                                REJECTVALUE = (int)ZJ118.ORIS_DarkPaintCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "ORIS 后道亮钢印",
                                REJECTVALUE = (int)ZJ118.ORIS_LightPaintCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                            reject = new RejectInfo
                            {
                                PM_MP_REJECTCODE_ID = "后道无胶水",
                                REJECTVALUE = (int)ZJ118.NoGlueCnt_r
                            };
                            rejectDto.REJECTS.Add(reject);
                        }
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道空头(LES)",
                            REJECTVALUE = (int)ZJ118.LESCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道烟支外观(OTIS)",
                            REJECTVALUE = (int)ZJ118.OTISCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道漏气",
                            REJECTVALUE = (int)ZJ118.AirtightnessCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道通风度",
                            REJECTVALUE = (int)ZJ118.TotalVentilationCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道吸阻",
                            REJECTVALUE = (int)ZJ118.PressureDropCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道缺少滤嘴",
                            REJECTVALUE = (int)ZJ118.MissingFilterCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        reject = new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = "后道杂质",
                            REJECTVALUE = (int)ZJ118.METISCnt_r
                        };
                        rejectDto.REJECTS.Add(reject);
                        rejectDto.GATHERTYPE = "A";
                        flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error($"在 ZJ118CollectProdAndRejectDataAct occur exception:{ex2}");
                flag = false;
            }
            if (flag)
            {
                await _mqttController.ConnectAsync("ZJ118CollectProdAndRejectDataAct");
                await _mqttController.PublishMessageAsync(config.MachineName + "/102", JsonConvert.SerializeObject(productDto), isSaveMessage: true, "ZJ118CollectProdAndRejectDataAct");
                await _mqttController.PublishMessageAsync(config.MachineName + "/1057", JsonConvert.SerializeObject(rejectDto), isSaveMessage: true, "ZJ118CollectProdAndRejectDataAct");
            }
            return flag;
        }

        private async Task<bool> ZJ118CollectStopInfoAct(string[] symb)
        {
            CigratteStopInfoDto stopInfoDto = new CigratteStopInfoDto();
            CigratteStopDataDto stopDataDto = new CigratteStopDataDto();
            byte[] command = null;
            byte[] response = null;
            bool flag;
            try
            {
                command = HuaniCommand.PackCommandBySymbArray(0, 240, symb, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectStopInfoAct");
                if (!HuaniCommand.CheckIfResponsIsValid(response, "SHIS", "ZJ118CollectStopInfoAct"))
                {
                    return false;
                }
                SHIS stopInfos = SHISParser.Parse(response, config.HuaniProtocolTypeEnum);
                stopInfoDto.PRODUCEDATE_IN = DateTime.Now.ToString("yyyy-MM-dd");
                stopInfoDto.STOPS = new List<StopInfo>();
                stopInfoDto.SWJDM = string.Empty;
                stopInfoDto.XWJDM = config.MachineName;
                stopInfoDto.PLANWORKORDERNO = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                stopInfoDto.PM_MP_MACHINEPART_ID = "0";
                stopInfoDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                stopInfoDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                stopInfoDto.GATHERTYPE = "A";
                stopInfoDto.NO = 1;
                for (int i = 0; i < stopInfos.Index; i++)
                {
                    StopInfo stopInfo = new StopInfo
                    {
                        PM_MP_STOPCODE_ID = VisuHelper.GetP18StopCodeForSHIS(stopInfos.Stops[i].No),
                        STOPSTARTTIME = VisuHelper.ZJ118Uint32ToDateTime(stopInfos.Stops[i].StartTime),
                        STOPENDTIME = VisuHelper.ZJ118Uint32ToDateTime(stopInfos.Stops[i].EndTime),
                        STOPTIME = (int)stopInfos.Stops[i].Downtime
                    };
                    //if (!ZJ118StopInfoBuffer.ContainsKey(stopInfo.STOPSTARTTIME))
                    //{
                    //    stopInfoDto.STOPS.Add(stopInfo);
                    //}
                    stopInfoDto.STOPS.Add(stopInfo);
                }
                flag = true;
                stopDataDto = new CigratteStopDataDto();
                command = HuaniCommand.PackCommandBySymbArray(0, 240, new string[1] { SymbAdrEnum.SANA.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectStopInfoAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "SANA", "ZJ118CollectStopInfoAct"))
                {
                    SANA res = SANAParser.Parse(response, config.HuaniProtocolTypeEnum);
                    stopDataDto.PRODUCEDATE_IN = DateTime.Now.ToString("yyyy-MM-dd");
                    stopDataDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    stopDataDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    stopDataDto.PLANWORKORDERNO = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                    stopDataDto.PM_MP_MACHINEPART_ID = "0";
                    stopDataDto.XWJDM = config.MachineName;
                    stopDataDto.NO = 1;
                    stopDataDto.GATHERTYPE = "A";
                    stopDataDto.STOPS = new List<MachineGd110DetailModel>();
                    SANA_ENTRY[] stops = res.Stops;
                    for (int j = 0; j < stops.Length; j++)
                    {
                        SANA_ENTRY item = stops[j];
                        if (item.No != 0)
                        {
                            MachineGd110DetailModel model = new MachineGd110DetailModel
                            {
                                STOPTIME = (int)item.Downtime,
                                STOPCNT = (int)item.Count,
                                PM_MP_STOPCODE_ID = VisuHelper.GetP18StopCodeForSHIS(item.No)
                            };
                            stopDataDto.STOPS.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error("Send Command:" + BitConverter.ToString(command));
                if (response != null)
                {
                    LogController.Instance.Error("Recieve response:" + BitConverter.ToString(response));
                }
                LogController.Instance.Error($"在 ZJ118CollectStopInfoAct occur exception:{ex2}");
                flag = false;
            }
            if (flag)
            {
                await _mqttController.ConnectAsync("ZJ118CollectStopInfoAct");
                if (await _mqttController.PublishMessageAsync(config.MachineName + "/SS", JsonConvert.SerializeObject(stopInfoDto), isSaveMessage: true, "ZJ118CollectStopInfoAct"))
                {
                    //foreach (StopInfo item2 in stopInfoDto.STOPS)
                    //{
                    //    ZJ118StopInfoBuffer.TryAdd(item2.STOPSTARTTIME, value: true);
                    //}
                }
                CigratteStopDataDto cigratteStopDataDto = stopDataDto;
                if (cigratteStopDataDto != null && cigratteStopDataDto.STOPS?.Count > 0)
                {
                    await _mqttController.PublishMessageAsync(config.MachineName + "/110", JsonConvert.SerializeObject(stopDataDto), isSaveMessage: true, "ZJ118CollectStopInfoAct");
                }
            }
            return true;
        }


        private async Task<bool> ZJ118CollectRealTimeDataAct()
        {
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = config.MachineName;
            bool flag = false;
            try
            {
               
                var command = HuaniCommand.PackCommandBySymbArray(0, 64, new string[1] { SymbAdrEnum.TEST.ToString() }, config.HuaniProtocolTypeEnum);
                var response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "TEST", "ZJ118CollectRealTimeDataAct"))
                {
                    TEST speed = PROCParser.ParseTEST(response, config.HuaniProtocolTypeEnum);
                    realTimeDataDto.Details.Add(new DataItemDetailDto
                    {
                        IsGood = true,
                        Value = speed.MachineSpeed,
                        Code = config.MachineName + ".MachineSpeed"
                    });

                    MachineStatus =  speed.MachineSpeed > 0 ? MachineStatusEnum.Running : MachineStatusEnum.Stop;
                }
                flag = true;
                List<string> names = new List<string> { "msst", "GSST", "RSST", "PSST", "NSST", "SSST" };
                foreach (string name in names)
                {
                    command = HuaniCommand.PackCommandBySymbArray(0, 35, new string[1] { name }, config.HuaniProtocolTypeEnum);
                    response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                    if (HuaniCommand.CheckIfResponsIsValid(response, name, "ZJ118CollectRealTimeDataAct"))
                    {
                        List<(string, object)> value = PROCParser.Parsemsst(response, name, config.HuaniProtocolTypeEnum);
                        value.ForEach(delegate ((string, object) tuple)
                        {
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                IsGood = true,
                                Value = tuple.Item2,
                                Code = config.MachineName + "." + tuple.Item1
                            });
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error($"在ZJ118CollectRealTimeDataAct occur exception:{ex2}");
            }
            if (flag && realTimeDataDto.Details.Count > 0)
            {
                LogDisPlayAction?.Invoke(realTimeDataDto);
                DataManage.UpdateData( MachineCode, realTimeDataDto.Details);
                await _mqttController.ConnectAsync("ZJ118CollectRealTimeDataAct");
                await _mqttController.PublishMessageAsync("realtime/" + config.MachineName, JsonConvert.SerializeObject(realTimeDataDto), isSaveMessage: false, "ZJ118CollectRealTimeDataAct", AtMostOnce: true);
            }
            return true;
        }

        private async Task<bool> ZJ118CollectOthersDataAct()
        {
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = config.MachineName;
            bool flag = false;
            try
            {
                byte[] command = HuaniCommand.PackCommandBySymbArray(0, 240, new string[1] { SymbAdrEnum.PROC.ToString() }, config.HuaniProtocolTypeEnum);
                byte[] response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "PROC", "ZJ118CollectRealTimeDataAct"))
                {
                    ZJ118proc = PROCParser.Parse(response);
                    FieldInfo[] fields = typeof(PROC).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array = fields;
                    foreach (FieldInfo field in array)
                    {
                        object fieldValue = field.GetValue(ZJ118proc);
                        if (fieldValue != null)
                        {
                            string fieldName = config.MachineName + ".PROC." + field.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName,
                                Value = fieldValue,
                                IsGood = true
                            });
                        }
                    }
                }
                flag = true;
                command = HuaniCommand.PackCommandBySymbArray(0, 240, new string[1] { SymbAdrEnum.SHFD.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "SHFD", "ZJ118CollectRealTimeDataAct"))
                {
                    SHFD shfd = VisuHelper.ParseStruct<SHFD>(response, 12);
                    FieldInfo[] fields2 = typeof(SHFD).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _currentZJ118ProductionCounter = shfd.Brand;
                    _brandname = shfd.BrandName;
                    FieldInfo[] array2 = fields2;
                    foreach (FieldInfo field2 in array2)
                    {
                        if (!field2.Name.Contains("Dummy"))
                        {
                            object fieldValue2 = field2.GetValue(shfd);
                            if (fieldValue2 != null)
                            {
                                string fieldName2 = config.MachineName + ".SHFD." + field2.Name;
                                realTimeDataDto.Details.Add(new DataItemDetailDto
                                {
                                    Code = fieldName2,
                                    Value = fieldValue2,
                                    IsGood = true
                                });
                            }
                        }
                    }
                }
                flag = true;
                command = HuaniCommand.PackCommand(0, 0, VisuCommandNameEnum.ReadBrandParameter, config.HuaniProtocolTypeEnum, "", 0, _currentZJ118ProductionCounter.ToString()).Item1;
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "ReadBrandParameter", "ZJ118CollectRealTimeDataAct"))
                {
                    List<Pvalue> brandParams = HuaniCommand.ParseBrandPvalue(response, config.HuaniProtocolTypeEnum);
                    int num;
                    if (brandParams != null)
                    {
                        num = ((brandParams != null && brandParams.Count > 0) ? 1 : 0);
                    }
                    else
                    {
                        num = 0;
                    }
                    if (num != 0)
                    {
                        foreach (Pvalue item in brandParams)
                        {
                            if (item.ParameterValue != null)
                            {
                                realTimeDataDto.Details.Add(new DataItemDetailDto
                                {
                                    IsGood = true,
                                    Value = item.ParameterValue,
                                    Code = config.MachineName + ".Brand." + item.ParaNumerName
                                });
                            }
                        }
                    }
                }
               
                for (int k = 0; k < functions118.Count; k++)
                {
                    command = HuaniCommand.PackCommand(0, functions118[k], VisuCommandNameEnum.ReadMachineParameter, config.HuaniProtocolTypeEnum, "", 0).Item1;
                    response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                    if (HuaniCommand.CheckIfResponsIsValid(response, "ReadBrandParameter", "ZJ118CollectRealTimeDataAct"))
                    {
                        List<Pvalue> brandParams2 = HuaniCommand.ParseDeviceParamerPvalue(response, config.HuaniProtocolTypeEnum);
                        int num2;
                        if (brandParams2 != null)
                        {
                            num2 = ((brandParams2 != null && brandParams2.Count > 0) ? 1 : 0);
                        }
                        else
                        {
                            num2 = 0;
                        }
                        if (num2 != 0)
                        {
                            foreach (Pvalue item2 in brandParams2)
                            {
                                if (item2.ParameterValue != null)
                                {
                                    realTimeDataDto.Details.Add(new DataItemDetailDto
                                    {
                                        IsGood = true,
                                        Value = item2.ParameterValue,
                                        Code = config.MachineName + ".Machine." + item2.ParaNumerName
                                    });
                                }
                            }
                        }
                    }
                }
                command = HuaniCommand.PackCommandBySymbArray(0, 35, new string[1] { SymbAdrEnum.zsgb.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "zsgb", "ZJ118CollectRealTimeDataAct"))
                {
                    zsgb brandParams3 = PROCParser.Parsezsgb(response, config.HuaniProtocolTypeEnum);
                    FieldInfo[] fields3 = typeof(zsgb).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array3 = fields3;
                    foreach (FieldInfo field3 in array3)
                    {
                        object fieldValue3 = field3.GetValue(brandParams3);
                        if (fieldValue3 != null)
                        {
                            string fieldName3 = config.MachineName + ".zsgb." + field3.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName3,
                                Value = fieldValue3,
                                IsGood = true
                            });
                        }
                    }
                }
                command = HuaniCommand.PackCommandBySymbArray(0, 35, new string[1] { SymbAdrEnum.tsth.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "tsth", "ZJ118CollectRealTimeDataAct"))
                {
                    tsth brandParams4 = PROCParser.Parsetsth(response, config.HuaniProtocolTypeEnum);
                    FieldInfo[] fields4 = typeof(tsth).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array4 = fields4;
                    foreach (FieldInfo field4 in array4)
                    {
                        object fieldValue4 = field4.GetValue(brandParams4);
                        if (fieldValue4 != null)
                        {
                            string fieldName4 = config.MachineName + ".tsth." + field4.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName4,
                                Value = fieldValue4,
                                IsGood = true
                            });
                        }
                    }
                }
                command = HuaniCommand.PackCommandBySymbArray(0, 37, new string[1] { SymbAdrEnum.tmpm.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "tmpm", "ZJ118CollectRealTimeDataAct"))
                {
                    entn brandParams5 = PROCParser.Parseentn(response, config.HuaniProtocolTypeEnum);
                    FieldInfo[] fields5 = typeof(entn).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array5 = fields5;
                    foreach (FieldInfo field5 in array5)
                    {
                        object fieldValue5 = field5.GetValue(brandParams5);
                        if (fieldValue5 != null)
                        {
                            string fieldName5 = config.MachineName + ".entn." + field5.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName5,
                                Value = fieldValue5,
                                IsGood = true
                            });
                        }
                    }
                }
                flag = true;
                command = HuaniCommand.PackCommandBySymbArray(0, 64, new string[1] { SymbAdrEnum.QUAL.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "QUAL", "ZJ118CollectRealTimeDataAct"))
                {
                    List<(string, object)> list = PROCParser.ParseQUAL(response, config.HuaniProtocolTypeEnum);
                    list.ForEach(delegate ((string, object) tuple)
                    {
                        realTimeDataDto.Details.Add(new DataItemDetailDto
                        {
                            IsGood = true,
                            Value = tuple.Item2,
                            Code = config.MachineName + "." + tuple.Item1
                        });
                    });
                }
                command = HuaniCommand.PackCommandBySymbArray(0, 64, new string[1] { SymbAdrEnum.QUAC.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ118CollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "QUAL", "ZJ118CollectRealTimeDataAct"))
                {
                    List<(string, object)> list2 = PROCParser.ParseQUAC(response, config.HuaniProtocolTypeEnum);
                    list2.ForEach(delegate ((string, object) tuple)
                    {
                        realTimeDataDto.Details.Add(new DataItemDetailDto
                        {
                            IsGood = true,
                            Value = tuple.Item2,
                            Code = config.MachineName + "." + tuple.Item1
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error($"在ZJ118CollectRealTimeDataAct occur exception:{ex2}");
            }
            if (flag && realTimeDataDto.Details.Count > 0)
            {
                LogDisPlayAction?.Invoke(realTimeDataDto);
                DataManage.UpdateData(MachineCode, realTimeDataDto.Details);
                await _mqttController.ConnectAsync("ZJ118CollectRealTimeDataAct");
                await _mqttController.PublishMessageAsync("realtime/" + config.MachineName, JsonConvert.SerializeObject(realTimeDataDto), isSaveMessage: false, "ZJ118CollectRealTimeDataAct", AtMostOnce: true);
            }
            return true;
        }

        protected async override Task OthersAsync(List<YS.Yuanji.Commom.Item> items)
        {
            await ZJ118CollectOthersDataAct();
        }

        protected override async Task RealtimeAsync(List<YS.Yuanji.Commom.Item> items)
        {
            await ZJ118CollectRealTimeDataAct();
        }

        protected async override Task ProductAsync(List<YS.Yuanji.Commom.Item> items)
        {
            string[] shfd = new string[] { SymbAdrEnum.SHFD.ToString() };
            await ZJ118CollectProdAndRejectDataAct(shfd);
            string[] shis = new string[] { SymbAdrEnum.SHIS.ToString() };
            await ZJ118CollectStopInfoAct(shis);
        }

    }
}
