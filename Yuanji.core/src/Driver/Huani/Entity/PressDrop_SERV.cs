using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{

    /// <summary>
    /// node 3,func:3   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("通风度分析信息")]
    public struct PressDrop_SERV
    {
        [Description("波峰位置偏移")]
        public byte Plot_Marke1_Offset;         // 波峰位置偏移

        [Description("波峰位置长度")]
        public byte Plot_Marke1_Laenge;         // 波峰位置长度

        [Description("波谷位置偏移")]
        public byte Plot_Marke2_Offset;         // 波谷位置偏移

        [Description("波谷位置长度")]
        public byte Plot_Marke2_Laenge;         // 波谷位置长度

        [Description("预留")]
        public ushort Stellung_Fluistor;         // 预留

        [Description("预留")]
        public byte Dummy0;         // 预留

        [Description("预留")]
        public byte Vordruck_Attri;         // 预留

        [Description("预留")]
        public ushort Vordruck;         // 预留

        [Description("预留")]
        public byte Dummy1;         // 预留

        [Description("预留")]
        public byte Spg_Vordr_Attri;         // 预留

        [Description("预留")]
        public ushort Spg_Vordr;         // 预留

        [Description("预留")]
        public byte Dummy2;         // 预留

        [Description("前排烟支吸阻传感器属性")]
        public byte Druck_Zig_vo_Attri;         // 前排烟支吸阻传感器属性

        [Description("前排烟支吸阻传感器值")]
        public ushort Druck_Zig_vo;         // 前排烟支吸阻传感器值

        [Description("预留")]
        public byte Dummy3;         // 预留

        [Description("后排烟支吸阻传感器属性")]
        public byte Druck_Zig_hi_Attri;         // 后排烟支吸阻传感器属性

        [Description("后排烟支吸阻传感器值")]
        public ushort Druck_Zig_hi;         // 后排烟支吸阻传感器值

        [Description("预留")]
        public byte Dummy4;         // 预留

        [Description("前排烟支吸阻传感器电压属性")]
        public byte Spg_Zig_vo_Attri;         // 前排烟支吸阻传感器电压属性

        [Description("前排烟支吸阻传感器电压值")]
        public ushort Spg_Zig_vo;         // 前排烟支吸阻传感器电压值

        [Description("预留")]
        public byte Dummy5;         // 预留

        [Description("后排烟支吸阻传感器电压属性")]
        public byte Spg_Zig_hi_Attri;         // 后排烟支吸阻传感器电压属性

        [Description("后排烟支吸阻传感器电压值")]
        public ushort Spg_Zig_hi;         // 后排烟支吸阻传感器电压值

        [Description("预留")]
        public byte Dummy6;         // 预留

        [Description("前排烟支吸阻均值属性")]
        public byte Mittw_vo_Attri;         // 前排烟支吸阻均值属性

        [Description("前排烟支吸阻均值")]
        public ushort Mittw_vo;         // 前排烟支吸阻均值

        [Description("预留")]
        public byte Dummy7;         // 预留

        [Description("前排前道烟支吸阻均值属性")]
        public byte Mittw_vo_Stvo_Attri;         // 前排前道烟支吸阻均值属性

        [Description("前排前道烟支吸阻均值")]
        public ushort Mittw_vo_Stvo;         // 前排前道烟支吸阻均值

        [Description("预留")]
        public byte Dummy8;         // 预留

        [Description("前排后道烟支吸阻均值属性")]
        public byte Mittw_vo_Sthi_Attri;         // 前排后道烟支吸阻均值属性

        [Description("前排后道烟支吸阻均值")]
        public ushort Mittw_vo_Sthi;         // 前排后道烟支吸阻均值

        [Description("预留")]
        public byte Dummy9;         // 预留

        [Description("后排烟支吸阻均值属性")]
        public byte Mittw_hi_Attri;         // 后排烟支吸阻均值属性

        [Description("后排烟支吸阻均值")]
        public ushort Mittw_hi;         // 后排烟支吸阻均值

        [Description("预留")]
        public byte Dummy10;         // 预留

        [Description("后排前道烟支吸阻均值属性")]
        public byte Mittw_hi_Stvo_Attri;         // 后排前道烟支吸阻均值属性

        [Description("后排前道烟支吸阻均值")]
        public ushort Mittw_hi_Stvo;         // 后排前道烟支吸阻均值

        [Description("预留")]
        public byte Dummy11;         // 预留

        [Description("后排后道烟支吸阻均值属性")]
        public byte Mittw_hi_Sthi_Attri;         // 后排后道烟支吸阻均值属性

        [Description("后排后道烟支吸阻均值")]
        public ushort Mittw_hi_Sthi;         // 后排后道烟支吸阻均值

        [Description("前排烟支吸阻波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public byte[,] Mulde_vo;         // 前排烟支吸阻波形数据（二维数组）

        [Description("后排烟支吸阻波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public byte[,] Mulde_hi;         // 后排烟支吸阻波形数据（二维数组）

        [Description("前排烟支吸阻传感器电压详细数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3280)]
        public ushort[] Spg_Zig_Tro_vo;         // 前排烟支吸阻传感器电压详细数据

        [Description("后排烟支吸阻传感器电压详细数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3280)]
        public ushort[] Spg_Zig_Tro_hi;         // 后排烟支吸阻传感器电压详细数据
    }
}
