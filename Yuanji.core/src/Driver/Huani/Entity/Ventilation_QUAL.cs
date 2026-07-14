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
    /// node 3,func:2   --zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("通风度信息")]
    public struct Ventilation_QUAL
    {
        [Description("预留")]
        public byte Dummy0;         // 预留

        [Description("前排通风度均值属性")]
        public byte Mittw_vo_akt_Attri;         // 前排通风度均值属性

        [Description("前排通风度均值")]
        public ushort Mittw_vo_akt;         // 前排通风度均值

        [Description("预留")]
        public byte Dummy1;         // 预留

        [Description("后排通风度均值属性")]
        public byte Mittw_hi_akt_Attri;         // 后排通风度均值属性

        [Description("后排通风度均值")]
        public ushort Mittw_hi_akt;         // 后排通风度均值

        [Description("预留")]
        public byte Dummy2;         // 预留

        [Description("前排通风度标准差属性")]
        public byte Stdabw_vo_Attri;         // 前排通风度标准差属性

        [Description("前排通风度标准差")]
        public ushort Stdabw_vo;         // 前排通风度标准差

        [Description("预留")]
        public byte Dummy3;         // 预留

        [Description("后排通风度标准差属性")]
        public byte Stdabw_hi_Attri;         // 后排通风度标准差属性

        [Description("后排通风度标准差")]
        public ushort Stdabw_hi;         // 后排通风度标准差

        [Description("预留")]
        public byte Dummy4;         // 预留

        [Description("前道通风度均值属性")]
        public byte Mittw_Stvo_akt_Attri;         // 前道通风度均值属性

        [Description("前道通风度均值")]
        public ushort Mittw_Stvo_akt;         // 前道通风度均值

        [Description("预留")]
        public byte Dummy5;         // 预留

        [Description("后道通风度均值属性")]
        public byte Mittw_Sthi_akt_Attri;         // 后道通风度均值属性

        [Description("后道通风度均值")]
        public ushort Mittw_Sthi_akt;         // 后道通风度均值

        [Description("预留")]
        public byte Dummy6;         // 预留

        [Description("前道通风度标准差属性")]
        public byte Stdabw_Stvo_Attri;         // 前道通风度标准差属性

        [Description("前道通风度标准差")]
        public ushort Stdabw_Stvo;         // 前道通风度标准差

        [Description("预留")]
        public byte Dummy7;         // 预留

        [Description("后道通风度标准差属性")]
        public byte Stdabw_Sthi_Attri;         // 后道通风度标准差属性

        [Description("后道通风度标准差")]
        public ushort Stdabw_Sthi;         // 后道通风度标准差

        [Description("预留")]
        public byte Dummy8;         // 预留

        [Description("最小通风度废品门槛属性")]
        public byte Auswgr_min_Attri;         // 最小通风度废品门槛属性

        [Description("最小通风度废品门槛")]
        public int Auswgr_min;         // 最小通风度废品门槛

        [Description("预留")]
        public byte Dummy9;         // 预留

        [Description("最大通风度废品门槛属性")]
        public byte Auswgr_max_Attri;         // 最大通风度废品门槛属性

        [Description("最大通风度废品门槛")]
        public int Auswgr_max;         // 最大通风度废品门槛

        [Description("预留")]
        public byte Dummy10;         // 预留

        [Description("最小通风度警告门槛属性")]
        public byte Warngr_min_Attri;         // 最小通风度警告门槛属性

        [Description("最小通风度警告门槛")]
        public int Warngr_min;         // 最小通风度警告门槛

        [Description("预留")]
        public byte Dummy11;         // 预留

        [Description("最大通风度警告门槛属性")]
        public byte Warngr_max_Attri;         // 最大通风度警告门槛属性

        [Description("最大通风度警告门槛")]
        public int Warngr_max;         // 最大通风度警告门槛

        [Description("预留")]
        public byte Dummy12;         // 预留

        [Description("最小通风度停机门槛属性")]
        public byte Stopgr_min_Attri;         // 最小通风度停机门槛属性

        [Description("最小通风度停机门槛")]
        public int Stopgr_min;         // 最小通风度停机门槛

        [Description("预留")]
        public byte Dummy13;         // 预留

        [Description("最大通风度停机门槛属性")]
        public byte Stopgr_max_Attri;         // 最大通风度停机门槛属性

        [Description("最大通风度停机门槛")]
        public int Stopgr_max;         // 最大通风度停机门槛

        [Description("预留")]
        public byte Dummy14;         // 预留

        [Description("通风度平均值属性")]
        public byte Mittw_akt_Attri;         // 通风度平均值属性

        [Description("通风度平均值")]
        public ushort Mittw_akt;         // 通风度平均值

        [Description("预留")]
        public byte Dummy15;         // 预留

        [Description("通风度标准偏差属性")]
        public byte Stdabw_Attri;         // 通风度标准偏差属性

        [Description("通风度标准偏差")]
        public ushort Stdabw;         // 通风度标准偏差

        [Description("前排通风度波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] Graph_vo_akt;         // 前排通风度波形数据

        [Description("后排通风度波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] Graph_hi_akt;         // 后排通风度波形数据

        [Description("前排通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_vo;         // 前排通风度详细绘图数据

        [Description("后排通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_hi;         // 后排通风度详细绘图数据

        [Description("前道通风度波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] Graph_Stvo_akt;         // 前道通风度波形数据

        [Description("后道通风度波形数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public short[] Graph_Sthi_akt;         // 后道通风度波形数据

        [Description("前道通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_Stvo;         // 前道通风度详细绘图数据

        [Description("后道通风度详细绘图数据")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 97)]
        public byte[] Plot_Sthi;         // 后道通风度详细绘图数据
    }
}
