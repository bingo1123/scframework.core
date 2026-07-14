using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 4,func:0   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("吸丝带信息")]
    public struct SRM_PRCC
    {
        [Description("前道吸丝带当前位置")]
        public float FrontSuctionBeltCurrentPosition;         // 前道吸丝带当前位置

        [Description("后道吸丝带当前位置")]
        public float RearSuctionBeltCurrentPosition;         // 后道吸丝带当前位置

        [Description("前道吸丝带平均位置")]
        public float FrontSuctionBeltAveragePosition;         // 前道吸丝带平均位置

        [Description("后道吸丝带平均位置")]
        public float RearSuctionBeltAveragePosition;         // 后道吸丝带平均位置

        [Description("吸丝带位置下警告极限")]
        public float SuctionBeltLowerWarningLimit;         // 吸丝带位置下警告极限

        [Description("吸丝带位置上警告极限")]
        public float SuctionBeltUpperWarningLimit;         // 吸丝带位置上警告极限
    }
}
