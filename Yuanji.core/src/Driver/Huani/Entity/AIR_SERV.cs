using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 3,func:1--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("气密性设定信息")]
    public struct AIR_SERV
    {
        [Description("波峰位置偏移")]
        public byte Plot_Marke1_Offset;         // 波峰位置偏移

        [Description("波峰位置长度")]
        public byte Plot_Marke1_Laenge;         // 波峰位置长度

        [Description("波谷位置偏移")]
        public byte Plot_Marke2_Offset;         // 波谷位置偏移

        [Description("波谷位置长度")]
        public byte Plot_Marke2_Laenge;         // 波谷位置长度

        [Description("Tylan调节阀属性")]
        public ushort Stellung_Ventil;         // Tylan调节阀属性

        [Description("Tylan调节阀开度")]
        public ushort Stellung_Tylan;         // Tylan调节阀开度

        [Description("气源故障文本")]
        public ushort Druckmeldung_Ers;         // 气源故障文本

        [Description("气源打开按钮")]
        public byte Tastendarstellung_F3;         // 气源打开按钮

        [Description("检测气源属性")]
        public byte Druck_Volumen_Attri;         // 检测气源属性

        [Description("检测气源压力")]
        public ushort Druck_Volumen;         // 检测气源压力

        [Description("测试气源打开按钮")]
        public byte Taste_F3_On;         // 测试气源打开按钮

        [Description("检测气源电压属性")]
        public byte Spg_Volumen_Attri;         // 检测气源电压属性

        [Description("检测气源电压")]
        public ushort Spg_Volumen;         // 检测气源电压

        [Description("备用")]
        public byte Dummy1;         // 备用

        [Description("前排烟支气密性传感器属性")]
        public byte Druck_Zig_vo_Attri;         // 前排烟支气密性传感器属性

        [Description("前排烟支气密性传感器值")]
        public ushort Druck_Zig_vo;         // 前排烟支气密性传感器值

        [Description("备用")]
        public byte Dummy2;         // 备用

        [Description("后排烟支气密性传感器属性")]
        public byte Druck_Zig_hi_Attri;         // 后排烟支气密性传感器属性

        [Description("后排烟支气密性传感器值")]
        public ushort Druck_Zig_hi;         // 后排烟支气密性传感器值

        [Description("备用")]
        public byte Dummy3;         // 备用

        [Description("前排烟支气密性传感器电压属性")]
        public byte Spg_Zig_vo_Attri;         // 前排烟支气密性传感器电压属性

        [Description("前排烟支气密性传感器电压值")]
        public ushort Spg_Zig_vo;         // 前排烟支气密性传感器电压值

        [Description("备用")]
        public byte Dummy4;         // 备用

        [Description("后排烟支气密性传感器电压属性")]
        public byte Spg_Zig_hi_Attri;         // 后排烟支气密性传感器电压属性

        [Description("后排烟支气密性传感器电压值")]
        public ushort Spg_Zig_hi;         // 后排烟支气密性传感器电压值

        [Description("备用")]
        public byte Dummy5;         // 备用

        [Description("前排烟支气密性均值属性")]
        public byte Mittw_vo_Attri;         // 前排烟支气密性均值属性

        [Description("前排烟支气密性均值")]
        public ushort Mittw_vo;         // 前排烟支气密性均值

        [Description("备用")]
        public byte Dummy6;         // 备用

        [Description("前排前道烟支气密性均值属性")]
        public byte Mittw_vo_Stvo_Attri;         // 前排前道烟支气密性均值属性

        [Description("前排前道烟支气密性均值")]
        public ushort Mittw_vo_Stvo;         // 前排前道烟支气密性均值

        [Description("备用")]
        public byte Dummy7;         // 备用

        [Description("前排后道烟支气密性均值属性")]
        public byte Mittw_vo_Sthi_Attri;         // 前排后道烟支气密性均值属性

        [Description("前排后道烟支气密性均值")]
        public ushort Mittw_vo_Sthi;         // 前排后道烟支气密性均值

        [Description("备用")]
        public byte Dummy8;         // 备用

        [Description("后排烟支气密性均值属性")]
        public byte Mittw_hi_Attri;         // 后排烟支气密性均值属性

        [Description("后排烟支气密性均值")]
        public ushort Mittw_hi;         // 后排烟支气密性均值

        [Description("备用")]
        public byte Dummy9;         // 备用

        [Description("后排前道烟支气密性均值属性")]
        public byte Mittw_hi_Stvo_Attri;         // 后排前道烟支气密性均值属性

        [Description("后排前道烟支气密性均值")]
        public ushort Mittw_hi_Stvo;         // 后排前道烟支气密性均值

        [Description("备用")]
        public byte Dummy10;         // 备用

        [Description("后排后道烟支气密性均值属性")]
        public byte Mittw_hi_Sthi_Attri;         // 后排后道烟支气密性均值属性

        [Description("后排后道烟支气密性均值")]
        public ushort Mittw_hi_Sthi;         // 后排后道烟支气密性均值

        [Description("前排烟支气密性波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public byte[,] Mulde_vo;         // 前排烟支气密性波形数据（二维数组）

        [Description("后排烟支气密性波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 41)]
        public byte[,] Mulde_hi;         // 后排烟支气密性波形数据（二维数组）

        [Description("前排烟支气密性传感器电压详细数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3280)]
        public ushort[] Spg_Zig_Tro_vo;         // 前排烟支气密性传感器电压详细数据

        [Description("后排烟支气密性传感器电压详细数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3280)]
        public ushort[] Spg_Zig_Tro_hi;         // 后排烟支气密性传感器电压详细数据
    }
}
