using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Reflection;
using YS.Yuanji.Commom;
using YS.Yuanji.Commom.helper;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;
using Yuanji.core.src.Driver.Huani.Controller;
using Yuanji.core.src.Driver.Huani.Entity;

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
    //这部分的协议是针对ZJ116A也就zj116而言，对其他的适不适合需要看协议返回的是不是一样的，其是大端在前，小端在后，采用的ascll编码格式
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
    public  class ZJ116AController:DataCollectContoller
    {
        private ConcurrentDictionary<string, bool> ZJ116AStopInfoBuffer = new ConcurrentDictionary<string, bool>();

        private int _currentZJ116AProductionCounter = 0;

        private int _fiterCounter = 10;

        private Dictionary<string,int> _countfiters = new Dictionary<string, int>();
        private PROC proc_116A = new PROC();

        private List<byte> functions116A = new List<byte>()
        {
            0x11,0x12,0x13,0x21,0X22,0X40,0x50,0x70
        };

        public ZJ116AController(MqttController mqttController) : base(mqttController)
        {
        }

        protected override async Task OthersAsync(List<YS.Yuanji.Commom.Item> items)
        {
            await ZJ116ACollectOtherDataAct();
        }

        protected override async Task RealtimeAsync(List<YS.Yuanji.Commom.Item> items)
        {
            await ZJ116ACollectRealTimeDataAct();
        }

        protected override async Task ProductAsync(List<YS.Yuanji.Commom.Item> items)
        {
            string[] shfd = new string[] { SymbAdrEnum.SHFD.ToString() };
            string[] shis = new string[] { SymbAdrEnum.SHIS.ToString() };
            await ZJ116ACollectProdAndRejectDataAct(shfd);
            await ZJ116ACollectStopInfoAct(shis);
        }

        private async Task ZJ116ACollectProdAndRejectDataAction()
        {

        }

        private async Task<bool> ZJ116ACollectProdAndRejectDataAct(string[] symbs)
        {
            CigaretteProductDto productDto = new CigaretteProductDto();
            CigratteRejectDto rejectDto = new CigratteRejectDto();
            var tt = DateTime.Now;
            bool flag = false;
            await Task.Run(async () =>
            {

                byte[] command = null;
                byte[] response = null;
                try
                {
                    command = HuaniCommand.PackCommandBySymbArray(0, 0xF0, symbs, config.HuaniProtocolTypeEnum);
                    response = await _chanle.SendCommandAsync(command);
                    if (!HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.SHFD)))
                        return;

                    await Task.Delay(ControllerConst.DefaultHauniIntervel);


                    if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ116A)
                    {
                        SHFD ZJ116A = VisuHelper.ParseStruct<SHFD>(response, HuaniConst.P18ReadDataOffset);
                        productDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                        productDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                        productDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                        productDto.SWJDM = string.Empty;
                        productDto.XWJDM = config.MachineName;
                        productDto.NO = 1;
                        productDto.TOTALPRODUCTION = (int)ZJ116A.TotalProduction;
                        productDto.TOTALWASTE = (int)proc_116A.TotalWasteCount;
                        productDto.TOTALWASTEPCT = proc_116A.TotalWastePercent;
                        productDto.EFFMACHINE = ZJ116A.EffMachine;
                        productDto.EFFPRODUCTION = ZJ116A.EffProduction;
                        productDto.RUNTIME = (int)ZJ116A.RunTime;
                        productDto.TOTALSTOPCNT = (int)ZJ116A.Stops;
                        productDto.TOTALSTOPTIME = (int)ZJ116A.StopTime;
                        productDto.batch = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                        productDto.PRODUCTIONSTARTTIME = VisuHelper.P18Uint32ToDateTime(ZJ116A.ShiftStart);
                        //productDto.RUNTIME = (int)ZJ116A.ShiftTime;
                        productDto.PAPER = ZJ116A.Tobacco_Cons; //烟草消耗量
                        productDto.WPAPER = ZJ116A.Filter_Cons; //过滤器消耗量
                        productDto.TobaccoSilk = ZJ116A.MLP_Air_Count; //气耗
                        productDto.FILTERTIP = (int)ZJ116A.Paper_Cons; //纸张消耗量
                        productDto.PAPERPAN = (int)ZJ116A.MLP_FilterCount; //滤嘴消耗计数
                        productDto.WPAPERPAN = (int)ZJ116A.MLP_MAXBOB; //MAX水松纸消耗(盘)
                        productDto.PAOTIAOS = ZJ116A.MLP_CurrentDrain; //电耗消耗量
                        productDto.GATHERTYPE = "A";

                        /////车速
                        //if (config.HuaniProtocolTypeEnum == HuaniProtocolTypeEnum.ZJ116A)
                        //{
                        //    var command1 = HuaniCommand.PackCommandBySymbArray(0, 0x40, new string[] { SymbAdrEnum.TEST.ToString() }, config.HuaniProtocolTypeEnum);
                        //    var response1 = await _visuCommunication.SendCommandAsync(command1);

                        //    if (HuaniCommand.CheckIfResponsIsValid(response1, nameof(SymbAdrEnum.TEST)))
                        //    {
                        //        var speed = PROCParser.ParseTEST(response1,config.HuaniProtocolTypeEnum);
                        //        productDto.MACHINESPEED = speed.MachineSpeed;
                        //    }
                        //}

                        #region 剔除
                        rejectDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                        rejectDto.PB_SHIFT_ID = productDto.PB_SHIFT_ID;
                        rejectDto.PB_PRODUCT_ID = productDto.PB_PRODUCT_ID;
                        rejectDto.PM_MP_MACHINEPART_ID = "0";
                        rejectDto.SWJDM = string.Empty;
                        rejectDto.XWJDM = config.MachineName;
                        rejectDto.REJECTS = new List<RejectInfo>();


                        RejectInfo reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道太轻", REJECTVALUE = (int)ZJ116A.LightWeightCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道太重", REJECTVALUE = (int)ZJ116A.HeavyWeightCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前软点", REJECTVALUE = (int)ZJ116A.SoftSpotsCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前硬点", REJECTVALUE = (int)ZJ116A.HardSpotsCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前轻烟端", REJECTVALUE = (int)ZJ116A.LightEndsCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                        {
                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道异物剔除", REJECTVALUE = (int)ZJ116A.ForeignBodyCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道钢印偏移", REJECTVALUE = (int)ZJ116A.ORIS_PaintOffsetCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道暗污点", REJECTVALUE = (int)ZJ116A.ORIS_DarkPointCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道亮污点", REJECTVALUE = (int)ZJ116A.ORIS_LightPointCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道暗钢印", REJECTVALUE = (int)ZJ116A.ORIS_DarkPaintCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道亮钢印", REJECTVALUE = (int)ZJ116A.ORIS_LightPaintCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道无胶水", REJECTVALUE = (int)ZJ116A.NoGlueCnt_f };
                            rejectDto.REJECTS.Add(reject);
                        }


                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道空头(LES)", REJECTVALUE = (int)ZJ116A.LESCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道烟支外观(OTIS)", REJECTVALUE = (int)ZJ116A.OTISCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道漏气", REJECTVALUE = (int)ZJ116A.AirtightnessCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道通风度", REJECTVALUE = (int)ZJ116A.TotalVentilationCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道吸阻", REJECTVALUE = (int)ZJ116A.PressureDropCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道ORIS废品", REJECTVALUE = (int)ZJ116A.ORISCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道缺少滤嘴", REJECTVALUE = (int)ZJ116A.MissingFilterCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道杂质", REJECTVALUE = (int)ZJ116A.METISCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        #region 后道数据
                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道太轻", REJECTVALUE = (int)ZJ116A.LightWeightCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道太重", REJECTVALUE = (int)ZJ116A.HeavyWeightCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道软点", REJECTVALUE = (int)ZJ116A.SoftSpotsCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道硬点", REJECTVALUE = (int)ZJ116A.HardSpotsCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道轻烟端", REJECTVALUE = (int)ZJ116A.LightEndsCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                        {
                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道异物剔除", REJECTVALUE = (int)ZJ116A.ForeignBodyCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道钢印偏移", REJECTVALUE = (int)ZJ116A.ORIS_PaintOffsetCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道ORIS废品", REJECTVALUE = (int)ZJ116A.ORISCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道暗污点", REJECTVALUE = (int)ZJ116A.ORIS_DarkPointCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道亮污点", REJECTVALUE = (int)ZJ116A.ORIS_LightPointCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道暗钢印", REJECTVALUE = (int)ZJ116A.ORIS_DarkPaintCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道亮钢印", REJECTVALUE = (int)ZJ116A.ORIS_LightPaintCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道无胶水", REJECTVALUE = (int)ZJ116A.NoGlueCnt_r };
                            rejectDto.REJECTS.Add(reject);
                        }

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道空头(LES)", REJECTVALUE = (int)ZJ116A.LESCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道烟支外观(OTIS)", REJECTVALUE = (int)ZJ116A.OTISCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道漏气", REJECTVALUE = (int)ZJ116A.AirtightnessCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道通风度", REJECTVALUE = (int)ZJ116A.TotalVentilationCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道吸阻", REJECTVALUE = (int)ZJ116A.PressureDropCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道缺少滤嘴", REJECTVALUE = (int)ZJ116A.MissingFilterCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道杂质", REJECTVALUE = (int)ZJ116A.METISCnt_r };
                        rejectDto.REJECTS.Add(reject);
                        #endregion
                        #endregion

                        rejectDto.GATHERTYPE = "A";
                        flag = true;
                    }
                }
                catch (Exception ex)
                {
                    // LogController.Instance.Error($"Send Command:{BitConverter.ToString(command)}");
                    // LogController.Instance.Error($"Recieve response:{BitConverter.ToString(response)}");
                    LogController.Instance.Error($"在 ZJ116ACollectProdAndRejectDataAct occur exception:{ex}");
                    flag = false;
                }

            });
            if (flag)
            {
                var res = await _mqttController.ConnectAsync();
                res = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicProductData}", JsonConvert.SerializeObject(productDto));
                res = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicReject}", JsonConvert.SerializeObject(rejectDto));
            }
            return flag;
        }

        private async Task<bool> ZJ116ACollectRealTimeDataAct()
        {
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto
            {
                Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Details = new List<DataItemDetailDto>(),
                MachineId = config.MachineName
            };
            bool flag = false;
            try
            {
                byte[] command ;
                byte[] response;

                command = HuaniCommand.PackCommandBySymbArray(0, 19, new string[1] { SymbAdrEnum.zsgb.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "zsgb", "ZJ116ACollectRealTimeDataAct"))
                {
                    zsgb brandParams3 = PROCParser.Parsezsgb(response, config.HuaniProtocolTypeEnum);
                    FieldInfo[] fields4 = typeof(zsgb).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array4 = fields4;
                    foreach (FieldInfo field4 in array4)
                    {
                        object fieldValue4 = field4.GetValue(brandParams3);
                        if (fieldValue4 != null)
                        {
                            string fieldName4 = config.MachineName + ".zsgb." + field4.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName4,
                                Value = fieldValue4,
                                IsGood = true
                            });
                        }
                    }
                }
                command = HuaniCommand.PackCommandBySymbArray(0, 34, new string[1] { SymbAdrEnum.BPTR.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "tsth", "ZJ116ACollectRealTimeDataAct"))
                {
                    BPTR brandParams4 = PROCParser.ParseBPTR(response, config.HuaniProtocolTypeEnum);
                    FieldInfo[] fields5 = typeof(BPTR).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array5 = fields5;
                    foreach (FieldInfo field5 in array5)
                    {
                        object fieldValue5 = field5.GetValue(brandParams4);
                        if (fieldValue5 != null)
                        {
                            string fieldName5 = config.MachineName + ".BPTR." + field5.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName5,
                                Value = fieldValue5,
                                IsGood = true
                            });
                        }
                    }
                }
                command = HuaniCommand.PackCommandBySymbArray(0, 34, new string[1] { SymbAdrEnum.ZIGH.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "ZIGH", "ZJ116ACollectRealTimeDataAct"))
                {
                    ZIGH brandParams5 = PROCParser.ParseZIGH(response, config.HuaniProtocolTypeEnum);
                    FieldInfo[] fields6 = typeof(ZIGH).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array6 = fields6;
                    foreach (FieldInfo field6 in array6)
                    {
                        object fieldValue6 = field6.GetValue(brandParams5);
                        if (fieldValue6 != null)
                        {
                            string fieldName6 = config.MachineName + ".ZIGH." + field6.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName6,
                                Value = fieldValue6,
                                IsGood = true
                            });
                        }
                    }
                }
                flag = true;
                // 风室负压
                command = HuaniCommand.PackCommandBySymbArray(0, 0x12, new string[1] { SymbAdrEnum.ITOS.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "ITOS", "ZJ116ACollectRealTimeDataAct"))
                {
                    ITOS brandParams5 = PROCParser.ParseITOS(response, config.HuaniProtocolTypeEnum);
                    FieldInfo[] fields6 = typeof(ITOS).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array6 = fields6;
                    foreach (FieldInfo field6 in array6)
                    {
                        object fieldValue6 = field6.GetValue(brandParams5);
                        if (fieldValue6 != null)
                        {
                            string fieldName6 = config.MachineName + ".ITOS." + field6.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName6,
                                Value = fieldValue6,
                                IsGood = true
                            });
                        }
                    }
                }
                // 伺服参数
                command = HuaniCommand.PackCommandBySymbArray(0, 0x81, new string[1] { SymbAdrEnum.SMN4.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "SMN4", "ZJ116ACollectRealTimeDataAct"))
                {
                    var brandParams5 = PROCParser.ParseSMN4(response,"SMN4", config.HuaniProtocolTypeEnum);
                    foreach (var field6 in brandParams5)
                    {
                        if (field6.Item1 != null)
                        {
                            string fieldName6 = config.MachineName + "."+ field6.Item1;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName6,
                                Value = field6.Item2,
                                IsGood = true
                            });
                        }
                    }
                }

                if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ116A)
                {
                    byte[] command2 = HuaniCommand.PackCommandBySymbArray(0, 64, new string[1] { SymbAdrEnum.TEST.ToString() }, config.HuaniProtocolTypeEnum);
                    byte[] response2 = await _chanle.SendCommandAsync(command2, "ZJ116ACollectRealTimeDataAct");
                    if (HuaniCommand.CheckIfResponsIsValid(response2, "TEST", "ZJ116ACollectRealTimeDataAct"))
                    {
                        TEST speed = PROCParser.ParseTEST(response2);
                        realTimeDataDto.Details.Add(new DataItemDetailDto
                        {
                            IsGood = true,
                            Value = speed.MachineSpeed,
                            Code = config.MachineName + ".MachineSpeed"
                        });
                        MachineStatus = speed.MachineSpeed > 0 ? MachineStatusEnum.Running : MachineStatusEnum.Stop;
                    }
                }
                flag = true;
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error($"在 ZJ116ACollectRealTimeDataAct occur exception:{ex2}");
            }
            if (flag && realTimeDataDto.Details.Count > 0)
            {
                LogDisPlayAction?.Invoke(realTimeDataDto);
                DataManage.UpdateData(MachineCode, realTimeDataDto.Details);
                await _mqttController.ConnectAsync("ZJ116ACollectRealTimeDataAct");
                await _mqttController.PublishMessageAsync("realtime/" + config.MachineName, JsonConvert.SerializeObject(realTimeDataDto), isSaveMessage: false, "ZJ116ACollectRealTimeDataAct", AtMostOnce: true);
            }
            return true;
        }

        private async Task<bool> ZJ116ACollectOtherDataAct()
        {
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto
            {
                Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Details = new List<DataItemDetailDto>(),
                MachineId = config.MachineName
            };
            bool flag = false;
            try
            {
                byte[] command = HuaniCommand.PackCommandBySymbArray(0, 240, new string[1] { SymbAdrEnum.PROC.ToString() }, config.HuaniProtocolTypeEnum);
                byte[] response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "PROC", "ZJ116ACollectRealTimeDataAct"))
                {
                    proc_116A = PROCParser.Parse(response);
                    FieldInfo[] fields = typeof(PROC).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array = fields;
                    foreach (FieldInfo field in array)
                    {
                        object fieldValue = field.GetValue(proc_116A);
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
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "SHFD", "ZJ116ACollectRealTimeDataAct"))
                {
                    SHFD shfd = VisuHelper.ParseStruct<SHFD>(response, 12);
                    FieldInfo[] fields2 = typeof(SHFD).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    _currentZJ116AProductionCounter = shfd.Brand;
                    FieldInfo[] array2 = fields2;
                    foreach (FieldInfo field2 in array2)
                    {
                        if (!field2.Name.Contains("Dummy"))
                        {
                            object fieldValue2 = field2.GetValue(shfd);
                            if (fieldValue2 != null)
                            {
                                string fieldName2 = config.MachineName + ".SHFD." + field2.Name;
                                if(fieldName2.Contains("Cnt_"))
                                {
                                    // 对于计数器类的变量，如果值为0，则进行过滤，避免数据跳变造成的干扰
                                    if(shfd.TotalProduction != 0 && fieldValue2.ToString() == "0")
                                    {
                                        var ff = DataManage.GetData(config.MachineName, fieldName2);
                                        int oldValue = 0;
                                        if(ff != null)
                                        {
                                            int.TryParse(ff.Value?.ToString(), out oldValue);
                                        }
                                        int newValue = 0;
                                        int.TryParse(fieldValue2.ToString(), out newValue);
                                        if(newValue < oldValue)
                                        {
                                            int current;
                                            if(!_countfiters.TryGetValue(fieldName2, out current))
                                            {
                                                current = 0;
                                            }
                                            current++;
                                            if(current <= _fiterCounter)
                                            {
                                                _countfiters[fieldName2] = current;
                                                continue;
                                            }
                                            _countfiters[fieldName2] = 0;
                                        }
                                        else
                                        {
                                            _countfiters[fieldName2] = 0;
                                        }
                                    }
                                    else
                                    {
                                        _countfiters[fieldName2] = 0;
                                    }
                                }

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
                command = HuaniCommand.PackCommand(0, 0, VisuCommandNameEnum.ReadBrandParameter, config.HuaniProtocolTypeEnum, "", 0, _currentZJ116AProductionCounter.ToString()).Item1;
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "ReadBrandParameter", "ZJ116ACollectRealTimeDataAct"))
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
                command = HuaniCommand.PackCommandBySymbArray(0, 240, new string[1] { SymbAdrEnum.ACDI.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "ACDI", "ZJ116ACollectRealTimeDataAct"))
                {
                    ACDI_P18 acdi = VisuHelper.ParseStruct<ACDI_P18>(response, 12);
                    FieldInfo[] fields3 = typeof(ACDI_P18).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    FieldInfo[] array3 = fields3;
                    foreach (FieldInfo field3 in array3)
                    {
                        object fieldValue3 = field3.GetValue(acdi);
                        if (fieldValue3 != null)
                        {
                            string fieldName3 = config.MachineName + ".ACDI." + field3.Name;
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = fieldName3,
                                Value = fieldValue3,
                                IsGood = true
                            });
                        }
                    }
                }
                for (int l = 0; l < functions116A.Count; l++)
                {
                    command = HuaniCommand.PackCommand(0, functions116A[l], VisuCommandNameEnum.ReadMachineParameter, config.HuaniProtocolTypeEnum, "", 0).Item1;
                    response = await _chanle.SendCommandAsync(command, "ZJ116ACollectRealTimeDataAct");
                    if (HuaniCommand.CheckIfResponsIsValid(response, "ReadBrandParameter", "ZJ116ACollectRealTimeDataAct"))
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
                    await Task.Delay(20);
                }
                flag = true;
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error($"在 ZJ116ACollectRealTimeDataAct occur exception:{ex2}");
            }
            if (flag && realTimeDataDto.Details.Count > 0)
            {
                LogDisPlayAction?.Invoke(realTimeDataDto);
                DataManage.UpdateData(MachineCode, realTimeDataDto.Details);
                await _mqttController.ConnectAsync("ZJ116ACollectRealTimeDataAct");
                await _mqttController.PublishMessageAsync("realtime/" + config.MachineName, JsonConvert.SerializeObject(realTimeDataDto), isSaveMessage: false, "ZJ116ACollectRealTimeDataAct", AtMostOnce: true);
            }
            return true;
        }


        private async Task<bool> ZJ116ACollectStopInfoAct(string[] symb)
        {
            CigratteStopInfoDto stopInfoDto = new CigratteStopInfoDto();
            CigratteStopDataDto stopDataDto = new CigratteStopDataDto();
            byte[] command = null;
            byte[] response = null;
            bool flag;
            try
            {
                command = HuaniCommand.PackCommandBySymbArray(0, 240, symb, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectStopInfoAct");
                if (!HuaniCommand.CheckIfResponsIsValid(response, "SHIS", "ZJ116ACollectStopInfoAct"))
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
                        STOPSTARTTIME = VisuHelper.P18Uint32ToDateTime(stopInfos.Stops[i].StartTime),
                        STOPENDTIME = VisuHelper.P18Uint32ToDateTime(stopInfos.Stops[i].EndTime),
                        STOPTIME = (int)stopInfos.Stops[i].Downtime
                    };
                    //if (!ZJ116AStopInfoBuffer.ContainsKey(stopInfo.STOPSTARTTIME))
                    //{
                    //    stopInfoDto.STOPS.Add(stopInfo);
                    //}
                    stopInfoDto.STOPS.Add(stopInfo);
                }
                flag = true;
                stopDataDto = new CigratteStopDataDto();
                command = HuaniCommand.PackCommandBySymbArray(0, 240, new string[1] { SymbAdrEnum.SANA.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command, "ZJ116ACollectStopInfoAct");
                if (HuaniCommand.CheckIfResponsIsValid(response, "SANA", "ZJ116ACollectStopInfoAct"))
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
                LogController.Instance.Error($"在 ZJ116ACollectStopInfoAct occur exception:{ex2}");
                flag = false;
            }
            if (flag)
            {
                await _mqttController.ConnectAsync("ZJ116ACollectStopInfoAct");
                if (await _mqttController.PublishMessageAsync(config.MachineName + "/SS", JsonConvert.SerializeObject(stopInfoDto), isSaveMessage: true, "ZJ116ACollectStopInfoAct"))
                {
                    //foreach (StopInfo item in stopInfoDto.STOPS)
                    //{
                    //    ZJ116AStopInfoBuffer.TryAdd(item.STOPSTARTTIME, value: true);
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

        public override Task Initialization()
        {
            _fiterCounter = DeviceConfig.ParameterDict.ContainsKey("fiterCounter") ? int.Parse(DeviceConfig.ParameterDict["fiterCounter"]):10;
            return base.Initialization();
        }
    }
}
