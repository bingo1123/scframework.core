using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 4,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SRM废品比例信息")]
    public struct SRM_ANPC
    {
        [Description("前道过轻废品比率")]
        public float FrontTooLightPct;         // 前道过轻废品比率

        [Description("前道过重废品比率")]
        public float FrontTooHeavyPct;         // 前道过重废品比率

        [Description("前道软点废品比率")]
        public float FrontSoftSpotPct;         // 前道软点废品比率

        [Description("前道硬点废品比率")]
        public float FrontHardSpotPct;         // 前道硬点废品比率

        [Description("前道轻烟端废品比率")]
        public float FrontLightEndPct;         // 前道轻烟端废品比率

        [Description("前道杂质烟废品比率")]
        public float FrontForeignMatterPct;         // 前道杂质烟废品比率

        [Description("后道过轻废品比率")]
        public float NearTooLightPct;         // 后道过轻废品比率

        [Description("后道过重废品比率")]
        public float NearTooHeavyPct;         // 后道过重废品比率

        [Description("后道软点废品比率")]
        public float NearSoftSpotPct;         // 后道软点废品比率

        [Description("后道硬点废品比率")]
        public float NearHardSpotPct;         // 后道硬点废品比率

        [Description("后道轻烟端废品比率")]
        public float NearLightEndPct;         // 后道轻烟端废品比率

        [Description("后道杂质废品比率")]
        public float NearForeignMatterPct;         // 后道杂质废品比率

        [Description("前后道过轻废品比率")]
        public float FrontNearTooLightPct;         // 前后道过轻废品比率

        [Description("前后道过重废品比率")]
        public float FrontNearTooHeavyPct;         // 前后道过重废品比率

        [Description("前后道软点废品比率")]
        public float FrontNearSoftSpotPct;         // 前后道软点废品比率

        [Description("前后道硬点废品比率")]
        public float FrontNearHardSpotPct;         // 前后道硬点废品比率

        [Description("前后道轻烟端废品比率")]
        public float FrontNearLightEndPct;         // 前后道轻烟端废品比率

        [Description("前后道杂质废品比率")]
        public float FrontNearForeignMatterPct;         // 前后道杂质废品比率

        [Description("前道SRM废品总比率")]
        public float FrontSRMTotalPct;         // 前道SRM废品总比率

        [Description("后道SRM废品总比率")]
        public float NearSRMTotalPct;         // 后道SRM废品总比率

        [Description("前后道SRM废品总比率")]
        public float FrontNearSRMTotalPct;         // 前后道SRM废品总比率
    }
}
