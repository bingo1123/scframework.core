using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    /*
        #define UNDEFINED 0L // Status undefined
        #define EVENT_OFF 0L // Event has not occurred
        #define STATE_OFF 1L // Status OFF
        #define EVENT_ON 2L // Event has occurred
        #define STATE_ON 3L // Status ON
     */

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("机器状态信息")]
    public struct MACHINE_STATE
    {
        [Description("SE/VE 启动按钮启动事件")]
        public ushort EVT_StartButtonSEVE;           // SE/VE 启动按钮

        [Description("SE/VE 运行状态(1:Off / 3:ON)")]
        public ushort STA_SEVErunning;               // SE/VE 运行状态

        [Description("SE 将香烟传递到 MAX 的状态")]
        public ushort STA_Production;                // SE 将香烟传递到 MAX 的状态

        [Description("MAX 点动按钮状态")]
        public ushort EVT_PushButtonMAX;             // MAX 点动按钮状态

        [Description("MAX 运行状态(1:Off / 3:ON)")]
        public ushort STA_MAXrunning;                // MAX 运行状态

        [Description("烟草杆取样盘")]
        public ushort STA_BowlRod;                   // 烟草杆取样盘

        [Description("烟支取样盘")]
        public ushort STA_BowlCigarettes;            // 烟支取样盘

        [Description("程序停止事件")]
        public ushort EVT_ProgramStop;               // 程序停止事件

        [Description("核制剂驶出状态")]
        public ushort STA_PraeparatAusgefahren;      // 核制剂驶出

        [Description("立即停止事件")]
        public ushort EVT_SofortStopp;               // 立即停止事件

        [Description("出现副传动关的停机事件")]
        public ushort EVT_StoppMitNebenantriebeAus;  // 出现副传动关的停机

        [Description("SAM 可用状态")]
        public ushort STA_SAM_Available;             // SAM 可用状态

        [Description("CIS 设备状态")]
        public ushort STA_CIS_2;                     // CIS 设备状态

        [Description("跨接下游机")]
        public ushort STA_LinkUpMachine;             // zj116中为备用

        [Description("无滤嘴烟支")]
        public ushort STA_Plain_Cigarettes;          // zj116中为备用

        [Description("激光打孔转动装置已存在")]
        public ushort STA_Laser_Roll;                        // zj116中为备用
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("采样控制系统状态")]
    public struct EST_STATE
    {
        [Description("使用采样的总线用户ID")]
        public byte NodeID;                          // 使用采样的总线用户ID

        [Description("填充字节")]
        public byte Filler;                          // 填充字节

        [Description("报告编号")]
        public ushort ReportNo;                      // 报告编号

        [Description("报告标题")]
        public ushort ReportTid;                     // 报告标题

        [Description("采样编号")]
        public ushort SampleNo;                      // 采样编号

        [Description("灯状态")]
        public byte Lamp;                            // 灯状态

        [Description("取样盘满状态")]
        public bool BowlFilled;                      // TRUE: 盘已填充(长度四个字节)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("系统状态")]
    public struct SYSTEM_STATE
    {
        [Description("使能 0: AUS（关）/ 1:EIN（开）")]
        public ushort UpdateEnabled;                 // 使能 0: AUS（关）/ 1:EIN（开）

        [Description("钥匙开关")]
        public ushort KeySwitch;                     // 钥匙开关

        [Description("班次状态")]
        public ushort ShiftState;                    // 班次状态

        [Description("生产数据采集")]
        public ushort ProductionData;                // 生产数据采集

        [Description("参数检查")]
        public ushort ParameterCheck;                // 参数检查

        [Description("备用2")]
        public ushort Reserve2;                      //备用

        [Description("备用1")]
        public ushort Reserve1;                      //备用
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("机器状态、采样控制系统和 MLP系统的信息")]
    public struct STAT
    {
        [Description("中心机器状态")]
        public MACHINE_STATE MachineState;           // 中央机器状态字

        [Description("AWS 采样状态")]
        public byte AWS_State;                       // AWS 采样状态

        [Description("EST 采样状态")]
        public EST_STATE EST_State;                  // EST 采样状态

        [Description("系统状态")]
        public SYSTEM_STATE SystemState;             // 系统状态

        [Description("停机活动状态")]
        public bool StopActive;                      // 停机活动状态(长度四个字节)

        [Description("停机消息显示状态")]
        public bool StopMsgLatched;                  // 停机消息显示状态(长度四个字节)


        //--------以下为ZJ119多出来的字段
        [Description("最后按键的时间点")]
        public UInt32 LastKeyEvent;                  // ZJ19多出来的数据

        [Description("SP停止激活状态")]
        public bool SPStopActive;                    // SP停止激活状态(长度四个字节)
    }

}
