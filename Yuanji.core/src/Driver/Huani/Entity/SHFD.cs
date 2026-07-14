using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    ///<summary>
    /// 此结构只适用于ZJ19<br/>
    /// 前三班分别保存在SHF1、SHF2、SHF3中
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Description("当前班的班次统计数据")]
    public struct SHFD
    {
        [Description("MLP Actual Date")]
        public uint Date;                       // 000: MLP Actual Date

        [Description("品牌号")]
        public ushort Brand;                    // 004: 牌号

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_BRND_NAMLEN + 1)]
        [Description("品牌名称")]
        public string BrandName;                // 006: 牌号名称(40的Length)

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_MACH_NAMLEN + 1)]
        [Description("机器名")]
        public string UnitName;                 // 047: 机器名 (40的Length)

        [Description("班次开始时间")]
        public uint ShiftStart;                 // 088: 班次开始时间

        [Description("班次持续时长")]
        public uint ShiftTime;                  // 092: 班次持续时长

        [Description("休息时长")]
        public uint BreakTime;                  // 094: 休息时长

        [Description("MLP运行时间")]
        public uint RunTime;                    // 100: MLP Runtime

        [Description("停机时长")]
        public uint StopTime;                   // 104: 停机时长

        [Description("总产量")]
        public uint TotalProduction;            // 108: 总产量

        [Description("总废品量")]
        public uint TotalWasteCnt;              // 112: 总废品量

        [Description("总废品率")]
        public float TotalWastePct;             // 116: 总废品率

        [Description("停机次数")]
        public uint Stops;                      // 120: 停机次数

        [Description("机器效率")]
        public float EffMachine;                // 124: 机器效率

        [Description("生产效率")]
        public float EffProduction;             // 128: 生产效率

        [Description("ITL_Efficiency")]
        public float ITL_Efficiency;            // 132: ITL Efficiency

        [Description("滤嘴消耗计数")]
        public uint MLP_FilterCount;            // 136: 滤嘴消耗计数

        [Description("SE盘纸消耗(盘)")]
        public uint MLP_SEBOB;                  // 140: SE盘纸消耗(盘)

        [Description("MAX水松纸消耗(盘)")]
        public uint MLP_MAXBOB;                 // 144: MAX水松纸消耗（盘）

        [Description("胶水消耗")]
        public uint MLP_MAX_Glue;               // 148: 胶水消耗

        [Description("SE胶水消耗")]
        public uint MLP_SE_Glue;                // 152: SE胶水消耗

        [Description("VE烟丝消耗")]
        public uint MLP_VE_Tobacco;             // 156: VE烟丝消耗

        [Description("气耗")]
        public uint MLP_Air_Count;              // 160: 气耗

        [Description("电耗")]
        public uint MLP_CurrentDrain;           // 164: 电耗

        [Description("MLP 备用")]
        public uint MLP_Dummy010;               // 168: MLP 备用

        // Front
        [Description("前道平均重量")]
        public float AvgCigWeight_f;            // 172: 前道平均重量

        [Description("前道短期标准偏差")]
        public float ShortStdAv_f;              // 176: 前道短期标准偏差

        [Description("前道长期标准偏差")]
        public float LongStdAv_f;               // 180: 前道长期标准偏差

        [Description("前道过轻废品数")]
        public uint LightWeightCnt_f;           // 184: 前道过轻废品数

        [Description("前道过轻废品数")]
        public float LightWeightPct_f;          // 188: 前道过轻废品率

        [Description("前道过重废品数")]
        public uint HeavyWeightCnt_f;           // 192: 前道过重废品数

        [Description("前道过重废品率")]
        public float HeavyWeightPct_f;          // 196: 前道过重废品率

        [Description("前道软点")]
        public uint SoftSpotsCnt_f;             // 200: 前道软点

        [Description("前道软点废品率")]
        public float SoftSpotsPct_f;            // 204: 前道软点废品率

        [Description("前道硬点")]
        public uint HardSpotsCnt_f;             // 208: 前道硬点

        [Description("前道硬点废品率")]
        public float HardSpotsPct_f;            // 212: 前道硬点废品率

        [Description("前道轻烟端")]
        public uint LightEndsCnt_f;             // 216: 前道轻烟端

        [Description("前道轻烟端废品率")]
        public float LightEndsPct_f;            // 220: 前道轻烟端废品率

        [Description("前道异物剔除")]
        public uint ForeignBodyCnt_f;           // 224: 前道异物剔除

        [Description("前道异物剔除废品率")]
        public float ForeignBodyPct_f;          // 228: 前道异物剔除废品率

        [Description("前道压实端位置平均值")]
        public float DensedPosAvg_f;            // 232: 前道压实端位置平均值

        [Description("前道压实端量度")]
        public float DensedAmountAvg_f;         // 236: 前道压实端量度

        [Description("前道钢印偏移")]
        public uint ORIS_PaintOffsetCnt_f;      // 240: 前道钢印偏移

        [Description("前道钢印偏移废品率")]
        public float ORIS_PaintOffsetPct_f;     // 244: 前道钢印偏移废品率

        [Description("SRM备用7")]
        public uint SRM_Dummy007_f;             // 248: SRM 备用

        [Description("SRM备用8")]
        public uint SRM_Dummy008_f;             // 252: SRM 备用

        [Description("SRM备用9")]
        public uint SRM_Dummy009_f;             // 256: SRM 备用

        [Description("烟丝水分")]
        public uint Tobacco_moisture;           // 260: 烟丝水分

        [Description("前道空头")]
        public uint LESCnt_f;                   // 264: 前道空头

        [Description("前道空头废品率")]
        public float LESPct_f;                  // 268: 前道空头废品率

        [Description("前道烟支外观")]
        public uint OTISCnt_f;                  // 272: 前道烟支外观

        [Description("前道烟支外观废品率")]
        public float OTISPct_f;                 // 276: 前道烟支外观废品率

        [Description("前道漏气")]
        public uint AirtightnessCnt_f;          // 280: 前道漏气

        [Description("前道漏气废品率")]
        public float AirtightnessPct_f;         // 284: 前道漏气废品率

        [Description("前道通风度")]
        public uint TotalVentilationCnt_f;      // 288: 前道通风度

        [Description("前道通风度废品率")]
        public float TotalVentilationPct_f;    // 292: 前道通风度废品率

        [Description("前道吸阻")]
        public uint PressureDropCnt_f;          // 296: 前道吸阻

        [Description("前道吸阻废品率")]
        public float PressureDropPct_f;        // 300: 前道吸阻废品率

        [Description("CIS_FIS:[支]")]
        public uint FISCnt_f;                   // 304: CIS FIS [cnt]

        [Description("CIS_FIS:[%]")]
        public float FISPct_f;                  // 308: CIS FIS [%]

        [Description("CIS 备用")]
        public uint AirtightnessCnt_Rod_f;

        [Description("前道通风度均值")]
        public float Ventilation_avg_f;

        [Description("前道通风度短偏")]
        public float Ventilation_Short_std_f;

        [Description("CIS—备用")]
        public float Airtightness_avg_f;

        [Description("328: CIS 备用")]
        public float Airtightness_Short_std_f;

        [Description("332: CIS 备用")]
        public float PressureDrop_avg_f;

        [Description("336: CIS 备用")]
        public float PressureDrop_Short_std_f;

        [Description("340: CIS 备用")]
        public uint CIS_Dummy008_f;

        [Description("344: CIS 备用")]
        public uint CIS_Dummy009_f;

        [Description("348: CIS 备用")]
        public uint CIS_Dummy010_f;

        [Description("前道ORIS废品")]
        public uint ORISCnt_f;

        [Description("前道ORIS废品率")]
        public float ORISPct_f;

        [Description("前道ORIS暗污点")]
        public uint ORIS_DarkPointCnt_f;

        [Description("前道ORIS暗污点比例")]
        public float ORIS_DarkPointPct_f;

        [Description("前道ORIS亮污点")]
        public uint ORIS_LightPointCnt_f;

        [Description("前道ORIS亮污点比例")]
        public float ORIS_LightPointPct_f;

        [Description("前道ORIS暗钢印")]
        public uint ORIS_DarkPaintCnt_f;

        [Description("前道ORIS暗钢印比例")]
        public float ORIS_DarkPaintPct_f;

        [Description("前道ORIS亮钢印")]
        public uint ORIS_LightPaintCnt_f;

        [Description("前道ORIS亮钢印比例")]
        public float ORIS_LightPaintPct_f;

        [Description("前道圆周均值")]
        public float Rod_diameter_f;

        [Description("前道圆周偏差")]
        public float Short_Rod_diameter_f;

        [Description("400: AWS METIS [cnt]  no used")]
        public uint METISCnt_f;

        [Description("404: AWS MEIIS [%]  no used")]
        public float METISPct_f;

        [Description("前道缺少滤嘴")]
        public uint MissingFilterCnt_f;

        [Description("前道缺少滤嘴废品率")]
        public float MissingFilterPct_f;

        [Description("前道无胶水")]
        public uint NoGlueCnt_f;

        [Description("前道无胶水废品率")]
        public float NoGluePct_f;

        [Description("424: AWS Tipping [cnt] no used")]
        public uint TippingCnt_f;

        [Description("428: AWS Tipping [%] no used")]
        public float TippingPct_f;

        [Description("432: AWS 备用 no used")]
        public uint RollingCnt_f;

        [Description("436: AWS 备用 no used")]
        public uint RollingPct_f;

        [Description("440: AWS 备用 no used")]
        public uint AWS_Sensor1;

        [Description("444: AWS 备用 no used")]
        public uint AWS_Sensor2;

        [Description("448: AWS 备用 no used")]
        public uint AWS_Sensor3;

        [Description("452: AWS 备用 no used")]
        public uint AWS_Sensor4;

        [Description("456: AWS 备用 no used")]
        public uint AWS_Sensor5;

        [Description("460: AWS 备用 no used")]
        public uint AWS_Sensor6;

        [Description("464: AWS 备用")]
        public uint Dummy002_f;

        [Description("468: AWS 备用")]
        public uint Dummy003_f;

        [Description("472: AWS 备用")]
        public uint Dummy004_f;

        [Description("476: AWS 备用")]
        public uint Dummy005_f;

        [Description("480: AWS 备用")]
        public uint Dummy006_f;

        [Description("484: AWS 备用")]
        public uint Dummy007_f;

        [Description("SE盘纸消耗(米)")]
        public uint MLP_SEBOBmi;

        [Description("MAX水松纸消耗（米）")]
        public uint MLP_MAXBOBmi;

        [Description("后道平均重量")]
        public float AvgCigWeight_r;

        [Description("后道短期标准偏差")]
        public float ShortStdAv_r;

        [Description("后道长期标准偏差")]
        public float LongStdAv_r;

        [Description("后道过轻")]
        public uint LightWeightCnt_r;

        [Description("后道过轻比例")]
        public float LightWeightPct_r;

        [Description("后道过重")]
        public uint HeavyWeightCnt_r;

        [Description("后道过重比例")]
        public float HeavyWeightPct_r;

        [Description("后道软点")]
        public uint SoftSpotsCnt_r;

        [Description("后道软点比例")]
        public float SoftSpotsPct_r;

        [Description("后道硬点")]
        public uint HardSpotsCnt_r;

        [Description("后道硬点比例")]
        public float HardSpotsPct_r;

        [Description("后道轻烟端")]
        public uint LightEndsCnt_r;

        [Description("后道轻烟端比例")]
        public float LightEndsPct_r;

        [Description("后道异物")]
        public uint ForeignBodyCnt_r;

        [Description("后道异物比例")]
        public float ForeignBodyPct_r;

        [Description("后道压实端位置")]
        public float DensedPosAvg_r;

        [Description("后道压实端量度")]
        public float DensedAmountAvg_r;

        [Description("后道ORIS钢印偏移")]
        public uint ORIS_PaintOffsetCnt_r;

        [Description("后道ORIS钢印偏移比例")]
        public float ORIS_PaintOffsetPct_r;

        [Description("572// 248: SRM 备用")]
        public uint SRM_Dummy007_r;

        [Description("576// 252: SRM 备用")]
        public uint SRM_Dummy008_r;

        [Description("580 SRM 备用")]
        public uint SRM_Dummy009_r;

        [Description("584 SRM 备用")]
        public uint SRM_Dummy010_r;

        [Description("后道空头")]
        public uint LESCnt_r;

        [Description("后道空头比例")]
        public float LESPct_r;

        [Description("后道烟支外观")]
        public uint OTISCnt_r;

        [Description("后道烟支外观比例")]
        public float OTISPct_r;

        [Description("后道漏气")]
        public uint AirtightnessCnt_r;

        [Description("后道漏气比例")]
        public float AirtightnessPct_r;

        [Description("后道通风度")]
        public uint TotalVentilationCnt_r;

        [Description("后道通风度比例")]
        public float TotalVentilationPct_r;

        [Description("后道吸阻")]
        public uint PressureDropCnt_r;

        [Description("后道吸阻比例")]
        public float PressureDropPct_r;

        [Description("628 FISCnt_r")]
        public uint FISCnt_r;

        [Description("632 FISPct_r")]
        public float FISPct_r;

        [Description("636// 312: CIS 备用")]
        public uint AirtightnessCnt_Rod_r;

        [Description("前道通风度均值")]
        public float Ventilation_avg_r;

        [Description("备用前道通风度短偏")]
        public float Ventilation_Short_std_r;

        [Description("648 CIS 备用")]
        public float Airtightness_avg_r;

        [Description("652 CIS 备用")]
        public float Airtightness_Short_std_r;

        [Description("656 CIS 备用")]
        public float PressureDrop_avg_r;

        [Description("660 CIS 备用")]
        public float PressureDrop_Short_std_r;

        [Description("664 CIS 备用")]
        public uint CIS_Dummy008_r;

        [Description("668 CIS 备用")]
        public uint CIS_Dummy009_r;

        [Description("672 CIS 备用")]
        public uint CIS_Dummy010_r;

        [Description("后道ORIS废品")]
        public uint ORISCnt_r;

        [Description("后道ORIS废品率")]
        public float ORISPct_r;

        [Description("ORIS 后道暗污点")]
        public uint ORIS_DarkPointCnt_r;

        [Description("ORIS 后道暗污点")]
        public float ORIS_DarkPoint;

        [Description("后道ORIS亮污点")]
        public uint ORIS_LightPointCnt_r;               // 692: ORIS 后道亮污点

        [Description("后道ORIS亮污点")]
        public float ORIS_LightPointPct_r;               // 696: ORIS 后道亮污点

        [Description("后道ORIS暗钢印")]
        public uint ORIS_DarkPaintCnt_r;               // 700: ORIS 后道暗钢印

        [Description("后道ORIS暗钢印")]
        public float ORIS_DarkPaintPct_r;               // 704: ORIS 后道暗钢印

        [Description("后道ORIS亮钢印")]
        public uint ORIS_LightPaintCnt_r;               // 708: ORIS 后道亮钢印

        [Description("后道ORIS亮钢印")]
        public float ORIS_LightPaintPct_r;               // 712: ORIS 后道亮钢印

        [Description("后道圆周均值")]
        public float Rod_diameter_r;                    // 716: 后道圆周均值

        [Description("后道圆周偏差")]
        public float Short_Rod_diameter_r;               // 720: 后道圆周偏差

        [Description("AWS METIS [cnt] no used")]
        public uint METISCnt_r;                    // 724: AWS METIS [cnt] no used

        [Description("AWS MEIIS [%] no used")]
        public float METISPct_r;                    // 728: AWS MEIIS [%] no used

        [Description("后道缺嘴")]
        public uint MissingFilterCnt_r;            // 732: 后道缺嘴

        [Description("后道缺嘴比例")]
        public float MissingFilterPct_r;            // 736: 后道缺嘴比例

        [Description("后道无胶水")]
        public uint NoGlueCnt_r;                   // 740: 后道无胶水

        [Description("后道无胶水比例")]
        public float NoGluePct_r;                   // 744: 后道无胶水比例

        [Description("AWS Tipping [cnt] no used")]
        public uint TippingCnt_r;                  // 748: AWS Tipping [cnt] no used

        [Description("AWS Tipping [%] no used")]
        public float TippingPct_r;                  // 752: AWS Tipping [%] no used

        [Description("AWS 备用")]
        public uint AWS_Dummy005_r;                // 756: AWS 备用

        [Description("AWS 备用")]
        public uint AWS_Dummy006_r;                // 760: AWS 备用

        [Description("AWS 备用")]
        public uint AWS_Dummy007_r;                // 764: AWS 备用

        [Description("AWS 备用")]
        public uint AWS_Dummy008_r;                // 768: AWS 备用

        [Description("AWS 备用")]
        public uint AWS_Dummy009_r;                // 772: AWS 备用

        [Description("AWS 备用")]
        public uint AWS_Dummy010_r;                // 776: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy000_r;                    // 780: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy001_r;                    // 784: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy002_r;                    // 788: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy003_r;                    // 792: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy004_r;                    // 796: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy005_r;                    // 800: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy006_r;                    // 804: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy007_r;                    // 808: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy008_r;                    // 812: AWS 备用

        [Description("AWS 备用")]
        public uint Dummy009_r;                    // 816: AWS 备用

        [Description("平均车速")]
        public uint Avg_Speed;                     // 820: Avg Speed cig / min over shift

        [Description("烟草消耗量")]
        public uint Tobacco_Cons;                  // 824: Tobacco consumption

        [Description("纸张消耗量")]
        public uint Paper_Cons;                    // 828: Paper consumption

        [Description("线轴数量")]
        public uint Bobbin_Count;                  // 832: Bobbin count

        [Description("SE 轴杆前断裂")]
        public uint SERodBreakFront;               // 836: SE rod break front

        [Description("SE 轴杆前断裂机器启动")]
        public uint SERodBrkFrStart;               // 840: SE rod break front machine start

        [Description("SE 轴杆前断裂换线轴")]
        public uint SERodBrkFrBobChng;             // 844: SE rod break front bobbin change

        [Description("SE 轴杆前断裂换线轴次数")]
        public uint SERodBrkFrBobCnt;              // 848: SE rod break front bobbin swivel

        [Description("SE 轴杆前断裂机器加速")]
        public uint SERodBrkFrAcc;                 // 852: SE rod break front machine accelerates

        [Description("SE 轴杆后断裂")]
        public uint SERodBreakRear;                // 856: SE rod break rear

        [Description("SE 轴杆后断裂机器启动")]
        public uint SERodBrkReStart;               // 860: SE rod break rear machine start

        [Description("SE 轴杆后断裂换线轴")]
        public uint SERodBrkReBobChng;             // 864: SE rod break rear bobbin change

        [Description("SE 轴杆后断裂换线轴次数")]
        public uint SERodBrkReBobCnt;              // 868: SE rod break rear bobbin swivel

        [Description("SE 轴杆后断裂机器加速")]
        public uint SERodBrkReAcc;                 // 872: SE rod break rear machine accelerates

        [Description("SE 纸张前断裂")]
        public uint SEPaperBrkFront;               // 876: SE paper break front

        [Description("SE 纸张前断裂机器启动")]
        public uint SEPaperBrkFrStart;             // 880: SE paper break front machine start

        [Description("SE 纸张前断裂换线轴")]
        public uint SEPaperBrkFrBobChng;           // 884: SE paper break front bobbin change

        [Description("SE 纸张前断裂换线轴次数")]
        public uint SEPaperBrkFrBobCnt;            // 888: SE paper break front bobbin swivel

        [Description("SE 纸张前断裂机器加速")]
        public uint SEPaperBrkFrAcc;               // 892: SE paper break front machine accelerates

        [Description("SE 纸张后断裂")]
        public uint SEPaperBrkRear;                // 896: SE paper break rear

        [Description("SE 纸张后断裂机器启动")]
        public uint SEPaperBrkReStart;             // 900: SE paper break rear machine start

        [Description("SE 纸张后断裂换线轴")]
        public uint SEPaperBrkReBobChng;           // 904: SE paper break rear bobbin change

        [Description("SE 纸张后断裂换线轴次数")]
        public uint SEPaperBrkReBobCnt;            // 908: SE paper break rear bobbin swivel

        [Description("SE 纸张后断裂机器加速")]
        public uint SEPaperBrkReAcc;         // 912: SE paper break rear machine accelerates

        [Description("SRM 目标重量")]
        public float SRM_Target;                    // 916: SRM target weight

        [Description("最大拼接次数")]
        public uint MAX_Splice;                   // 920: MAX splice

        [Description("最大倾斜拼接次数")]
        public uint Tip_Splice_Count;              // 924: MAX tipping splice count

        [Description("最大过滤器消耗量")]
        public uint Filter_Cons;                   // 928: MAX Filter consumption

        [Description("最大线轴数量")]
        public uint Max_Bobbin_Count;              // 932: MAX bobbin count

        [Description("最大卷纸器断裂")]
        public uint MAXPaperBrk;                   // 936: MAX paper break at curler

        [Description("最大卷纸器断裂机器启动")]
        public uint MAXPaperBrkStart;              // 940: MAX paper break at curler

        [Description("最大卷纸器断裂换线轴")]
        public uint MAXPaperBrkBobChng;            // 944: MAX paper break at curler

        [Description("最大卷纸器断裂换线轴次数")]
        public uint MAXPaperBrkBobCnt;             // 948: MAX paper break at curler

        [Description("最大加热器断裂")]
        public uint MAXPaperBrkHeater;             // 952: MAX paper break at heater

        [Description("最大加热器断裂机器启动")]
        public uint MAXPaperBrkHeatStart;          // 956: MAX paper break at heater

        [Description("最大加热器断裂换线轴")]
        public uint MAXPaperBrkHeatBobChng;        // 960: MAX paper break at heater

        [Description("最大加热器断裂换线轴次数")]
        public uint MAXPaperBrkHeatBobCnt;         // 964: MAX paper break at heater

        [Description("最大滚筒堵塞")]
        public uint MAXRollBlockJam;               // 968: MAX roll block jam

        [Description("最大滚筒堵塞机器启动")]
        public uint MAXRollBlockJamStart;          // 972: MAX roll block jam machine start

        [Description("最大滚筒堵塞拼接后")]
        public uint MAXRollBlockJamSlice;          // 976: MAX roll block jam after splice

        [Description("最大堆叠形成器堵塞")]
        public uint MAXJamTransfer;                // 980: MAX jam at stackformer

        [Description("最大堆叠形成器堵塞样本后")]
        public uint MAXJamRejection;           // 984: MAX jam at stackformer after sampling

        [Description("SE 切割废料")]
        public uint CutWaste;                      // 988: SE cut waste

        [Description("SE 拼接")]
        public uint SE_Splice;                   // 992: SE splice

        [Description("SE 开缝废料")]
        public uint OpenSeamWaste;                 // 996: SE open seam waste

        [Description("SE 闭缝废料")]
        public uint ClosedSeamWaste;               // 1000: SE closed seam waste

        [Description("最大滚筒阀门")]
        public uint RollDrumValve;                 // 1004: MAX rolldrum valve

        [Description("最大废料前轨")]
        public uint WasteFrontTrack;               // 1008: MAX waste front track

        [Description("最大废料后轨")]
        public uint WasteRearTrack;                // 1012: MAX waste rear track

        [Description("最大前排废料")]
        public uint EjectFront;                    // 1016: MAX ejected front

        [Description("最大后排废料")]
        public uint EjectRear;                     // 1020: MAX ejected rear

        [Description("缺少过滤器")]
        public uint MissingFilter;                 // 1024: Missing filter

        [Description("倾斜拼接附加废料")]
        public uint TippSpliceAddWaste;            // 1028: Tipping splice additional waste

        [Description("最大弹出废料")]
        public uint EjectWaste;                    // 1032: MAX eject waste

        [Description("最大人工废料")]
        public uint ManualWaste;                   // 1036: MAX manual waste

        [Description("最大外部废料")]
        public uint ExternalWaste;                 // 1040: MAX external waste

        [Description("最大报废香烟")]
        public uint RejectedCigarettes;            // 1044: MAX rejected cigarettes

        [Description("LE/O 密度轨道前")]
        public uint DensityCnt_Track_f;            // 1048: LE/O density track front

        [Description("LE/O 密度轨道后")]
        public uint DensityCnt_Track_r;            // 1052: LE/O density track rear

        [Description("LE/O 密度杆前")]
        public uint DensityCnt_Rod_f;              // 1056: LE/O density rod front

        [Description("LE/O 密度杆后")]
        public uint DensityCnt_Rod_r;              // 1060: LE/O density rod rear

        // These two constants should be declared based on the maximum length of BrandName and UnitName
        private const int MAX_BRND_NAMLEN = 40; // Adjust based on actual size
        private const int MAX_MACH_NAMLEN = 40; // Adjust based on actual size
    }

  
}
