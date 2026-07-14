using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{

    /// <summary>
    /// node 4,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SRM废品统计信息")]
    public struct SRM_ANAC
    {
        [Description("前道过轻废品支数")]
        public uint FrontTooLightCnt;         // 前道过轻废品支数

        [Description("前道过重废品支数")]
        public uint FrontTooHeavyCnt;         // 前道过重废品支数

        [Description("前道软点废品支数")]
        public uint FrontSoftSpotCnt;         // 前道软点废品支数

        [Description("前道硬点废品支数")]
        public uint FrontHardSpotCnt;         // 前道硬点废品支数

        [Description("前道轻烟端废品支数")]
        public uint FrontLightEndCnt;         // 前道轻烟端废品支数

        [Description("前道异物杂质废品支数")]
        public uint FrontForeignMatterCnt;         // 前道异物杂质废品支数

        [Description("后道过轻废品支数")]
        public uint NearTooLightCnt;         // 后道过轻废品支数

        [Description("后道过重废品支数")]
        public uint NearTooHeavyCnt;         // 后道过重废品支数

        [Description("后道软点废品支数")]
        public uint NearSoftSpotCnt;         // 后道软点废品支数

        [Description("后道硬点废品支数")]
        public uint NearHardSpotCnt;         // 后道硬点废品支数

        [Description("后道轻烟端废品支数")]
        public uint NearLightEndCnt;         // 后道轻烟端废品支数

        [Description("后道异物杂质废品支数")]
        public uint NearForeignMatterCnt;         // 后道异物杂质废品支数

        [Description("前后道过轻废品支数")]
        public uint FrontNearTooLightCnt;         // 前后道过轻废品支数

        [Description("前后道过重废品支数")]
        public uint FrontNearTooHeavyCnt;         // 前后道过重废品支数

        [Description("前后道软点废品支数")]
        public uint FrontNearSoftSpotCnt;         // 前后道软点废品支数

        [Description("前后道硬点废品支数")]
        public uint FrontNearHardSpotCnt;         // 前后道硬点废品支数

        [Description("前后道轻烟端废品支数")]
        public uint FrontNearLightEndCnt;         // 前后道轻烟端废品支数

        [Description("前后道异物杂质废品支数")]
        public uint FrontNearForeignMatterCnt;         // 前后道异物杂质废品支数

        [Description("前道SRM废品支数")]
        public uint FrontSRMCnt;         // 前道SRM废品支数

        [Description("后道SRM废品支数")]
        public uint NearSRMCnt;         // 后道SRM废品支数

        [Description("前后道SRM废品总支数")]
        public uint FrontNearSRMTotalCnt;         // 前后道SRM废品总支数
    }

}
