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
    /// node 1,func:2--zj119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("LHC信息")]
    public struct LHCS
    {
        [Description("通信状态 周期方波信号")]
        public bool Com_bit;         // 通信状态 周期方波信号

        [Description("LHC-启停状态")]
        public bool StartStop_state;         // LHC-启停状态

        [Description("LHC-补水状态")]
        public bool Replenishment_state;         // LHC-补水状态

        [Description("LHC-水泵运行状态")]
        public bool OpState_water_pump;         // LHC-水泵运行状态

        [Description("LHC-加热器运行状态")]
        public bool Heater_operation_status;         // LHC-加热器运行状态

        [Description("LHC-电磁阀1运行状态")]
        public bool Solenoid_valve_1_operating_sta;         // LHC-电磁阀1运行状态

        [Description("LHC-电磁阀2运行状态")]
        public bool Solenoid_valve_2_operating_sta;         // LHC-电磁阀2运行状态

        [Description("LHC-电磁阀3运行状态")]
        public bool Solenoid_valve_3_operating_sta;         // LHC-电磁阀3运行状态

        [Description("LHC-电磁阀4运行状态")]
        public bool Solenoid_valve_4_operation_sta;         // LHC-电磁阀4运行状态

        [Description("LHC-液位过低")]
        public bool Low_level;         // LHC-液位过低

        [Description("LHC-相序错误")]
        public bool Phase_sequence_error;         // LHC-相序错误

        [Description("LHC-水泵过载")]
        public bool Water_pump_overload;         // LHC-水泵过载

        [Description("LHC-加热器过载")]
        public bool Heater_overload;         // LHC-加热器过载

        [Description("LHC-循环流量过低")]
        public bool Circulating_flow_too_low;         // LHC-循环流量过低

        [Description("LHC-循环水温过高")]
        public bool Circulating_water_tem_high;         // LHC-循环水温过高

        [Description("LHC-储液水箱温度过高")]
        public bool Reservoir_tem_high;         // LHC-储液水箱温度过高

        [Description("LHC-液位预警")]
        public bool Level_warning;         // LHC-液位预警

        [Description("LHC-水压力报警")]
        public bool WaterPressure_alarm;         // LHC-水压力报警

        [Description("预留")]
        public bool reserve18;         // 预留

        [Description("预留")]
        public bool reserve19;         // 预留

        [Description("预留")]
        public bool reserve20;         // 预留

        [Description("预留")]
        public bool reserve21;         // 预留

        [Description("预留")]
        public bool reserve22;         // 预留

        [Description("预留")]
        public bool reserve23;         // 预留

        [Description("储液水箱水温")]
        public int Water_tem_liquid_storage_tank;         // 储液水箱水温

        [Description("循环出口水温")]
        public int Circulating_outlet_water_tem;         // 循环出口水温

        [Description("循环进口水温")]
        public int Circulating_inlet_water_tem;         // 循环进口水温

        [Description("循环水流量")]
        public int Circulating_water_flow;         // 循环水流量

        [Description("循环出口压力")]
        public int Circulating_outlet_pressure;         // 循环出口压力

        [Description("换热器进风温度")]
        public int Air_inlet_tem_heat_exchanger;         // 换热器进风温度

        [Description("空气分配箱温度")]
        public int Air_distribution_box_tem;         // 空气分配箱温度
    }
}
