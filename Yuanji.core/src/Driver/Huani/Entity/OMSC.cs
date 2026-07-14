using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using  Yuanji.core.src.Driver.Huani.Controller;
using YS.Yuanji.Commom;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    ///<Summary>
    /// 当前班次的生产、停机、操作模式的时间
    /// P18: 前三班保存在OMS1、OMS2、OMS3中
    /// M5: 前三班保存在 OMSC1, OMSC2 and OMSC3 中
    ///</Summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前班次的生产、停机、操作模式的时间")]
    [Serializable]
    public struct OMSC
    {
        [Description("班次持续时间(秒)")]
        public uint ShiftTime;       // 

        [Description("休息持续时间(秒)")]
        public uint BreakTime;       // 当前休息持续时间（秒）

        [Description("生产持续时间(秒)")]
        public uint ProductionTime;  // 当前生产持续时间（秒）

        [Description("总停机次数")]
        public uint TotalStopCount;  // 停机总次数

        [Description("停机总时间（内部/外部总计）")]
        public uint TotalStopTime;   // 停机总时间（内部/外部总计）

        [Description("内部停机持续时长(秒)")]
        public uint InternalStopTime; // 内部停机持续时长（秒）

        [Description("外部停机持续时长(秒)")]
        public uint ExternalStopTime; // 外部停机持续时长（秒）

        [Description("班次实际开始时间(秒)")]
        public uint ShiftStart;      // 当前班次开始时间（秒）

        [Description("班次运行时间(秒)")]
        public uint RunTime;         // 当前班次运行时间（秒）



        [Description("生产操作模式")]
        public uint OpmProduction;   // 生产操作模式

        [Description("等待状态操作模式")]
        public uint OpmWaitingStatus; // 等待状态操作模式

        [Description("维修操作模式")]
        public uint OpmRepair;       // 维修操作模式

        [Description("维护/检修操作模式")]
        public uint OpmMaintenance;  // 维护/检修操作模式

        [Description("测试/修改操作模式")]
        public uint OpmTest;         // 测试/修改操作模式

        [Description("清理操作模式")]
        public uint OpmCleanup;      // 清理操作模式

        [Description("停机操作模式")]
        public uint OpmStandStill;   // p18协议就在这里为止

        //-----M5-----
        [Description("内部停机次数")]
        public uint? InternalStopCount; // 内部停机次数

        [Description("外部停机次数")]
        public uint? ExternalStopCount; // 外部停机次数

        [Description("停机之间的平均运行时间(秒)")]
        public uint? AverageRuntime;    // 停机之间的平均运行时间（秒）
    }

    #region OMSC（生产时间和停机时间）
    public class OMSCParser 
    {
        public static OMSC Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.M5)
        {
            OMSC omsc = new OMSC();
            int offset;
            if (type == ProtocolTypeEnum.M5 || type == ProtocolTypeEnum.BOBME)
                offset = HuaniConst.M5ReadDataOffset;
            else
                offset = HuaniConst.P18ReadDataOffset;

            omsc.ShiftTime = BitConverter.ToUInt32(response, offset);
            omsc.BreakTime = BitConverter.ToUInt32(response, offset + 4);
            omsc.ProductionTime = BitConverter.ToUInt32(response, offset + 8);
            omsc.TotalStopCount = BitConverter.ToUInt32(response, offset + 12);
            omsc.TotalStopTime = BitConverter.ToUInt32(response, offset + 16);
            omsc.InternalStopTime = BitConverter.ToUInt32(response, offset + 20);
            omsc.ExternalStopTime = BitConverter.ToUInt32(response, offset + 24);
            omsc.ShiftStart = BitConverter.ToUInt32(response, offset + 28);
            omsc.RunTime = BitConverter.ToUInt32(response, offset + 32);
            omsc.OpmProduction = BitConverter.ToUInt32(response, offset + 36);
            omsc.OpmWaitingStatus = BitConverter.ToUInt32(response, offset + 40);
            omsc.OpmRepair = BitConverter.ToUInt32(response, offset + 44);
            omsc.OpmMaintenance = BitConverter.ToUInt32(response, offset + 48);
            omsc.OpmTest = BitConverter.ToUInt32(response, offset + 52);
            omsc.OpmCleanup = BitConverter.ToUInt32(response, offset + 56);
            omsc.OpmStandStill = BitConverter.ToUInt32(response, offset + 60);

            if (type == ProtocolTypeEnum.M5)
            {
                omsc.InternalStopCount = BitConverter.ToUInt32(response, offset + 64);
                omsc.ExternalStopCount = BitConverter.ToUInt32(response, offset + 68);
                omsc.AverageRuntime = BitConverter.ToUInt32(response, offset + 72);
            }
            else 
            {
                omsc.InternalStopCount = null;
                omsc.ExternalStopCount = null;
                omsc.AverageRuntime = null;
            }
            return omsc;
        }
    }
    #endregion
}
