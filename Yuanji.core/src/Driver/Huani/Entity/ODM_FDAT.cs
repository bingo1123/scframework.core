using System.ComponentModel;
using System.Runtime.InteropServices;


namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 7,func:0--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("ODM前道圆周信息")]
    public struct ODM_FDAT
    {
        [Description("前道目标圆周")]
        public float ODM_TargetCycle;         // 前道目标圆周

        [Description("前道当前圆周均值")]
        public float ODM_Meanvalue;         // 前道当前圆周均值

        [Description("前道当前圆周偏差")]
        public float ODM_Deviation;         // 前道当前圆周偏差

        [Description("前道电机启动位置")]
        public ushort ODM_MotorStartPosition;         // 前道电机启动位置

        [Description("前道电机当前位置")]
        public ushort ODM_MotorCurrentPosition;         // 前道电机当前位置

        [Description("前道电机已调节角度（相对于启动位置）")]
        public ushort ODM_MotorPositionDev;         // 前道电机已调节角度（相对于启动位置）

        [Description("前道CCD1曲线数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 625)]
        public int[] ODM_CCD1Curve;         // 前道CCD1曲线数据

        [Description("前道CCD2曲线数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 625)]
        public int[] ODM_CCD2Curve;         // 前道CCD2曲线数据

        [Description("CCD1状态")]
        public ushort CCD1_Status;         // CCD1状态

        [Description("CCD1当前值")]
        public ushort CCD1_CurrentVa;         // CCD1当前值

        [Description("CCD1阈值")]
        public ushort CCD1_Threshold;         // CCD1阈值

        [Description("CCD2状态")]
        public ushort CCD2_Status;         // CCD2状态

        [Description("CCD2当前值")]
        public ushort CCD2_CurrentVal;         // CCD2当前值

        [Description("CCD2阈值")]
        public ushort CCD2_Threshold;         // CCD2阈值

        [Description("前道校准滚轴直径")]
        public int ODM_CaliRODDia;         // 前道校准滚轴直径

        [Description("前道偏差属性")]
        public byte ODM_DeviationAttr;         // 前道偏差属性

        [Description("前道偏差基础报告值")]
        public float ODM_Deviation_baseReport;         // 前道偏差基础报告值

        [Description("前道标准差")]
        public float odm_STDdev;         // 前道标准差

        [Description("手动tid")]
        public int maunal_tid;         // 手动tid
    }
}
