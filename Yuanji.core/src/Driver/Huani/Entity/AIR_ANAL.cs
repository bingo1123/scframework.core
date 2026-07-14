using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 3,func:1--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("气密性分析信息")]
    public struct AIR_ANAL
    {
        [Description("预留")]
        public byte Dummy0;         // 预留

        [Description("前排气密性故障频度属性")]
        public byte Haeuf_vo_Attri;         // 前排气密性故障频度属性

        [Description("前排气密性故障频度")]
        public int Haeuf_vo;         // 前排气密性故障频度

        [Description("预留")]
        public byte Dummy1;         // 预留

        [Description("后排气密性故障频度属性")]
        public byte Haeuf_hi_Attri;         // 后排气密性故障频度属性

        [Description("后排气密性故障频度")]
        public int Haeuf_hi;         // 后排气密性故障频度

        [Description("预留")]
        public byte Dummy2;         // 预留

        [Description("前道气密性故障频度属性")]
        public byte Haeuf_Stvo_Attri;         // 前道气密性故障频度属性

        [Description("前道气密性故障频度")]
        public int Haeuf_Stvo;         // 前道气密性故障频度

        [Description("预留")]
        public byte Dummy3;         // 预留

        [Description("后道气密性故障频度属性")]
        public byte Haeuf_Sthi_Attri;         // 后道气密性故障频度属性

        [Description("后道气密性故障频度")]
        public int Haeuf_Sthi;         // 后道气密性故障频度

        [Description("预留")]
        public byte Dummy4;         // 预留

        [Description("气密性故障频度属性")]
        public byte Haeuf_ges_Attri;         // 气密性故障频度属性

        [Description("气密性故障频度")]
        public int Haeuf_ges;         // 气密性故障频度

        [Description("前排气密性废品数")]
        public uint Stck_vo;         // 前排气密性废品数

        [Description("上一班前排气密性废品数")]
        public uint Stck_vo_alt;         // 上一班前排气密性废品数

        [Description("后排气密性废品数")]
        public uint Stck_hi;         // 后排气密性废品数

        [Description("上一班后排气密性废品数")]
        public uint Stck_hi_alt;         // 上一班后排气密性废品数

        [Description("前道气密性废品数")]
        public uint Stck_Stvo;         // 前道气密性废品数

        [Description("上一班前道气密性废品数")]
        public uint Stck_Stvo_alt;         // 上一班前道气密性废品数

        [Description("后道气密性废品数")]
        public uint Stck_Sthi;         // 后道气密性废品数

        [Description("上一班后道气密性废品数")]
        public uint Stck_Sthi_alt;         // 上一班后道气密性废品数

        [Description("气密性废品数")]
        public uint Stck_ges;         // 气密性废品数

        [Description("上一班气密性废品数")]
        public uint Stck_ges_alt;         // 上一班气密性废品数

        [Description("预留")]
        public byte Dummy5;         // 预留

        [Description("")]
        public byte Haeufgr_Attri;         // 

        [Description("")]
        public int Haeufgr;         // 

        [Description("前排气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_vo;         // 前排气密性详细绘图数据

        [Description("后排气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_hi;         // 后排气密性详细绘图数据

        [Description("前道气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_Stvo;         // 前道气密性详细绘图数据

        [Description("后道气密性详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_Sthi;         // 后道气密性详细绘图数据
    }
}
