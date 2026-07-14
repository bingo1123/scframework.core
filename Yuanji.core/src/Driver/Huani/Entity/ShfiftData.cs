using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using  Yuanji.core.src.Driver.Huani.Controller;
using YS.Yuanji.Log;
using YS.Yuanji.Commom;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// M5 当前班次数据
    /// </summary>
    [Serializable]
    [Description("当前班次数据")]
    public struct ShiftData
    {
        [Description("班次编号")]
        public uint ShiftNo;               // 班次编号

        [Description("当前班次开始时间")]
        public uint ShiftStart;            // 当前班次开始时间

        //[Description("班次总时长(秒)")]
        //public uint ShiftTime;             // 班次总时长（秒）

        //public uint BreakTime;             // 休息时间（秒）
        [Description("运行时间（秒）")]
        public uint RunTime;               // 运行时间（秒）

        [Description("停机时间（秒）")]
        public uint StopTime;              // 停机时间（秒）

        [Description("停机次数")]
        public uint StopCnt;               // 停机次数

        [Description("基于时间的机器效率")]
        public float EffMachineT;          // 基于时间的机器效率

        [Description("基于时间的生产效率")]
        public float EffProductionT;       // 基于时间的生产效率

        [Description("基于生产的机器效率")]
        public float EffMachineP;          // 基于生产的机器效率

        [Description("基于生产的生产效率")]
        public float EffProductionP;       // 基于生产的生产效率

        [Description("总废料数量(支)")]
        public uint TotalWasteCount;       // 总废料数量 [Cig]

        [Description("总废料百分比(%)")]
        public float TotalWastePercent;    // 总废料百分比 [%]
        //public uint ExternalWasteCount;    // 外部废料数量，例如每次启动浪费 200 个
        //public uint ExternalLossCount;     // 生产损失（外部原因）
        //public uint SpeedReductionLossCount;  // 因速度降低导致的生产损失
        //public uint ForeignBodyWasteCount; // 因检测到异物而造成的废料
        //public uint InternalWasteCount;    // 内部废料数量 = 总废料 - 外部废料 - 异物废料

        [Description("总生产数量(支)")]
        public uint ProductionTotal;

        [Description("烟丝消耗数量(公斤)")]
        public float TobaccoUsed;// 烟丝消耗

        [Description("滤嘴使用")]
        public int FilterSheetsUsed;//

        [Description("滤嘴使用inUnitCig")]
        public int FilterPlugs;//

        [Description("Max水松纸消耗盘")]
        public int MaxUsedBobbins;//

        [Description("Se卷烟纸消耗盘")]
        public int SeUsedBobbins;//
    }

    /// <summary>
    /// M5 当前剔除数据
    /// </summary>
    [Description("当前剔除数据")]
    public struct ShiftRejectDate
    {
        [Description("前道烟支太轻")]
        public uint LightWeightFront;

        [Description("后道烟支太轻")]
        public uint LightWeightRear;

        [Description("前道烟支太重")]
        public uint HeavyWeightFront;

        [Description("后道烟支太重")]
        public uint HeavyWeightRear;

        [Description("前道硬点")]
        public uint HardSpotFrontCnt;

        [Description("后道硬点")]
        public uint HardSpotRearCnt;

        [Description("前道软点")]
        public uint SoftSpotFrontCnt;

        [Description("后道软点")]
        public uint SoftSpotRearCnt;

        [Description("前道轻烟端")]
        public uint LightEndsFrontCnt;

        [Description("后道轻烟端")]
        public uint LightEndsRearCnt;

        [Description("前道密封度")]
        public uint AirtightnessFrontCnt;

        [Description("后道密封度")]
        public uint AirtightnessRearCnt;

        [Description("前道透气度")]
        public uint TotalVentilationFrontCnt;

        [Description("后道透气度")]
        public uint TotalVentilationRearCnt;

        [Description("前道吸阻")]
        public uint PressureDropFrontCnt;

        [Description("后道吸阻")]
        public uint PressureDropRearCnt;

        [Description("前道松头")]
        public uint FilterFaultFrontCnt;

        [Description("后道松头")]
        public uint FilterFaultRearCnt;

        [Description("前道形状")]
        public uint CigaretteShapeFrontCnt;

        [Description("后道形状")]
        public uint CigaretteShapeRearCnt;

        [Description("烟纸接缝总废品数量")]
        public uint CigPaperSpliceTotalCnt;

        [Description("封口接缝总废品数量")]
        public uint TippingSpliceTotalCnt;

        [Description("缺少滤嘴总数量")]
        public uint MissingFilterTotalCnt;

        [Description("无胶废品总数量")]
        public uint NoGlueTotalCnt;

        [Description("封口废品总数量")]
        public uint TippingTotalCnt;

        [Description("纸张切割废品总数量")]
        public uint PaperCutTotalCnt;

        [Description("前道胶水故障")]
        public uint GlueTippingPaperFrontCnt;

        [Description("后道胶水故障")]
        public uint GlueTippingPaperRearCnt;
    }

    /// <summary>
    /// 设备参数.
    /// </summary>
    [Description("设备参数")]
    public struct DeviceParameterData
    {
        [Description("车速")]
        public uint MachineSpeed;

        [Description("水松纸温度")]
        public uint Temperature4;

        [Description("搓板温度")]
        public uint Temperature5;
    }

    /// <summary>
    /// 温度参数.
    /// </summary>
    [Description("温度参数")]
    public struct DeviceParameterTempData
    {
        [Description("封口器生产温度")]
        public uint Temperature1;
        [Description("封口器 1 生产温度")]
        public uint Temperature2;
        [Description("烟枪温度")]
        public uint Temperature3;
    }


    public class M5ShiftDataParser
    {
        public static Dictionary<string, object> M5ParseTotalShiftData(byte[] response)
        {
            Dictionary<string, object> datas = new Dictionary<string, object>();
            int offset = HuaniConst.M5ReadDataOffset;
            try
            {
                for (int i = 0; i < M5Const.TotalShiftDataArray.Length; i++)
                {
                    if (M5Const.TotalShiftDataArray[i].Contains("Date") || M5Const.TotalShiftDataArray[i].Contains("ShiftStart"))//日期
                    {
                        var res = BitConverter.ToUInt32(response, offset);
                        var date = VisuHelper.M5Uint32ToDateTime(res);
                        datas.Add(M5Const.TotalShiftDataArray[i], date);
                        offset += 74;
                    }
                    else if (M5Const.TotalShiftDataArray[i].Contains("Name"))//字符串类型
                    {
                        datas.Add(M5Const.TotalShiftDataArray[i], Encoding.ASCII.GetString(response, offset, 40).Replace("\0", ""));
                        offset += 110;
                    }

                    else if (M5Const.TotalShiftDataArray[i].Contains("Eff") || M5Const.TotalShiftDataArray[i].Contains("Pct")
                        || M5Const.TotalShiftDataArray[i].Contains("Quality.") || M5Const.TotalShiftDataArray[i].Contains("TobaccoUsed"))
                    {
                        datas.Add(M5Const.TotalShiftDataArray[i], BitConverter.ToSingle(response, offset));//浮点型数据
                        offset += 74;
                    }

                    else
                    {
                        datas.Add(M5Const.TotalShiftDataArray[i], BitConverter.ToUInt32(response, offset));
                        offset += 74;
                    }
                }
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"in M5ShiftDataParser.M5ParseTotalShiftData occur exception:{ex},  response: {BitConverter.ToString(response)}");
            }
            return datas;
        }

        public static ShiftData M5ProductDataParse(byte[] response)
        {
            var result = new ShiftData();
            int offset = HuaniConst.M5ReadDataOffset;
            // 手动解析每个字段并设置值
            result.ShiftNo = BitConverter.ToUInt32(response, offset); offset += 74;
            result.ShiftStart = BitConverter.ToUInt32(response, offset); offset += 74;
            result.RunTime = BitConverter.ToUInt32(response, offset); offset += 74;
            result.StopTime = BitConverter.ToUInt32(response, offset); offset += 74;
            result.StopCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.EffMachineT = BitConverter.ToSingle(response, offset); offset += 74;
            result.EffProductionT = BitConverter.ToSingle(response, offset); offset += 74;
            result.EffMachineP = BitConverter.ToSingle(response, offset); offset += 74;
            result.EffProductionP = BitConverter.ToSingle(response, offset); offset += 74;
            result.TotalWasteCount = BitConverter.ToUInt32(response, offset); offset += 74;
            result.TotalWastePercent = BitConverter.ToSingle(response, offset); offset += 74;
            result.ProductionTotal = BitConverter.ToUInt32(response, offset); offset += 74;
            result.TobaccoUsed = BitConverter.ToSingle(response, offset); offset += 74;
            result.FilterSheetsUsed = BitConverter.ToInt32(response, offset); offset += 74;
            result.MaxUsedBobbins = BitConverter.ToInt32(response, offset); offset += 74;
            result.SeUsedBobbins = BitConverter.ToInt32(response, offset); offset += 74;
            return result;
        }

        public static ShiftRejectDate M5RejectDataParse(byte[] response, int offset = HuaniConst.M5ReadDataOffset)
        {
            ShiftRejectDate result = new ShiftRejectDate();
            //foreach (var field in typeof(ShiftRejectDate).GetFields())
            //{
            //    if (field.FieldType == typeof(uint))
            //    {
            //        uint value = BitConverter.ToUInt32(response, offset);
            //        field.SetValueDirect(__makeref(result), value);
            //        offset += 74;
            //    }
            //}
            result.LightWeightFront = BitConverter.ToUInt32(response, offset); offset += 74;
            result.LightWeightRear = BitConverter.ToUInt32(response, offset); offset += 74;

            result.HeavyWeightFront = BitConverter.ToUInt32(response, offset); offset += 74;
            result.HeavyWeightRear = BitConverter.ToUInt32(response, offset); offset += 74;

            result.HardSpotFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.HardSpotRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.SoftSpotFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.SoftSpotRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.LightEndsFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.LightEndsRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.AirtightnessFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.AirtightnessRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.TotalVentilationFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.TotalVentilationRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.PressureDropFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.PressureDropRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.FilterFaultFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.FilterFaultRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.CigaretteShapeFrontCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.CigaretteShapeRearCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.CigPaperSpliceTotalCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.TippingSpliceTotalCnt = BitConverter.ToUInt32(response, offset); offset += 74;
            result.MissingFilterTotalCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            result.PaperCutTotalCnt = BitConverter.ToUInt32(response, offset); offset += 74;

            return result;
        }

        
        public static DeviceParameterData DeviceParameterData(byte[] response, int offset = HuaniConst.M5ReadDataOffset)
        {
            DeviceParameterData deviceParameter = new DeviceParameterData();
            deviceParameter.MachineSpeed = BitConverter.ToUInt16(response, offset);

            return deviceParameter;
        }

        public static DeviceParameterTempData DeviceParameteTemprData(byte[] response, int offset = HuaniConst.M5ReadDataOffset)
        {
            DeviceParameterTempData deviceParameter = new DeviceParameterTempData();
            deviceParameter.Temperature1 = BitConverter.ToUInt16(response, offset); offset += 2;
            deviceParameter.Temperature2 = BitConverter.ToUInt16(response, offset); offset += 11;
            deviceParameter.Temperature3 = BitConverter.ToUInt16(response, offset); offset += 9;

            return deviceParameter;
        }


        public static DeviceParameterData DeviceParameterData4(byte[] response, int offset = HuaniConst.M5ReadDataOffset)
        {
            DeviceParameterData deviceParameter = new DeviceParameterData();
            deviceParameter.Temperature4 = BitConverter.ToUInt16(response, offset+2);

            return deviceParameter;
        }


        public static DeviceParameterData DeviceParameterData5(byte[] response, int offset = HuaniConst.M5ReadDataOffset)
        {
            DeviceParameterData deviceParameter = new DeviceParameterData();
            deviceParameter.Temperature5 = BitConverter.ToUInt16(response, offset+2);

            return deviceParameter;
        }
    }
}
