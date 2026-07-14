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
    [Description("SE 信息")]
    public struct SE_GNRL
    {
        [Description("左压纸辊旋钮")]
        public bool S7S;         // 左压纸辊旋钮

        [Description("第一引纸辊压紧回转")]
        public bool A160S_Y21;         // 第一引纸辊压紧回转

        [Description("第一引纸辊电机")]
        public bool M1S;         // 第一引纸辊电机

        [Description("断纸检测")]
        public bool B21S;         // 断纸检测

        [Description("纸宽调整电机")]
        public bool M24S;         // 纸宽调整电机

        [Description("第二引纸辊压紧回转")]
        public bool A160S_Y22;         // 第二引纸辊压紧回转

        [Description("钢印位置调整电机")]
        public bool M10S;         // 钢印位置调整电机

        [Description("纸张力辊电机")]
        public bool M2S;         // 纸张力辊电机

        [Description("纸宽度检测传感器")]
        public bool B19S;         // 纸宽度检测传感器

        [Description("第二引纸辊电机")]
        public bool M11S;         // 第二引纸辊电机

        [Description("前道纸边高度控制电机")]
        public bool M25S;         // 前道纸边高度控制电机

        [Description("右压纸辊旋钮")]
        public bool S8S;         // 右压纸辊旋钮

        [Description("切割辊刀架")]
        public bool A160S_Y39;         // 切割辊刀架

        [Description("后道纸边高度控制电机")]
        public bool M26S;         // 后道纸边高度控制电机

        [Description("胶泵电机")]
        public bool M12S;         // 胶泵电机

        [Description("切割压紧辊转入")]
        public bool A160S_Y25;         // 切割压紧辊转入

        [Description("卷烟纸切刀")]
        public bool A140S_Y70;         // 卷烟纸切刀

        [Description("前道纸高电机实际位置")]
        public int M25S_actual_poistion;         // 前道纸高电机实际位置

        [Description("后道纸高电机实际位置")]
        public int M26S_actual_poistion;         // 后道纸高电机实际位置

        [Description("纸宽调整电机实际位置")]
        public int M24S_actual_poistion;         // 纸宽调整电机实际位置

        [Description("烟纸切断")]
        public bool S12S;         // 烟纸切断

        [Description("点动")]
        public bool S13S;         // 点动

        [Description("穿纸输送")]
        public bool S9S;         // 穿纸输送

        [Description("SE润滑油料位")]
        public float SE_oil_level;         // SE润滑油料位

        [Description("SE润滑油温度")]
        public float SE_oil_temperature;         // SE润滑油温度

        [Description("SE润滑油粘度")]
        public float SE_oil_viscosity;         // SE润滑油粘度

        [Description("备用")]
        public int Empty;         // 备用

        [Description("左供墨装置加料")]
        public bool A160S_Y32;         // 左供墨装置加料

        [Description("左油墨料位")]
        public ushort ink_level_left;         // 左油墨料位

        [Description("左油墨温度")]
        public int E55S;         // 左油墨温度

        [Description("左钢印下压辊摆动")]
        public bool A160S_Y23;         // 左钢印下压辊摆动

        [Description("左油墨计量泵")]
        public bool M3S;         // 左油墨计量泵

        [Description("左钢印辊电机")]
        public bool M5S;         // 左钢印辊电机

        [Description("左匀墨辊电机")]
        public bool M4S;         // 左匀墨辊电机

        [Description("左供墨装置已使用")]
        public bool A160S_Y27;         // 左供墨装置已使用

        [Description("右供墨装置加料")]
        public bool A160S_Y34;         // 右供墨装置加料

        [Description("右油墨料位")]
        public ushort ink_level_right;         // 右油墨料位

        [Description("右油墨温度")]
        public int E56S;         // 右油墨温度

        [Description("右钢印下压辊摆动")]
        public bool A160S_Y24;         // 右钢印下压辊摆动

        [Description("右油墨计量泵")]
        public bool M6S;         // 右油墨计量泵

        [Description("右钢印辊电机")]
        public bool M8S;         // 右钢印辊电机

        [Description("右匀墨辊电机")]
        public bool M7S;         // 右匀墨辊电机

        [Description("右供墨装置已使用")]
        public bool A160S_Y29;         // 右供墨装置已使用

        [Description("左油墨加热")]
        public bool V55S;         // 左油墨加热

        [Description("右油墨加热")]
        public bool V56S;         // 右油墨加热

        [Description("备用")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] Cigarette_paper_printer_reserve;   // 备用

        [Description("加胶水")]
        public bool A160S_Y31;         // 加胶水

        [Description("胶水加料状态显示")]
        public bool H1S;         // 胶水加料状态显示

        [Description("胶水箱料位")]
        public ushort B17S;         // 胶水箱料位

        [Description("前道喷嘴体")]
        public bool A160S_Y37;         // 前道喷嘴体

        [Description("喷嘴针伸缩")]
        public bool A160S_Y36;         // 喷嘴针伸缩

        [Description("胶水加料启动")]
        public bool S16S;         // 胶水加料启动

        [Description("胶水加料启动灯")]
        public bool S16S_H1;         // 胶水加料启动灯

        [Description("备用")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] seam_gule_reserve;         // 备用

        [Description("机器准备好")]
        public bool machine_operational;         // 机器准备好

        [Description("机器自动")]
        public bool machine_auto;         // 机器自动

        [Description("备用")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] seam_gule_reserve1;         // 备用

        [Description("前封口器温度")]
        public int E57S_E1;         // 前封口器温度

        [Description("前封口器加热")]
        public bool V57S;         // 前封口器加热

        [Description("封口器下降")]
        public bool A160S_Y13;         // 封口器下降

        [Description("封口器上升")]
        public bool A160S_Y15;         // 封口器上升

        [Description("后封口器温度")]
        public int E57S_E2;         // 后封口器温度

        [Description("后封口器加热")]
        public bool V58S;         // 后封口器加热

        [Description("封口器上升检测")]
        public bool B5S;         // 封口器上升检测

        [Description("封口器防护罩")]
        public bool B6_1S;         // 封口器防护罩

        [Description("封口器防护罩")]
        public bool B6_2S;         // 封口器防护罩

        [Description("前道直径控制电机")]
        public bool M13S;         // 前道直径控制电机

        [Description("后道直径控制电机")]
        public bool M14S;         // 后道直径控制电机

        [Description("后道断纸检测")]
        public bool B18S;         // 后道断纸检测

        [Description("前道断纸检测")]
        public bool B16S;         // 前道断纸检测

        [Description("烟枪温度")]
        public int R1S;         // 烟枪温度

        [Description("烟枪冷却阀")]
        public bool Y60S;         // 烟枪冷却阀

        [Description("打条器生产位置")]
        public bool A160S_Y85;         // 打条器生产位置

        [Description("打条器启动位置")]
        public bool A160S_Y86;         // 打条器启动位置

        [Description("前道直径控制电机实际位置")]
        public int M13S_actual_poistion;         // 前道直径控制电机实际位置

        [Description("后道直径控制电机实际位置")]
        public int M14S_actual_poistion;         // 后道直径控制电机实际位置

        [Description("封口器降下检测")]
        public bool S14S;         // 封口器降下检测

        [Description("烟沫吸除装置")]
        public bool B7S;         // 烟沫吸除装置

        [Description("布带张紧压力")]
        public float B57S;         // 布带张紧压力

        [Description("切刀进刀")]
        public bool A160S_Y11;         // 切刀进刀

        [Description("进刀次数")]
        public int Cuts_before_knife_advance;         // 进刀次数

        [Description("前道烟条启动")]
        public bool B8S;         // 前道烟条启动

        [Description("后道烟条启动")]
        public bool B9S;         // 后道烟条启动

        [Description("切刀磨刀电机一")]
        public bool M102_1S;         // 切刀磨刀电机一

        [Description("切刀磨刀电机二")]
        public bool M102_2S;         // 切刀磨刀电机二

        [Description("切刀长度检查")]
        public bool B42S;         // 切刀长度检查

        [Description("刀盘在干涉区")]
        public bool knife_carri_interfere;         // 刀盘在干涉区

        [Description("喇叭嘴在干涉区")]
        public bool carousel_interfere;         // 喇叭嘴在干涉区

        [Description("废烟车料位检测")]
        public ushort B45S;         // 废烟车料位检测

        [Description("垂直废烟输送带")]
        public bool M105S;         // 垂直废烟输送带

        [Description("垂直废烟输送带启动")]
        public bool S18S;         // 垂直废烟输送带启动

        [Description("垂直废烟输送带启动灯")]
        public bool S18S_H1;         // 垂直废烟输送带启动灯

        [Description("垂直废烟输送带防护板")]
        public bool S17S;         // 垂直废烟输送带防护板

        [Description("传递装置清洁吹气")]
        public bool A140S_Y42;         // 传递装置清洁吹气

        [Description("传递机构清洁吹气")]
        public bool A140S_Y46;         // 传递机构清洁吹气

        [Description("菱形导轨阀剔出")]
        public bool Y114S;         // 菱形导轨阀剔出

        [Description("垂直废烟输送带堵塞检测")]
        public bool B46S;         // 垂直废烟输送带堵塞检测

        [Description("备用")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] waste_conveyor_reserve;         // 备用

        [Description("切刀刀罩")]
        public bool S4S;         // 切刀刀罩

        [Description("刀罩连锁")]
        public bool S32S;         // 刀罩连锁

        [Description("切刀托架防护门")]
        public bool S26S;         // 切刀托架防护门

        [Description("烟枪防护罩门")]
        public bool S2S;         // 烟枪防护罩门

        [Description("烟枪布带门")]
        public bool S27S;         // 烟枪布带门

        [Description("左印刷器门")]
        public bool S28S;         // 左印刷器门

        [Description("右印刷器门")]
        public bool S29S;         // 右印刷器门

        [Description("中央动力柜左门")]
        public bool S12_1;         // 中央动力柜左门

        [Description("中央动力柜右门")]
        public bool S12_2;         // 中央动力柜右门

        [Description("中央控制柜门")]
        public bool S12P;         // 中央控制柜门

        [Description("SE控制柜门")]
        public bool S1S;         // SE控制柜门

        [Description("SE后机器室左门")]
        public bool S34S;         // SE后机器室左门

        [Description("SE后机器室右门")]
        public bool S35S;         // SE后机器室右门

        [Description("打条器护罩")]
        public bool S15S;         // 打条器护罩

        [Description("备用")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] Hoods_reserve;         // 备用

        [Description("机器速度")]
        public int machine_speed;         // 机器速度

        [Description("SE机器速度")]
        public int SE_machine_speed;         // SE机器速度

        [Description("机器速度手动设置")]
        public int machine_speed_manual;         // 机器速度手动设置

        [Description("前道钢印位置偏移")]
        public int printer_postion_offset_front;         // 前道钢印位置偏移

        [Description("后道钢印位置偏移")]
        public int printer_postion_offset_rear;         // 后道钢印位置偏移

        [Description("第一引纸辊补偿速度")]
        public int M1S_Speed_Add;         // 第一引纸辊补偿速度

        [Description("伺服指令执行结果")]
        public byte Servo_instruct_feed;         // 伺服指令执行结果

        [Description("蜘蛛手真空泵压力")]
        public float B56S;         // 蜘蛛手真空泵压力

        [Description("油墨气缸校准结果")]
        public byte ink_position_initialization_feed;         // 油墨气缸校准结果

        [Description("备用")]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Operate_reserve;         // 备用

        [Description("相电压_A相 (V)")]
        public float P0_Vab;         // 相电压_A相 (V)

        [Description("相电压_B相 (V)")]
        public float P0_Vbc;         // 相电压_B相 (V)

        [Description("相电压_C相 (V)")]
        public float P0_Vca;         // 相电压_C相 (V)

        [Description("线电压_AB (V)")]
        public float P0_Van;         // 线电压_AB (V)

        [Description("线电压_BC (V)")]
        public float P0_Vbn;         // 线电压_BC (V)

        [Description("线电压_CA (V)")]
        public float P0_Vcn;         // 线电压_CA (V)

        [Description("电流_A相 (A)")]
        public float P0_Ia;         // 电流_A相 (A)

        [Description("电流_B相 (A)")]
        public float P0_Ib;         // 电流_B相 (A)

        [Description("电流_C相 (A)")]
        public float P0_Ic;         // 电流_C相 (A)

        [Description("电流_中性线 (A)")]
        public float P0_In;         // 电流_中性线 (A)

        [Description("频率(Hz)")]
        public float P0_FQ;         // 频率(Hz)

        [Description("有功功率(kW)")]
        public float P0_kW;         // 有功功率(kW)

        [Description("无功功率(kVar)")]
        public float P0_kVar;         // 无功功率(kVar)

        [Description("视在功率(kVA)")]
        public float P0_kVA;

        [Description("功率因数")]
        public float P0_PF;

        [Description("正向有功电度 (kWh)")]
        public int P0_pkWh;

        [Description("反向有功电度 (kWh)")]
        public int P0_nkWh;

        [Description("正向无功电度 (kVarh)")]
        public int P0_pkVarh;

        [Description("反向无功电度 (kVarh)")]
        public int P0_nkVarh;

        [Description("视在电度 (kVAh)")]
        public int P0_kVAh;

        [Description("正向有功功率需量 (kW)")]
        public float P0_DmdpkW;

        [Description("反向有功功率需量 (kW)")]
        public float P0_DmdnkW;

        [Description("总耗气量(m3)")]
        public float air_press_flow;

        [Description("SE胶水重量（kg）")]
        public float glue_weight;

        [Description("总气源压力")]
        public float air_press_all_B55S;

        [Description("内循环温度")]
        public int B61S;

        [Description("内循环压力")]
        public int B62S;

        [Description("VE温度")]
        public int B81V;

        [Description("VE压力")]
        public int B80V;

        [Description("VE风机室温度")]
        public int S85V;

        [Description("SE温度")]
        public int B63S;

        [Description("SE压力")]
        public int B64S;

        [Description("MAX温度")]
        public int B110M;

        [Description("MAX压力")]
        public int B112M;

        [Description("MAX风机柜温度")]
        public int S90M;

        [Description("MAX阀岛柜温度")]
        public int S91M;

        [Description("外循环流量调节阀")]
        public int Y61S;

        [Description("水冷泵状态（开/关）")]
        public bool M1A;

        [Description("内循环流量监测状态")]
        public bool B60S;

        [Description("内循环水位监测状态")]
        public bool S60S;

        [Description("中央动力柜温度监测状态")]
        public bool S90;

        [Description("中央控制柜温度监测状态")]
        public bool S90P;

        [Description("SE控制柜温度监测状态")]
        public bool S90S;

        [Description("MAX控制柜温度监测状态")]
        public bool S45M;

        [Description("VE控制柜温度监测状态")]
        public bool S84V;

        [Description("烟枪冷却水流量")]
        public float B65S;

        [Description("A相增量脉冲计数值")]
        public int A_SEEncoderCounterVal;

        [Description("A相增量脉冲锁存值")]
        public int A_SEEncoderLatchVal;

        [Description("校准砝码称出重量")]
        public float Glue_weight_calibration;
    }
}
