using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 7,func:0--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("ODMH后道圆周信息")]
    public struct ODM_RDAT
    {
        [Description("后道目标圆周")]
        public float ODM_TargetCycle;

        [Description("后道当前圆周均值")]
        public float ODM_Meanvalue;

        [Description("后道当前圆周偏差")]
        public float ODM_Deviation;

        [Description("后道电机启动位置")]
        public ushort ODM_MotorStartPosition;

        [Description("后道电机当前位置")]
        public ushort ODM_MotorCurrentPosition;

        [Description("后道电机已调节角度（相对于启动位置）")]
        public ushort ODM_MotorPositionDev;

        [Description("后道CCD1曲线数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 625)]
        public int[] ODM_CCD1Curve;

        [Description("后道CCD2曲线数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 625)]
        public int[] ODM_CCD2Curve;

        [Description("CCD1状态")]
        public ushort CCD1_Status;

        [Description("CCD1当前值")]
        public ushort CCD1_CurrentVa;

        [Description("CCD1阈值")]
        public ushort CCD1_Threshold;

        [Description("CCD2状态")]
        public ushort CCD2_Status;

        [Description("CCD2当前值")]
        public ushort CCD2_CurrentVal;

        [Description("CCD2阈值")]
        public ushort CCD2_Threshold;

        [Description("后道校准滚轴直径")]
        public int ODM_CaliRODDia;

        [Description("后道偏差属性")]
        public byte ODM_DeviationAttr;

        [Description("后道偏差基础报告值")]
        public float ODM_Deviation_baseReport;

        [Description("后道标准差")]
        public float odm_STDdev;

        [Description("手动tid")]
        public int maunal_tid;
    }
}
