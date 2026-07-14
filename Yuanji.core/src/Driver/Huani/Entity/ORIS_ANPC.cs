using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 5,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SRM废品统计信息")]
    public struct ORIS_ANPC
    {
        [Description("前道暗污点废品比率")]
        public float FrontDarkStainWasteRatio;         // 前道暗污点废品比率

        [Description("前道亮污点废品比率")]
        public float FrontBrightStainWasteRatio;         // 前道亮污点废品比率

        [Description("前道封口故障废品比率")]
        public float FrontSealingFaultWasteRatio;         // 前道封口故障废品比率

        [Description("前道暗钢印废品比率")]
        public float FrontDarkEmbossingWasteRatio;         // 前道暗钢印废品比率

        [Description("前道亮钢印废品比率")]
        public float FrontBrightEmbossingWasteRatio;         // 前道亮钢印废品比率

        [Description("后道暗污点废品比率")]
        public float RearDarkStainWasteRatio;         // 后道暗污点废品比率

        [Description("后道亮污点废品比率")]
        public float RearBrightStainWasteRatio;         // 后道亮污点废品比率

        [Description("后道封口故障废品比率")]
        public float RearSealingFaultWasteRatio;         // 后道封口故障废品比率

        [Description("后道暗钢印废品比率")]
        public float RearDarkEmbossingWasteRatio;         // 后道暗钢印废品比率

        [Description("后道亮钢印废品比率")]
        public float RearBrightEmbossingWasteRatio;         // 后道亮钢印废品比率

        [Description("前后道暗污点废品比率")]
        public float FrontRearDarkStainWasteRatio;         // 前后道暗污点废品比率

        [Description("前后道亮污点废品比率")]
        public float FrontRearBrightStainWasteRatio;         // 前后道亮污点废品比率

        [Description("前后道封口故障废品比率")]
        public float FrontRearSealingFaultWasteRatio;         // 前后道封口故障废品比率

        [Description("前后道暗钢印废品比率")]
        public float FrontRearDarkEmbossingWasteRatio;         // 前后道暗钢印废品比率

        [Description("前后道亮钢印废品比率")]
        public float FrontRearBrightEmbossingWasteRatio;         // 前后道亮钢印废品比率

        [Description("前道ORIS废品总比率")]
        public float FrontORISTotalWasteRatio;         // 前道ORIS废品总比率

        [Description("后道ORIS废品总比率")]
        public float RearORISTotalWasteRatio;         // 后道ORIS废品总比率

        [Description("前后道ORIS废品总比率")]
        public float FrontRearORISTotalWasteRatio;         // 前后道ORIS废品总比率
    }
}
