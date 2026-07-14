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
    /// node 2,func:2--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("MAX拼接及贴胶信息")]
    public struct TPGL
    {
        [Description("第一引纸辊断纸检测")]
        public bool B21M;         // 第一引纸辊断纸检测

        [Description("纸接头检测")]
        public bool B22M;         // 纸接头检测

        [Description("第二引纸辊断纸检测")]
        public bool B89M;         // 第二引纸辊断纸检测

        [Description("压紧辊1转出(左旋)")]
        public bool S7M_left;         // 压紧辊1转出(左旋)

        [Description("压紧辊1回转(右旋)")]
        public bool S7M_right;         // 压紧辊1回转(右旋)

        [Description("压紧辊2回转(左旋)")]
        public bool S8M_left;         // 压紧辊2回转(左旋)

        [Description("压紧辊2转出(右旋)")]
        public bool S8M_right;         // 压紧辊2转出(右旋)

        [Description("供纸部分第一引纸辊")]
        public bool A152M_Y186;         // 供纸部分第一引纸辊

        [Description("供纸部分第二引纸辊")]
        public bool A152M_Y188;         // 供纸部分第二引纸辊

        [Description("备用")]
        public bool Reserved_02;         // 备用

        [Description("水松纸切割装置关闭水松纸辊真空")]
        public bool A151M_Y122;         // 水松纸切割装置关闭水松纸辊真空

        [Description("导纸板支架水松纸片抽吸系统")]
        public bool A151M_Y134;         // 导纸板支架水松纸片抽吸系统

        [Description("水松纸张力实际值")]
        public int Tipping_tension_actual_value;         // 水松纸张力实际值

        [Description("水松纸切割装置清洁风吹除水松纸")]
        public bool A151M_Y126;         // 水松纸切割装置清洁风吹除水松纸

        [Description("水松纸片抽吸系统抽吸")]
        public bool A151M_Y128;         // 水松纸片抽吸系统抽吸

        [Description("水松纸片抽吸系统吹送吹风装置")]
        public bool A151M_Y130;         // 水松纸片抽吸系统吹送吹风装置

        [Description("导纸板支架水松纸片抽吸系统")]
        public bool A151M_Y132;         // 导纸板支架水松纸片抽吸系统

        [Description("水松纸边沿检测实际值")]
        public int B93M_rectification_actural_value;         // 水松纸边沿检测实际值

        [Description("胶水罐工作位置")]
        public bool B62M;         // 胶水罐工作位置

        [Description("加热器断纸检测")]
        public bool B65M;         // 加热器断纸检测

        [Description("水松纸抬纸杆到位")]
        public bool B67M;         // 水松纸抬纸杆到位

        [Description("胶缸/胶水室到位")]
        public bool B69M;         // 胶缸/胶水室到位

        [Description("胶堆检测")]
        public bool B73M;         // 胶堆检测

        [Description("胶水桶是否存在")]
        public bool B71M;         // 胶水桶是否存在

        [Description("胶水装置润滑油流量检测")]
        public bool S73M;         // 胶水装置润滑油流量检测

        [Description("水松纸加热")]
        public bool A152M_Y214;         // 水松纸加热

        [Description("供胶装置胶水罐")]
        public bool A152M_Y182;         // 供胶装置胶水罐

        [Description("胶辊电机运行状态")]
        public bool M5M_axis_enable_status;         // 胶辊电机运行状态

        [Description("胶水辊启动按钮")]
        public bool S3M;         // 胶水辊启动按钮

        [Description("胶水辊启动按钮灯")]
        public bool S3M_lamp;         // 胶水辊启动按钮灯

        [Description("备用")]
        public bool Reserved_03;         // 备用

        [Description("备用")]
        public bool Reserved_04;         // 备用

        [Description("备用")]
        public bool Reserved_05;         // 备用

        [Description("胶水辊停止按钮")]
        public bool S12M;         // 胶水辊停止按钮

        [Description("胶水缸翻入")]
        public bool S24M_left;         // 胶水缸翻入

        [Description("胶水缸翻出")]
        public bool S24M_right;         // 胶水缸翻出

        [Description("胶泵(输入)电机速度")]
        public int Glue_feed_pump_speed_value;         // 胶泵(输入)电机速度

        [Description("胶泵(抽吸)电机速度")]
        public int Glue_suction_pump_speed_value;         // 胶泵(抽吸)电机速度

        [Description("胶水重量")]
        public int Glue_level_tank;         // 胶水重量

        [Description("水松纸加热实际温度")]
        public int Tipping_heater_actual_tem_value;         // 水松纸加热实际温度

        [Description("水松纸提升器")]
        public bool A152M_Y190;         // 水松纸提升器

        [Description("水松纸片分离")]
        public bool A152M_Y196;         // 水松纸片分离

        [Description("水松纸切割装置清洁风吹除水松纸")]
        public bool A151M_Y124;         // 水松纸切割装置清洁风吹除水松纸

        [Description("负压控制系统吸尘器")]
        public bool A152M_Y168;         // 负压控制系统吸尘器

        [Description("M1M叠加速度值")]
        public int M1M_SpeedCompensation;         // M1M叠加速度值

        [Description("第一引纸辊电机状态")]
        public bool M1M;         // 第一引纸辊电机状态

        [Description("第二引纸辊电机状态")]
        public bool M3M;         // 第二引纸辊电机状态

        [Description("MAX切纸轮压力")]
        public float S78M;         // MAX切纸轮压力
    }
}
