using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using  Yuanji.core.src.Driver.Huani.Controller;
using  Yuanji.core.src.Driver.Huani.Entity;
using YS.Yuanji.Log;
using YS.Yuanji.Commom;
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
    //这部分的协议是针对p18也就zj116而言，对其他的适不适合需要看协议返回的是不是一样的，其是大端在前，小端在后，采用的ascll编码格式
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

    public  class P18Controller: DataCollectContoller
    {
        private ConcurrentDictionary<string, bool> P18StopInfoBuffer = new ConcurrentDictionary<string, bool>();

        private int _currentP18ProductionCounter = 0;

        private PROC proc = new PROC();

        private List<byte> functions116 = new List<byte>()
        {
            0x11,0x12,0x13,0x21
        };

        private List<byte> functions119 = new List<byte>()
        {
            0x11,0x12,0x13,0x21,0x22,0x27,0x31,0x32,0x40,0x50,0x70,0xB0,0xB1
        };

        public P18Controller(MqttController mqttController) : base(mqttController)
        {
        }

        private async Task P18CollectProdAndRejectDataAction()
        {
            string[] shfd = new string[] { SymbAdrEnum.SHFD.ToString() };
            string[] shis = new string[] { SymbAdrEnum.SHIS.ToString() };
            await P18CollectProdAndRejectDataAct(shfd);

            await Task.Delay(ControllerConst.DefaultHauniIntervel);
            await P18CollectStopInfoAct(shis);

            await Task.Delay(ControllerConst.DefaultHauniIntervel);
            await P18CollectRealTimeDataAct();
        }

        private async Task<bool> P18CollectProdAndRejectDataAct(string[] symbs)
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


                    if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.P18
                        || config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                    {
                        SHFD p18 = VisuHelper.ParseStruct<SHFD>(response, HuaniConst.P18ReadDataOffset);
                        productDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                        productDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                        productDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                        productDto.SWJDM = string.Empty;
                        productDto.XWJDM = config.MachineName;
                        productDto.NO = 1;
                        productDto.TOTALPRODUCTION = (int)p18.TotalProduction;
                        productDto.TOTALWASTE = (int)proc.TotalWasteCount;
                        productDto.TOTALWASTEPCT = proc.TotalWastePercent;
                        productDto.EFFMACHINE = p18.EffMachine;
                        productDto.EFFPRODUCTION = p18.EffProduction;
                        productDto.RUNTIME = (int)p18.RunTime;
                        productDto.TOTALSTOPCNT = (int)p18.Stops;
                        productDto.TOTALSTOPTIME = (int)p18.StopTime;
                        productDto.batch = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                        productDto.PRODUCTIONSTARTTIME = VisuHelper.P18Uint32ToDateTime(p18.ShiftStart);
                        //productDto.RUNTIME = (int)p18.ShiftTime;
                        productDto.PAPER = p18.Tobacco_Cons; //烟草消耗量
                        productDto.WPAPER = p18.Filter_Cons; //过滤器消耗量
                        productDto.TobaccoSilk = p18.MLP_Air_Count; //气耗
                        productDto.FILTERTIP = (int)p18.Paper_Cons; //纸张消耗量
                        productDto.PAPERPAN = (int)p18.MLP_FilterCount; //滤嘴消耗计数
                        productDto.WPAPERPAN = (int)p18.MLP_MAXBOB; //MAX水松纸消耗(盘)
                        productDto.PAOTIAOS = p18.MLP_CurrentDrain; //电耗消耗量
                        productDto.GATHERTYPE = "A";

                         ///车速
                        if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.P18)
                        {
                            var command1 = HuaniCommand.PackCommandBySymbArray(0, 0x40, new string[] { SymbAdrEnum.TEST.ToString() }, config.HuaniProtocolTypeEnum);
                            var response1 = await _chanle.SendCommandAsync(command1);

                            if (HuaniCommand.CheckIfResponsIsValid(response1, nameof(SymbAdrEnum.TEST)))
                            {
                                var speed = PROCParser.ParseTEST(response1);
                                productDto.MACHINESPEED = speed.MachineSpeed;
                            }
                        }
                        else if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                        {

                            var command1 = HuaniCommand.PackCommandBySymbArray(0, 0x13, new string[] { SymbAdrEnum.GNRL.ToString() }, config.HuaniProtocolTypeEnum);
                            var response1 = await _chanle.SendCommandAsync(command1);

                            if (HuaniCommand.CheckIfResponsIsValid(response1, nameof(SymbAdrEnum.GNRL)))
                            {
                                var speed = PROCParser.ParseGNRL(response1);
                                productDto.MACHINESPEED = speed.MachineSpeed;
                            }
                        }

                        #region 剔除
                        rejectDto.PRODUCEDATE_IN = tt.ToString(MqttConst.CollectDateFormat);
                        rejectDto.PB_SHIFT_ID = productDto.PB_SHIFT_ID;
                        rejectDto.PB_PRODUCT_ID = productDto.PB_PRODUCT_ID;
                        rejectDto.PM_MP_MACHINEPART_ID = "0";
                        rejectDto.SWJDM = string.Empty;
                        rejectDto.XWJDM = config.MachineName;
                        rejectDto.REJECTS = new List<RejectInfo>();


                        RejectInfo reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道太轻", REJECTVALUE = (int)p18.LightWeightCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道太重", REJECTVALUE = (int)p18.HeavyWeightCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前软点", REJECTVALUE = (int)p18.SoftSpotsCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前硬点", REJECTVALUE = (int)p18.HardSpotsCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前轻烟端", REJECTVALUE = (int)p18.LightEndsCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                        {
                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道异物剔除", REJECTVALUE = (int)p18.ForeignBodyCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道钢印偏移", REJECTVALUE = (int)p18.ORIS_PaintOffsetCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道暗污点", REJECTVALUE = (int)p18.ORIS_DarkPointCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道亮污点", REJECTVALUE = (int)p18.ORIS_LightPointCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道暗钢印", REJECTVALUE = (int)p18.ORIS_DarkPaintCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 前道亮钢印", REJECTVALUE = (int)p18.ORIS_LightPaintCnt_f };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道无胶水", REJECTVALUE = (int)p18.NoGlueCnt_f };
                            rejectDto.REJECTS.Add(reject);
                        }


                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道空头(LES)", REJECTVALUE = (int)p18.LESCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道烟支外观(OTIS)", REJECTVALUE = (int)p18.OTISCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道漏气", REJECTVALUE = (int)p18.AirtightnessCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道通风度", REJECTVALUE = (int)p18.TotalVentilationCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道吸阻", REJECTVALUE = (int)p18.PressureDropCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道ORIS废品", REJECTVALUE = (int)p18.ORISCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道缺少滤嘴", REJECTVALUE = (int)p18.MissingFilterCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "前道杂质", REJECTVALUE = (int)p18.METISCnt_f };
                        rejectDto.REJECTS.Add(reject);

                        #region 后道数据
                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道太轻", REJECTVALUE = (int)p18.LightWeightCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道太重", REJECTVALUE = (int)p18.HeavyWeightCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道软点", REJECTVALUE = (int)p18.SoftSpotsCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道硬点", REJECTVALUE = (int)p18.HardSpotsCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道轻烟端", REJECTVALUE = (int)p18.LightEndsCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                        {
                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道异物剔除", REJECTVALUE = (int)p18.ForeignBodyCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道钢印偏移", REJECTVALUE = (int)p18.ORIS_PaintOffsetCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道ORIS废品", REJECTVALUE = (int)p18.ORISCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道暗污点", REJECTVALUE = (int)p18.ORIS_DarkPointCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道亮污点", REJECTVALUE = (int)p18.ORIS_LightPointCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道暗钢印", REJECTVALUE = (int)p18.ORIS_DarkPaintCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "ORIS 后道亮钢印", REJECTVALUE = (int)p18.ORIS_LightPaintCnt_r };
                            rejectDto.REJECTS.Add(reject);

                            reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道无胶水", REJECTVALUE = (int)p18.NoGlueCnt_r };
                            rejectDto.REJECTS.Add(reject);
                        }

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道空头(LES)", REJECTVALUE = (int)p18.LESCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道烟支外观(OTIS)", REJECTVALUE = (int)p18.OTISCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道漏气", REJECTVALUE = (int)p18.AirtightnessCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道通风度", REJECTVALUE = (int)p18.TotalVentilationCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道吸阻", REJECTVALUE = (int)p18.PressureDropCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道缺少滤嘴", REJECTVALUE = (int)p18.MissingFilterCnt_r };
                        rejectDto.REJECTS.Add(reject);

                        reject = new RejectInfo() { PM_MP_REJECTCODE_ID = "后道杂质", REJECTVALUE = (int)p18.METISCnt_r };
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
                    LogController.Instance.Error($"在 P18CollectProdAndRejectDataAct occur exception:{ex}");
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

        private async Task<bool> P18CollectStopInfoAct(string[] symb)
        {
            CigratteStopInfoDto stopInfoDto = new CigratteStopInfoDto();
            bool flag = false;

            byte[] command = null;
            byte[] response = null;

            try
            {
                command = HuaniCommand.PackCommandBySymbArray(0, 0xF0, symb, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command);
                if (!HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.SHIS)))
                    return false;

                var stopInfos = SHISParser.Parse(response, config.HuaniProtocolTypeEnum);
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

                    stopInfo.PM_MP_STOPCODE_ID = VisuHelper.GetP18StopCodeForSHIS(stopInfos.Stops[i].No);
                    stopInfo.STOPSTARTTIME = VisuHelper.P18Uint32ToDateTime(stopInfos.Stops[i].StartTime);
                    stopInfo.STOPENDTIME = VisuHelper.P18Uint32ToDateTime(stopInfos.Stops[i].EndTime);
                    stopInfo.STOPTIME = (int)stopInfos.Stops[i].Downtime;

                    if (!P18StopInfoBuffer.ContainsKey(stopInfo.STOPSTARTTIME))
                        stopInfoDto.STOPS.Add(stopInfo);
                }
                if (stopInfoDto.STOPS.Count > 0)
                    flag = true;
                else
                    flag = false;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"Send Command:{BitConverter.ToString(command)}");
                if (response != null) LogController.Instance.Error($"Recieve response:{BitConverter.ToString(response)}");
                LogController.Instance.Error($"在 P18CollectStopInfoAct occur exception:{ex}");
                flag = false;
            }

            if (flag)
            {
                await _mqttController.ConnectAsync();
                var res = await _mqttController.PublishMessageAsync($"{config.MachineName}/{MqttConst.MqttTopicStopInfo}", JsonConvert.SerializeObject(stopInfoDto));
                if (res)
                {
                    foreach (var item in stopInfoDto.STOPS)
                        P18StopInfoBuffer.TryAdd(item.STOPSTARTTIME, true);
                }
            }
            return true;
        }

        private async Task<bool> P18CollectRealTimeDataAct()//TODO 设备参数的为添加
        {
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();
            realTimeDataDto.MachineId = config.MachineName;
            bool flag = false;

            byte[] command = null;
            byte[] response = null;
            try
            {
                command = HuaniCommand.PackCommandBySymbArray(0, 0xF0, new string[] { SymbAdrEnum.PROC.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command);
                if (HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.PROC)))
                {
                    proc = PROCParser.Parse(response);
                    var fields = typeof(PROC).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var field in fields)
                    {
                        var fieldValue = field.GetValue(proc);
                        if (fieldValue == null)
                            continue;
                        var fieldName = $"{config.MachineName}.PROC.{field.Name}"; // 字段名
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                    }
                }
                flag = true;
                await Task.Delay(ControllerConst.DefaultHauniIntervel);

                command = HuaniCommand.PackCommandBySymbArray(0, 0xF0, new string[] { SymbAdrEnum.SHFD.ToString() }, config.HuaniProtocolTypeEnum);
                response = await _chanle.SendCommandAsync(command);
                if (HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.SHFD)))
                {
                    var shfd = VisuHelper.ParseStruct<SHFD>(response, HuaniConst.P18ReadDataOffset);
                    var fields = typeof(SHFD).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    _currentP18ProductionCounter = shfd.Brand;

                    foreach (var field in fields)
                    {
                        if (field.Name.Contains("Dummy"))
                            continue;

                        var fieldValue = field.GetValue(shfd);
                        if (fieldValue == null)
                            continue;

                        var fieldName = $"{config.MachineName}.SHFD.{field.Name}"; // 字段名
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                    }
                }
                flag = true;
                await Task.Delay(ControllerConst.DefaultHauniIntervel);
                command = HuaniCommand.PackCommand(0, 0
, VisuCommandNameEnum.ReadBrandParameter, config.HuaniProtocolTypeEnum,brandName:_currentP18ProductionCounter.ToString(), numberParam: 0).Item1;
                response = await _chanle.SendCommandAsync(command);
                if (HuaniCommand.CheckIfResponsIsValid(response, nameof(VisuCommandNameEnum.ReadBrandParameter)))
                {
                    var brandParams = HuaniCommand.ParseBrandPvalue(response, config.HuaniProtocolTypeEnum);
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

                ///设备参数.
                await Task.Delay(ControllerConst.DefaultHauniIntervel);

                if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                {
                    for (int i = 0; i < functions119.Count; i++)
                    {
                        command = HuaniCommand.PackCommand(0, functions119[i]
                        , VisuCommandNameEnum.ReadMachineParameter, config.HuaniProtocolTypeEnum, numberParam: 0).Item1;
                        response = await _chanle.SendCommandAsync(command);
                        if (HuaniCommand.CheckIfResponsIsValid(response, nameof(VisuCommandNameEnum.ReadBrandParameter)))
                        {
                            var brandParams = HuaniCommand.ParseDeviceParamerPvalue(response, config.HuaniProtocolTypeEnum);
                            if (brandParams != null && brandParams?.Count > 0)
                            {
                                foreach (var item in brandParams)
                                {
                                    if (item.ParameterValue == null)
                                        continue;

                                    realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item.ParameterValue, Code = $"{config.MachineName}.Machine.{item.ParaNumerName}" });
                                }
                            }
                        }

                        await Task.Delay(100);
                    }
                }
                else
                {
                    for (int i = 0; i < functions116.Count; i++)
                    {
                        command = HuaniCommand.PackCommand(0, functions116[i]
                        , VisuCommandNameEnum.ReadMachineParameter, config.HuaniProtocolTypeEnum, numberParam: 0).Item1;
                        response = await _chanle.SendCommandAsync(command);
                        if (HuaniCommand.CheckIfResponsIsValid(response, nameof(VisuCommandNameEnum.ReadBrandParameter)))
                        {
                            var brandParams = HuaniCommand.ParseDeviceParamerPvalue(response, config.HuaniProtocolTypeEnum);
                            if (brandParams != null && brandParams?.Count > 0)
                            {
                                foreach (var item in brandParams)
                                {
                                    if (item.ParameterValue == null)
                                        continue;

                                    realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = item.ParameterValue, Code = $"{config.MachineName}.Machine.{item.ParaNumerName}" });
                                }
                            }
                        }

                        await Task.Delay(20);
                    }
                }

               

                await Task.Delay(ControllerConst.DefaultHauniIntervel);

                if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.P18)
                {
                    // 温度.
                    command = HuaniCommand.PackCommandBySymbArray(0, 0x22, new string[] { SymbAdrEnum.ZIGH.ToString() }, config.HuaniProtocolTypeEnum);
                    response = await _chanle.SendCommandAsync(command);
                    if (HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.ZIGH)))
                    {
                        var brandParams = PROCParser.ParseTemp(response, config.HuaniProtocolTypeEnum);
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = brandParams, Code = $"{config.MachineName}.Temperature1" });

                    }
                    await Task.Delay(ControllerConst.DefaultHauniIntervel);
                    command = HuaniCommand.PackCommandBySymbArray(0, 0x22, new string[] { SymbAdrEnum.BPTR.ToString() }, config.HuaniProtocolTypeEnum);
                    response = await _chanle.SendCommandAsync(command);
                    if (HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.BPTR)))
                    {
                        var brandParams = PROCParser.ParseTemp(response, config.HuaniProtocolTypeEnum);
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = brandParams, Code = $"{config.MachineName}.Temperature2" });

                    }
                    await Task.Delay(ControllerConst.DefaultHauniIntervel);
                    command = HuaniCommand.PackCommandBySymbArray(0, 0x13, new string[] { SymbAdrEnum.TEMP.ToString() }, config.HuaniProtocolTypeEnum);
                    response = await _chanle.SendCommandAsync(command);
                    if (HuaniCommand.CheckIfResponsIsValid(response, nameof(SymbAdrEnum.TEMP)))
                    {
                        var brandParams = PROCParser.ParseTemp(response, config.HuaniProtocolTypeEnum);
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = brandParams, Code = $"{config.MachineName}.Temperature3" });

                    }
                }
                else if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                {
                    var command1 = HuaniCommand.PackCommandBySymbArray(0, 0x22, new string[] { SymbAdrEnum.MDRM.ToString() }, config.HuaniProtocolTypeEnum);
                    var response1 = await _chanle.SendCommandAsync(command1);

                    if (HuaniCommand.CheckIfResponsIsValid(response1, nameof(SymbAdrEnum.MDRM)))
                    {
                        var speed = PROCParser.ParseMDRM(response1);

                        var fields = typeof(MDRM).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            var fieldValue = field.GetValue(speed);
                            if (fieldValue == null)
                                continue;

                            var fieldName = $"{config.MachineName}.MDRM.{field.Name}"; // 字段名
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                        }
                    }

                    command1 = HuaniCommand.PackCommandBySymbArray(0, 0x22, new string[] { SymbAdrEnum.TPGL.ToString() }, config.HuaniProtocolTypeEnum);
                    response1 = await _chanle.SendCommandAsync(command1);

                    if (HuaniCommand.CheckIfResponsIsValid(response1, nameof(SymbAdrEnum.TPGL)))
                    {
                        var speed = PROCParser.ParseTPGL_1(response1);

                        var fields = typeof(TPGL_1).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            var fieldValue = field.GetValue(speed);
                            if (fieldValue == null)
                                continue;

                            var fieldName = $"{config.MachineName}.TPGL.{field.Name}"; // 字段名
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                        }
                    }

                }
                flag = true;    
                ///车速
                if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.P18)
                {
                    var command1 = HuaniCommand.PackCommandBySymbArray(0, 0x40, new string[] { SymbAdrEnum.TEST.ToString() }, config.HuaniProtocolTypeEnum);
                    var response1 = await _chanle.SendCommandAsync(command1);

                    if (HuaniCommand.CheckIfResponsIsValid(response1, nameof(SymbAdrEnum.TEST)))
                    {
                        var speed = PROCParser.ParseTEST(response1);
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = speed.MachineSpeed, Code = $"{config.MachineName}.MachineSpeed" });
                        realTimeDataDto.Details.Add(new DataItemDetailDto() { IsGood = true, Value = speed.MachineSpeed > 0 ? "生产" : "停机", Code = $"{config.MachineName}.MachineStatus" });
                    }
                }
                else if (config.HuaniProtocolTypeEnum == ProtocolTypeEnum.ZJ119)
                {
                    var command1 = HuaniCommand.PackCommandBySymbArray(0, 0x13, new string[] { SymbAdrEnum.GNRL.ToString() }, config.HuaniProtocolTypeEnum);
                    var response1 = await _chanle.SendCommandAsync(command1);

                    if (HuaniCommand.CheckIfResponsIsValid(response1, nameof(SymbAdrEnum.GNRL)))
                    {
                        var speed = PROCParser.ParseGNRL(response1);

                        var fields = typeof(GNRL).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        foreach (var field in fields)
                        {
                            var fieldValue = field.GetValue(speed);
                            if (fieldValue == null)
                                continue;

                            var fieldName = $"{config.MachineName}.GNRL.{field.Name}"; // 字段名
                            realTimeDataDto.Details.Add(new DataItemDetailDto() { Code = fieldName, Value = fieldValue, IsGood = true });
                        }
                    }
                }

                flag = true;
            }
            catch (Exception ex)
            {
                //LogController.Instance.Error($"Send Command:{BitConverter.ToString(command)}");
                //LogController.Instance.Error($"Recieve response:{BitConverter.ToString(response)}");
                LogController.Instance.Error($"在 P18CollectRealTimeDataAct occur exception:{ex}");
                //flag = false;
            }

            if (flag)
            {
                if (realTimeDataDto.Details.Count > 0)
                {
                    LogDisPlayAction?.Invoke(realTimeDataDto);
                    await _mqttController.ConnectAsync();
                    var res = await _mqttController.PublishMessageAsync($"{MqttConst.MqttTopicRealTime}/{config.MachineName}", JsonConvert.SerializeObject(realTimeDataDto), false,AtMostOnce:true);
                }
            }
            return true;
        }
    }
}
