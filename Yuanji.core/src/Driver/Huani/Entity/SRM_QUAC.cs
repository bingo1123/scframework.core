using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 4,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SRM质量平均值信息")]
    public struct SRM_QUAC
    {
        [Description("前道平均重量")]
        public float FrontAverageWeight;         // 前道平均重量

        [Description("后道平均重量")]
        public float RearAverageWeight;         // 后道平均重量

        [Description("机组平均重量")]
        public float UnitAverageWeight;         // 机组平均重量

        [Description("前道重量偏差")]
        public float FrontWeightDeviation;         // 前道重量偏差

        [Description("后道重量偏差")]
        public float RearWeightDeviation;         // 后道重量偏差

        [Description("机组重量偏差")]
        public float UnitWeightDeviation;         // 机组重量偏差

        [Description("前道短期偏差")]
        public float FrontShortTermDeviation;         // 前道短期偏差

        [Description("后道短期偏差")]
        public float RearShortTermDeviation;         // 后道短期偏差

        [Description("机组短期偏差")]
        public float UnitShortTermDeviation;         // 机组短期偏差

        [Description("前道长期偏差")]
        public float FrontLongTermDeviation;         // 前道长期偏差

        [Description("后道长期偏差")]
        public float RearLongTermDeviation;         // 后道长期偏差

        [Description("机组长期偏差")]
        public float UnitLongTermDeviation;         // 机组长期偏差

        [Description("前道压实端位置")]
        public float FrontCompactionEndPosition;         // 前道压实端位置

        [Description("后道压实端位置")]
        public float RearCompactionEndPosition;         // 后道压实端位置

        [Description("机组压实端位置")]
        public float UnitCompactionEndPosition;         // 机组压实端位置

        [Description("前道压实端量度")]
        public float FrontCompactionEndMeasure;         // 前道压实端量度

        [Description("后道压实端量度")]
        public float RearCompactionEndMeasure;         // 后道压实端量度

        [Description("机组压实端量度")]
        public float UnitCompactionEndMeasure;         // 机组压实端量度
    }
}
