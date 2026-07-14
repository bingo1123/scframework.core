using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 5,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("ORIS废品统计信息")]
    public struct ORIS_ANAC
    {
        [Description("前道暗污点废品")]
        public uint FrontDarkStainWaste;         // 前道暗污点废品

        [Description("前道亮污点废品")]
        public uint FrontBrightStainWaste;         // 前道亮污点废品

        [Description("前道封口故障废品")]
        public uint FrontSealingFaultWaste;         // 前道封口故障废品

        [Description("前道暗钢印废品")]
        public uint FrontDarkEmbossingWaste;         // 前道暗钢印废品

        [Description("前道亮钢印废品")]
        public uint FrontBrightEmbossingWaste;         // 前道亮钢印废品

        [Description("后道暗污点废品")]
        public uint RearDarkStainWaste;         // 后道暗污点废品

        [Description("后道亮污点废品")]
        public uint RearBrightStainWaste;         // 后道亮污点废品

        [Description("后道封口故障废品")]
        public uint RearSealingFaultWaste;         // 后道封口故障废品

        [Description("后道暗钢印废品")]
        public uint RearDarkEmbossingWaste;         // 后道暗钢印废品

        [Description("后道亮钢印废品")]
        public uint RearBrightEmbossingWaste;         // 后道亮钢印废品

        [Description("前后道暗污点废品")]
        public uint FrontRearDarkStainWaste;         // 前后道暗污点废品

        [Description("前后道亮污点废品")]
        public uint FrontRearBrightStainWaste;         // 前后道亮污点废品

        [Description("前后道封口故障废品")]
        public uint FrontRearSealingFaultWaste;         // 前后道封口故障废品

        [Description("前后道暗钢印废品")]
        public uint FrontRearDarkEmbossingWaste;         // 前后道暗钢印废品

        [Description("前后道亮钢印废品")]
        public uint FrontRearBrightEmbossingWaste;         // 前后道亮钢印废品

        [Description("前道ORIS废品总数")]
        public uint FrontORISTotalWaste;         // 前道ORIS废品总数

        [Description("后道ORIS废品总数")]
        public uint RearORISTotalWaste;         // 后道ORIS废品总数

        [Description("前后道ORIS废品总数")]
        public uint FrontRearORISTotalWaste;         // 前后道ORIS废品总数
    }
}
