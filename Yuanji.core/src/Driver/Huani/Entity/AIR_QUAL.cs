using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 3,func:1--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("气密性信息")]
    public struct AIR_QUAL
    {
        [Description("前排气密性均值属性")]
        public byte Mittw_vo_akt_Attri;         // 前排气密性均值属性

        [Description("前排气密性均值")]
        public byte Mittw_vo_akt;         // 前排气密性均值

        [Description("后排气密性均值属性")]
        public byte Mittw_hi_akt_Attri;         // 后排气密性均值属性

        [Description("后排气密性均值")]
        public byte Mittw_hi_akt;         // 后排气密性均值

        [Description("前排气密性标准差属性")]
        public byte Stdabw_vo_Attri;         // 前排气密性标准差属性

        [Description("前排气密性标准差")]
        public byte Stdabw_vo;         // 前排气密性标准差

        [Description("后排气密性标准差属性")]
        public byte Stdabw_hi_Attri;         // 后排气密性标准差属性

        [Description("后排气密性标准差")]
        public byte Stdabw_hi;         // 后排气密性标准差

        [Description("前道气密性均值属性")]
        public byte Mittw_Stvo_akt_Attri;         // 前道气密性均值属性

        [Description("前道气密性均值")]
        public byte Mittw_Stvo_akt;         // 前道气密性均值

        [Description("后道气密性均值属性")]
        public byte Mittw_Sthi_akt_Attri;         // 后道气密性均值属性

        [Description("后道气密性均值")]
        public byte Mittw_Sthi_akt;         // 后道气密性均值

        [Description("前道气密性标准差属性")]
        public byte Stdabw_Stvo_Attri;         // 前道气密性标准差属性

        [Description("前道气密性标准差")]
        public byte Stdabw_Stvo;         // 前道气密性标准差

        [Description("后道气密性标准差属性")]
        public byte Stdabw_Sthi_Attri;         // 后道气密性标准差属性

        [Description("后道气密性标准差")]
        public byte Stdabw_Sthi;         // 后道气密性标准差

        [Description("气密性废品门槛属性")]
        public byte Auswgr_min_Attri;         // 气密性废品门槛属性

        [Description("气密性废品门槛")]
        public byte Auswgr_min;         // 气密性废品门槛

        [Description("前排气密性波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Graph_vo_akt;         // 前排气密性波形数据

        [Description("后排气密性波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Graph_hi_akt;         // 后排气密性波形数据

        [Description("前排气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_vo;         // 前排气密性详细绘图数据

        [Description("后排气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_hi;         // 后排气密性详细绘图数据

        [Description("前道气密性波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Graph_Stvo_akt;         // 前道气密性波形数据

        [Description("后道气密性波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Graph_Sthi_akt;         // 后道气密性波形数据

        [Description("前道气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_Stvo;         // 前道气密性详细绘图数据

        [Description("后道气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_Sthi;         // 后道气密性详细绘图数据
    }

    // 假设这里的TYPE_Fktgraph_Byte_t是已经定义好的类型，如果没有定义需要根据实际情况补充其定义
  
}
