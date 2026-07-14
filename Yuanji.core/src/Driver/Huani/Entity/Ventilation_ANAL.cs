using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 3,func:2   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("通风度分析信息")]
    public struct Ventilation_ANAL
    {
        [Description("预留")]
        public byte Dummy0;         // 预留

        [Description("前排最小通风度故障频度属性")]
        public byte Haeuf_min_vo_Attri;         // 前排最小通风度故障频度属性

        [Description("前排最小通风度故障频度")]
        public int Haeuf_min_vo;         // 前排最小通风度故障频度

        [Description("预留")]
        public byte Dummy1;         // 预留

        [Description("前排最大通风度故障频度属性")]
        public byte Haeuf_max_vo_Attri;         // 前排最大通风度故障频度属性

        [Description("前排最大通风度故障频度")]
        public int Haeuf_max_vo;         // 前排最大通风度故障频度

        [Description("预留")]
        public byte Dummy2;         // 预留

        [Description("后排最小通风度故障频度属性")]
        public byte Haeuf_min_hi_Attri;         // 后排最小通风度故障频度属性

        [Description("后排最小通风度故障频度")]
        public int Haeuf_min_hi;         // 后排最小通风度故障频度

        [Description("预留")]
        public byte Dummy3;         // 预留

        [Description("后排最大通风度故障频度属性")]
        public byte Haeuf_max_hi_Attri;         // 后排最大通风度故障频度属性

        [Description("后排最大通风度故障频度")]
        public int Haeuf_max_hi;         // 后排最大通风度故障频度

        [Description("预留")]
        public byte Dummy4;         // 预留

        [Description("前道最小通风度故障频度属性")]
        public byte Haeuf_min_Stvo_Attri;         // 前道最小通风度故障频度属性

        [Description("前道最小通风度故障频度")]
        public int Haeuf_min_Stvo;         // 前道最小通风度故障频度

        [Description("预留")]
        public byte Dummy5;         // 预留

        [Description("前道最大通风度故障频度属性")]
        public byte Haeuf_max_Stvo_Attri;         // 前道最大通风度故障频度属性

        [Description("前道最大通风度故障频度")]
        public int Haeuf_max_Stvo;         // 前道最大通风度故障频度

        [Description("预留")]
        public byte Dummy6;         // 预留

        [Description("后道最小通风度故障频度属性")]
        public byte Haeuf_min_Sthi_Attri;         // 后道最小通风度故障频度属性

        [Description("后道最小通风度故障频度")]
        public int Haeuf_min_Sthi;         // 后道最小通风度故障频度

        [Description("预留")]
        public byte Dummy7;         // 预留

        [Description("后道最大通风度故障频度属性")]
        public byte Haeuf_max_Sthi_Attri;         // 后道最大通风度故障频度属性

        [Description("后道最大通风度故障频度")]
        public int Haeuf_max_Sthi;         // 后道最大通风度故障频度

        [Description("预留")]
        public byte Dummy8;         // 预留

        [Description("通风度故障频度属性")]
        public byte Haeuf_ges_Attri;         // 通风度故障频度属性

        [Description("通风度故障频度")]
        public int Haeuf_ges;         // 通风度故障频度

        [Description("前排最小通风度废品数")]
        public uint Stck_min_vo;         // 前排最小通风度废品数

        [Description("上一班前排最小通风度废品数")]
        public uint Stck_min_vo_alt;         // 上一班前排最小通风度废品数

        [Description("前排最大通风度废品数")]
        public uint Stck_max_vo;         // 前排最大通风度废品数

        [Description("上一班前排最大通风度废品数")]
        public uint Stck_max_vo_alt;         // 上一班前排最大通风度废品数

        [Description("后排最小通风度废品数")]
        public uint Stck_min_hi;         // 后排最小通风度废品数

        [Description("上一班后排最小通风度废品数")]
        public uint Stck_min_hi_alt;         // 上一班后排最小通风度废品数

        [Description("后排最大通风度废品数")]
        public uint Stck_max_hi;         // 后排最大通风度废品数

        [Description("上一班后排最大通风度废品数")]
        public uint Stck_max_hi_alt;         // 上一班后排最大通风度废品数

        [Description("前道最小通风度废品数")]
        public uint Stck_min_Stvo;         // 前道最小通风度废品数

        [Description("上一班前道最小通风度废品数")]
        public uint Stck_min_Stvo_alt;         // 上一班前道最小通风度废品数

        [Description("前道最大通风度废品数")]
        public uint Stck_max_Stvo;         // 前道最大通风度废品数

        [Description("上一班前道最大通风度废品数")]
        public uint Stck_max_Stvo_alt;         // 上一班前道最大通风度废品数

        [Description("后道最小通风度废品数")]
        public uint Stck_min_Sthi;         // 后道最小通风度废品数

        [Description("上一班后道最小通风度废品数")]
        public uint Stck_min_Sthi_alt;         // 上一班后道最小通风度废品数

        [Description("后道最大通风度废品数")]
        public uint Stck_max_Sthi;         // 后道最大通风度废品数

        [Description("上一班后道最大通风度废品数")]
        public uint Stck_max_Sthi_alt;         // 上一班后道最大通风度废品数

        [Description("通风度废品数")]
        public uint Stck_ges;         // 通风度废品数

        [Description("上一班通风度废品数")]
        public uint Stck_ges_alt;         // 上一班通风度废品数

        [Description("预留")]
        public byte Dummy9;         // 预留

        [Description("废品百分比属性")]
        public byte Haeufgr_Attri;         // 废品百分比属性

        [Description("废品百分比")]
        public int Haeufgr;         // 废品百分比

        [Description("前排最小通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_min_vo;         // 前排最小通风度详细绘图数据

        [Description("前排最大通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_max_vo;         // 前排最大通风度详细绘图数据

        [Description("后排最小通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_min_hi;         // 后排最小通风度详细绘图数据

        [Description("后排最大通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_max_hi;         // 后排最大通风度详细绘图数据

        [Description("前道最小通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_min_Stvo;         // 前道最小通风度详细绘图数据

        [Description("前道最大通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_max_Stvo;         // 前道最大通风度详细绘图数据

        [Description("后道最小通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_min_Sthi;         // 后道最小通风度详细绘图数据

        [Description("后道最大通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_max_Sthi;         // 后道最大通风度详细绘图数据
    }

   
}
