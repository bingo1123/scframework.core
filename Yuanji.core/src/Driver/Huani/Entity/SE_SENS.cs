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
    /// node 1,func:3--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("SE 传感器信息")]
    public struct SE_SENS
    {
        [Description("前道纸高传感器值")]
        public int cig_paper_height_sensor_value_front;         // 前道纸高传感器值

        [Description("后道纸高传感器值")]
        public int cig_paper_height_sensor_value_rear;         // 后道纸高传感器值

        [Description("前道纸高无纸标定数据")]
        public int cig_paper_height_no_paper_mark_data_front;         // 前道纸高无纸标定数据

        [Description("后道纸高无纸标定数据")]
        public int cig_paper_height_no_paper_mark_data_rear;         // 后道纸高无纸标定数据

        [Description("前道纸高满纸标定数据")]
        public int cig_paper_height_full_paper_mark_data_front;         // 前道纸高满纸标定数据

        [Description("后道纸高满纸标定数据")]
        public int cig_paper_height_full_paper_mark_data_rear;         // 后道纸高满纸标定数据

        [Description("前道纸边实际高度")]
        public int cig_paper_actual_height_front;         // 前道纸边实际高度

        [Description("后道纸边实际高度")]
        public int cig_paper_actual_height_rear;         // 后道纸边实际高度

        [Description("前道纸高电机位置")]
        public int cig_paper_height_motor_position_front;         // 前道纸高电机位置

        [Description("后道纸高电机位置")]
        public int cig_paper_height_motor_position_rear;         // 后道纸高电机位置

        [Description("纸边宽度实际差值")]
        public int cig_paper_width_actual_offset_value;         // 纸边宽度实际差值

        [Description("纸宽电机位置")]
        public int cig_paper_width_motor_position;         // 纸宽电机位置

        [Description("备用")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 176)]
        public byte[] sensor_reserve_1;         // 备用

        [Description("前道纸边高度波形")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public int[] cig_paper_height_oscilloscope_front;         // 前道纸边高度波形

        [Description("后道纸边高度波形")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public int[] cig_paper_height_oscilloscope_rear;         // 后道纸边高度波形

        [Description("纸边宽度波形")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public int[] cig_paper_width_oscilloscope;         // 纸边宽度波形

        [Description("前道纸高电机位置波形")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public int[] cig_paper_height_motor_pos_osc_front;         // 前道纸高电机位置波形

        [Description("后道纸高电机位置波形")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public int[] cig_paper_height_motor_pos_osc_rear;         // 后道纸高电机位置波形

        [Description("后道纸高电机位置波形")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 500)]
        public int[] cig_paper_width_motor_pos_oscilloscope;         // 纸宽电机位置波形
    }
}
