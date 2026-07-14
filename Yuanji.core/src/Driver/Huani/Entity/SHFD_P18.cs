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
    /// 此解构只适用于P18<br/>
    /// 前三班分别保存在SHF1、SHF2、SHF3中
    /// </summary>
    [Description("当前班的班次统计数据")]
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SHFD_P18
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

        [Description("MLP_备用001")]
        public uint MLP_Dummy001;

        [Description("MLP_备用002")]
        public uint MLP_Dummy002;

        [Description("MLP_备用003")]
        public uint MLP_Dummy003;

        [Description("MLP_备用004")]
        public uint MLP_Dummy004;

        [Description("MLP_备用005")]
        public uint MLP_Dummy005;

        [Description("MLP_备用006")]
        public uint MLP_Dummy006;

        [Description("MLP_备用007")]
        public uint MLP_Dummy007;

        [Description("MLP_备用008")]
        public uint MLP_Dummy008;

        [Description("MLP_备用009")]
        public uint MLP_Dummy009;

        [Description("MLP_备用010")]
        public uint MLP_Dummy010;

        [Description("SRM平均香烟重量")]
        public float AvgCigWeight;

        [Description("SRM平均短期标准差")]
        public float ShortStdAv;

        [Description("SRM平均组标准差")]
        public float LongStdAv;

        [Description("SRM轻重量香烟数量")]
        public uint LightWeightCnt;

        [Description("SRM轻重量香烟百分比")]
        public float LightWeightPct;

        [Description("SRM重重量香烟数量")]
        public uint HeavyWeightCnt;

        [Description("SRM重重量香烟百分比")]
        public float HeavyWeightPct;

        [Description("有软斑的香烟数量")]
        public uint SoftSpotsCnt;

        [Description("有软斑的香烟百分比")]
        public float SoftSpotsPct;

        [Description("有硬斑的香烟数量")]
        public uint HardSpotsCnt;

        [Description("有硬斑的香烟百分比")]
        public float HardSpotsPct;

        [Description("有轻端部的香烟数量")]
        public uint LightEndsCnt;

        [Description("有轻端部的香烟百分比")]
        public float LightEndsPct;

        // ...（以下字段为备用字段，注释为“SRM备用”或“CIS备用”或“AWS备用”）
        [Description("SRM备用001")]
        public uint SRM_Dummy001;
        [Description("SRM备用002")]
        public uint SRM_Dummy002;
        [Description("SRM备用003")]
        public uint SRM_Dummy003;
        [Description("SRM备用004")]
        public uint SRM_Dummy004;
        [Description("SRM备用005")]
        public uint SRM_Dummy005;
        [Description("SRM备用006")]
        public uint SRM_Dummy006;
        [Description("SRM备用007")]
        public uint SRM_Dummy007;
        [Description("SRM备用008")]
        public uint SRM_Dummy008;
        [Description("SRM备用009")]
        public uint SRM_Dummy009;
        [Description("SRM备用010")]
        public uint SRM_Dummy010;

        [Description("CIS松端数量")]
        public uint LESCnt;

        [Description("CIS松端百分比")]
        public float LESPct;

        [Description("CIS OTIS（形状缺陷）数量")]
        public uint OTISCnt;

        [Description("CIS OTIS（形状缺陷）百分比")]
        public float OTISPct;

        [Description("CIS气密性数量")]
        public uint AirtightnessCnt;

        [Description("CIS气密性百分比")]
        public float AirtightnessPct;

        [Description("CIS总通风量数量")]
        public uint TotalVentilationCnt;

        [Description("CIS总通风量百分比")]
        public float TotalVentilationPct;

        [Description("CIS压降数量")]
        public uint PressureDropCnt;

        [Description("CIS压降百分比")]
        public float PressureDropPct;

        [Description("CIS FIS（炭过滤器故障）数量")]
        public uint FISCnt;

        [Description("CIS FIS（炭过滤器故障）百分比")]
        public float FISPct;

        // ...（以下字段为CIS_Dummy和ORIS_Dummy备用字段，添加相应的Description）
        [Description("CIS备用001")]
        public uint CIS_Dummy001;
        [Description("CIS备用002")]
        public uint CIS_Dummy002;
        [Description("CIS备用003")]
        public uint CIS_Dummy003;
        [Description("CIS备用004")]
        public uint CIS_Dummy004;
        [Description("CIS备用005")]
        public uint CIS_Dummy005;
        [Description("CIS备用006")]
        public uint CIS_Dummy006;
        [Description("CIS备用007")]
        public uint CIS_Dummy007;
        [Description("CIS备用008")]
        public uint CIS_Dummy008;
        [Description("CIS备用009")]
        public uint CIS_Dummy009;
        [Description("CIS备用010")]
        public uint CIS_Dummy010;



        [Description("ORIS（光学香烟缺陷）数量")]
        public uint ORISCnt;

        [Description("ORIS（光学香烟缺陷）百分比")]
        public float ORISPct;


        [Description("ORIS备用001")]
        public uint ORIS_Dummy001;
        [Description("ORIS备用002")]
        public uint ORIS_Dummy002;
        [Description("ORIS备用003")]
        public uint ORIS_Dummy003;
        [Description("ORIS备用004")]
        public uint ORIS_Dummy004;
        [Description("ORIS备用005")]
        public uint ORIS_Dummy005;
        [Description("ORIS备用006")]
        public uint ORIS_Dummy006;
        [Description("ORIS备用007")]
        public uint ORIS_Dummy007;
        [Description("ORIS备用008")]
        public uint ORIS_Dummy008;
        [Description("ORIS备用009")]
        public uint ORIS_Dummy009;
        [Description("ORIS备用010")]
        public uint ORIS_Dummy010;


        [Description("AWS METIS（金属颗粒）数量")]
        public uint METISCnt;

        [Description("AWS METIS（金属颗粒）百分比")]
        public float METISPct;

        [Description("AWS缺失过滤器数量")]
        public uint MissingFilterCnt;

        [Description("AWS缺失过滤器百分比")]
        public float MissingFilterPct;

        [Description("AWS备用001")]
        public uint AWS_Dummy001;
        [Description("AWS备用002")]
        public uint AWS_Dummy002;
        [Description("AWS备用003")]
        public uint AWS_Dummy003;
        [Description("AWS备用004")]
        public uint AWS_Dummy004;
        [Description("AWS备用005")]
        public uint AWS_Dummy005;
        [Description("AWS备用006")]
        public uint AWS_Dummy006;
        [Description("AWS备用007")]
        public uint AWS_Dummy007;
        [Description("AWS备用008")]
        public uint AWS_Dummy008;
        [Description("AWS备用009")]
        public uint AWS_Dummy009;

        [Description("AWS备用010")]
        public uint AWS_Dummy010;

        // These two constants should be declared based on the maximum length of BrandName and UnitName
        private const int MAX_BRND_NAMLEN = 40; // Adjust based on actual size
        private const int MAX_MACH_NAMLEN = 40; // Adjust based on actual size


       
    }

    //public class SHFDP18Parser : ISymbEntityParser<SHFD_P18>
    //{
    //    public SHFD_P18 Parse(byte[] response, HuaniProtocolTypeEnum type = HuaniProtocolTypeEnum.P18)
    //    {
    //        int offset = HuaniConst.P18ReadDataOffset;
    //        int structSize = Marshal.SizeOf(typeof(SHFD_P18));

    //        if (response.Length - offset < structSize)
    //            Log4net.Manager.LogHelper.ErrorLogger.Info($"The response array does not contain enough data starting at offset {offset} to map to SHFD_P18.");
             
    //        IntPtr ptr = Marshal.AllocHGlobal(structSize);

    //        try
    //        {
    //            // Copy the relevant bytes from the response array into the allocated memory
                
    //            Marshal.Copy(response, offset, ptr, Marshal.SizeOf<SHFD_P18>());

    //            // Marshal the memory back into a struct
    //            return Marshal.PtrToStructure<SHFD_P18>(ptr);
    //        }
    //        finally
    //        {
    //            // Free the allocated memory
    //            Marshal.FreeHGlobal(ptr);
    //        }
    //    }
    //}
}
