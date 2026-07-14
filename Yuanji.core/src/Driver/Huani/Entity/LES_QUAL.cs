using System.ComponentModel;
using System.Runtime.InteropServices;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /// <summary>
    /// node 11,func:0--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("LES空头信息")]
    public struct LES_QUAL
    {
        [Description("预留字节")]
        public byte Dummy1;

        [Description("前排端头烟丝密度均值属性")]
        public byte LES_Mittw_bv_akt_Attri;

        [Description("前排端头烟丝密度均值")]
        public ushort LES_Mittw_bv_akt;

        [Description("预留字节")]
        public byte Dummy2;

        [Description("后排端头烟丝密度均值属性")]
        public byte LES_Mittw_bh_akt_Attri;

        [Description("后排端头烟丝密度均值")]
        public ushort LES_Mittw_bh_akt;

        [Description("预留字节")]
        public byte Dummy3;

        [Description("前排端头烟丝密度标准差属性")]
        public byte LES_Stdabw_bv_Attri;

        [Description("前排端头烟丝密度标准差")]
        public ushort LES_Stdabw_bv;

        [Description("预留字节")]
        public byte Dummy4;

        [Description("后排端头烟丝密度标准差属性")]
        public byte LES_Stdabw_bh_Attri;

        [Description("后排端头烟丝密度标准差")]
        public ushort LES_Stdabw_bh;

        [Description("预留字节")]
        public byte Dummy5;

        [Description("前道端头烟丝密度均值属性")]
        public byte LES_Mittw_sv_akt_Attri;

        [Description("前道端头烟丝密度均值")]
        public ushort LES_Mittw_sv_akt;

        [Description("预留字节")]
        public byte Dummy6;

        [Description("后道端头烟丝密度均值属性")]
        public byte LES_Mittw_sh_akt_Attri;

        [Description("后道端头烟丝密度均值")]
        public ushort LES_Mittw_sh_akt;

        [Description("预留字节")]
        public byte Dummy7;

        [Description("前道端头烟丝密度标准差属性")]
        public byte LES_Stdabw_sv_Attri;

        [Description("前道端头烟丝密度标准差")]
        public ushort LES_Stdabw_sv;

        [Description("预留字节")]
        public byte Dummy8;

        [Description("后道端头烟丝密度标准差属性")]
        public byte LES_Stdabw_sh_Attri;

        [Description("后道端头烟丝密度标准差")]
        public ushort LES_Stdabw_sh;

        [Description("预留字节")]
        public byte Dummy9;

        [Description("空头废品门槛属性")]
        public byte LES_Auswgr_min_Attri;

        [Description("空头废品门槛")]
        public byte LES_Auswgr_min;

        [Description("合格产品平均电压属性")]
        public byte LES_Gutproduktion_Attri;

        [Description("合格产品平均电压")]
        public ushort LES_Gutproduktion;

        [Description("预留字节")]
        public byte Dummy11;

        [Description("LPL相关的前排端头烟丝密度均值属性")]
        public byte LPL_Mittw_bv_akt_Attri;

        [Description("LPL相关的前排端头烟丝密度均值")]
        public ushort LPL_Mittw_bv_akt;

        [Description("预留字节")]
        public byte Dummy12;

        [Description("LPL相关的后排端头烟丝密度均值属性")]
        public byte LPL_Mittw_bh_akt_Attri;

        [Description("LPL相关的后排端头烟丝密度均值")]
        public ushort LPL_Mittw_bh_akt;

        [Description("预留字节")]
        public byte Dummy13;

        [Description("LPL相关的前排端头烟丝密度标准差属性")]
        public byte LPL_Stdabw_bv_Attri;

        [Description("LPL相关的前排端头烟丝密度标准差")]
        public ushort LPL_Stdabw_bv;

        [Description("预留字节")]
        public byte Dummy14;

        [Description("LPL相关的后排端头烟丝密度标准差属性")]
        public byte LPL_Stdabw_bh_Attri;

        [Description("LPL相关的后排端头烟丝密度标准差")]
        public ushort LPL_Stdabw_bh;

        [Description("预留字节")]
        public byte Dummy15;

        [Description("LPL相关的前道端头烟丝密度均值属性")]
        public byte LPL_Mittw_sv_akt_Attri;

        [Description("LPL相关的前道端头烟丝密度均值")]
        public ushort LPL_Mittw_sv_akt;

        [Description("预留字节")]
        public byte Dummy16;

        [Description("LPL相关的后道端头烟丝密度均值属性")]
        public byte LPL_Mittw_sh_akt_Attri;

        [Description("LPL相关的后道端头烟丝密度均值")]
        public ushort LPL_Mittw_sh_akt;

        [Description("预留字节，可能用于后续扩展等情况")]
        public byte Dummy17;

        [Description("LPL相关的前道端头烟丝密度标准差属性")]
        public byte LPL_Stdabw_sv_Attri;

        [Description("LPL相关的前道端头烟丝密度标准差")]
        public ushort LPL_Stdabw_sv;

        [Description("预留字节")]
        public byte Dummy18;

        [Description("LPL相关的后道端头烟丝密度标准差属性")]
        public byte LPL_Stdabw_sh_Attri;

        [Description("LPL相关的后道端头烟丝密度标准差")]
        public ushort LPL_Stdabw_sh;

        [Description("预留字节，可能用于后续扩展等情况")]
        public byte Dummy19;

        [Description("LPL相关的空头废品门槛属性")]
        public byte LPL_Auswgr_min_Attri;

        [Description("LPL相关的空头废品门槛")]
        public byte LPL_Auswgr_min;

        [Description("LPL相关的合格产品平均电压属性")]
        public byte LPL_Gutproduktion_Attri;

        [Description("LPL相关的合格产品平均电压")]
        public ushort LPL_Gutproduktion;

        [Description("LES相关的前排端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LES_Graph_bv_akt;

        [Description("LES相关的后排端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LES_Graph_bh_akt;

        [Description("LPL相关的前排端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LPL_Graph_bv_akt;

        [Description("LPL相关的后排端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LPL_Graph_bh_akt;

        [Description("LES相关的前排端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LES_Plot_bv;

        [Description("LES相关的后排端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LES_Plot_bh;

        [Description("LPL相关的前排端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LPL_Plot_bv;

        [Description("LPL相关的后排端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LPL_Plot_bh;

        [Description("LES相关的前道端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LES_Graph_sv_akt;

        [Description("LES相关的后道端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LES_Graph_sh_akt;

        [Description("LPL相关的前道端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LPL_Graph_sv_akt;

        [Description("LPL相关的后道端头烟丝密度波形数据（二维数组）")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[,] LPL_Graph_sh_akt;

        [Description("LES相关的前道端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LES_Plot_sv;

        [Description("LES相关的后道端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LES_Plot_sh;

        [Description("LPL相关的前道端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LPL_Plot_sv;

        [Description("LPL相关的后道端头烟丝密度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] LPL_Plot_sh;
    }

   
}
