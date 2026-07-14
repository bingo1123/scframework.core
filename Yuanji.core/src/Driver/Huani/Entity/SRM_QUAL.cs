using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 4,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SRM质量信息")]
    public struct SRM_QUAL
    {
        [Description("目标重量")]
        public float TargetWeight;         // 目标重量

        [Description("前道平均重量")]
        public float FrontAverageWeight;         // 前道平均重量

        [Description("后道平均重量")]
        public float RearAverageWeight;         // 后道平均重量

        [Description("实际平均重量")]
        public float ActualAverageWeight;         // 实际平均重量

        [Description("前道平均重量偏差")]
        public float FrontAverageWeightDeviation;         // 前道平均重量偏差

        [Description("后道平均重量偏差")]
        public float RearAverageWeightDeviation;         // 后道平均重量偏差

        [Description("平均重量偏差")]
        public float AverageWeightDeviation;         // 平均重量偏差

        [Description("前道短期标准偏差")]
        public float FrontShortTermStandardDeviation;         // 前道短期标准偏差

        [Description("后道短期标准偏差")]
        public float RearShortTermStandardDeviation;         // 后道短期标准偏差

        [Description("平均短期标准偏差")]
        public float AverageShortTermStandardDeviation;         // 平均短期标准偏差

        [Description("前道长期标准偏差")]
        public float FrontLongTermStandardDeviation;         // 前道长期标准偏差

        [Description("后道长期标准偏差")]
        public float RearLongTermStandardDeviation;         // 后道长期标准偏差

        [Description("平均长期标准偏差")]
        public float AverageLongTermStandardDeviation;         // 平均长期标准偏差

        [Description("前道端头压实位置")]
        public float FrontEndCompactionPosition;         // 前道端头压实位置

        [Description("后道端头压实位置")]
        public float RearEndCompactionPosition;         // 后道端头压实位置

        [Description("端头压实位置")]
        public float EndCompactionPosition;         // 端头压实位置

        [Description("前道端头压实量度")]
        public float FrontEndCompactionMeasure;         // 前道端头压实量度

        [Description("后道端头压实量度")]
        public float RearEndCompactionMeasure;         // 后道端头压实量度

        [Description("端头压实量度")]
        public float EndCompactionMeasure;         // 端头压实量度
    }
}
