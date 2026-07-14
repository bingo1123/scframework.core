using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 3,func:2   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("通风度信息")]
    public struct Ventilation_SERV
    {
        [Description("波峰位置偏移")]
        public byte Plot_Marke1_Offset;         // 波峰位置偏移

        [Description("波峰位置长度")]
        public byte Plot_Marke1_Laenge;         // 波峰位置长度

        [Description("波谷位置偏移")]
        public byte Plot_Marke2_Offset;         // 波谷位置偏移

        [Description("波谷位置长度")]
        public byte Plot_Marke2_Laenge;         // 波谷位置长度

        [Description("备用")]
        public byte Dummy0;         // 备用

        [Description("前排烟支通风度传感器属性")]
        public byte Vordruck_Zig_vo_Attri;         // 前排烟支通风度传感器属性

        [Description("前排烟支通风度传感器值")]
        public ushort Vordruck_Zig_vo;         // 前排烟支通风度传感器值

        [Description("备用")]
        public byte Dummy1;         // 备用

        [Description("后排烟支通风度传感器属性")]
        public byte Vordruck_Zig_hi_Attri;         // 后排烟支通风度传感器属性

        [Description("后排烟支通风度传感器值")]
        public ushort Vordruck_Zig_hi;         // 后排烟支通风度传感器值

        [Description("备用")]
        public byte Dummy2;         // 备用

        [Description("前排烟支通风度传感器电压属性")]
        public byte Spg_Vordr_Zig_vo_Attri;         // 前排烟支通风度传感器电压属性

        [Description("前排烟支通风度传感器电压值")]
        public ushort Spg_Vordr_Zig_vo;         // 前排烟支通风度传感器电压值

        [Description("备用")]
        public byte Dummy3;         // 备用

        [Description("后排烟支通风度传感器电压属性")]
        public byte Spg_Vordr_Zig_hi_Attri;         // 后排烟支通风度传感器电压属性

        [Description("后排烟支通风度传感器电压值")]
        public ushort Spg_Vordr_Zig_hi;         // 后排烟支通风度传感器电压值

        [Description("备用")]
        public byte Dummy4;         // 备用

        [Description("前排烟支通风度均值属性")]
        public byte Mittw_vo_Attri;         // 前排烟支通风度均值属性

        [Description("前排烟支通风度均值")]
        public ushort Mittw_vo;         // 前排烟支通风度均值

        [Description("备用")]
        public byte Dummy5;         // 备用

        [Description("前排前道烟支通风度均值属性")]
        public byte Mittw_vo_Stvo_Attri;         // 前排前道烟支通风度均值属性

        [Description("前排前道烟支通风度均值")]
        public ushort Mittw_vo_Stvo;         // 前排前道烟支通风度均值

        [Description("备用")]
        public byte Dummy6;         // 备用

        [Description("前排后道烟支通风度均值属性")]
        public byte Mittw_vo_Sthi_Attri;         // 前排后道烟支通风度均值属性

        [Description("前排后道烟支通风度均值")]
        public ushort Mittw_vo_Sthi;         // 前排后道烟支通风度均值

        [Description("备用")]
        public byte Dummy7;         // 备用

        [Description("后排烟支通风度均值属性")]
        public byte Mittw_hi_Attri;         // 后排烟支通风度均值属性

        [Description("后排烟支通风度均值")]
        public ushort Mittw_hi;         // 后排烟支通风度均值

        [Description("备用")]
        public byte Dummy8;         // 备用

        [Description("后排前道烟支通风度均值属性")]
        public byte Mittw_hi_Stvo_Attri;         // 后排前道烟支通风度均值属性

        [Description("后排前道烟支通风度均值")]
        public ushort Mittw_hi_Stvo;         // 后排前道烟支通风度均值

        [Description("备用")]
        public byte Dummy9;         // 备用

        [Description("后排后道烟支通风度均值属性")]
        public byte Mittw_hi_Sthi_Attri;         // 后排后道烟支通风度均值属性

        [Description("后排后道烟支通风度均值")]
        public ushort Mittw_hi_Sthi;         // 后排后道烟支通风度均值

        [Description("前排烟支通风度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public short[,] Mulde_vo;         // 前排烟支通风度波形数据（二维数组）

        [Description("后排烟支通风度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public short[,] Mulde_hi;         // 后排烟支通风度波形数据（二维数组）

        [Description("前排烟支通风度传感器电压详细数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3280)]
        public ushort[] Spg_Vordr_Zig_Tro_vo;         // 前排烟支通风度传感器电压详细数据

        [Description("后排烟支通风度传感器电压详细数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3280)]
        public ushort[] Spg_Vordr_Zig_Tro_hi;         // 后排烟支通风度传感器电压详细数据
    }
}
