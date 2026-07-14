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
    /// <summary>
    /// 当前班的启动阶段的停机历史数据，此标签只适用于ZJ119
    /// 前三班停机历史数据保存在结构一样的SPS1、SPS2、SPS3中
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前班的启动阶段的停机历史数据")]
    public struct SPST_ENTRY
    {
        [Description("停机消息号")]
        public ushort No;                     // 停机消息号

        [Description("停机原因文本号")]
        public ushort Tid;                    // 停机原因文本号

        [Description("停机原因开始时间")]
        public uint StartTime;                // 停机原因开始时间

        [Description("停机原因结束时间")]
        public uint EndTime;                  // 停机原因结束时间

        [Description("停机原因持续时间")]
        public uint Downtime;                 // 停机原因持续时间
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前班的启动阶段的停机历史数据列表")]
    public struct SPST
    {
        public ushort Index;                  // 停机原因总个数
        public bool Spare;                    // 未使用(长度四个字节)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public SPST_ENTRY[] Lst;              // 停机表
    }

    public class SPATParser
    {
        public static SPST Parser(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.ZJ119)
        {
            SPST sPST = new SPST();
            int offset = HuaniConst.P18ReadDataOffset;
            sPST.Index = BitConverter.ToUInt16(response, offset);
            sPST.Spare = BitConverter.ToUInt32(response, offset + 2) == 0;
            sPST.Lst = new SPST_ENTRY[sPST.Index];
            for (int i = 0; i < sPST.Lst.Length; i++)
            {
                SPST_ENTRY eNTRY = new SPST_ENTRY();
                eNTRY.No = BitConverter.ToUInt16(response, offset + 6);
                eNTRY.Tid = BitConverter.ToUInt16(response, offset + 8);
                eNTRY.StartTime=BitConverter.ToUInt32(response,offset + 10);
                eNTRY.EndTime = BitConverter.ToUInt32(response, offset + 14);
                eNTRY.Downtime = BitConverter.ToUInt32(response, offset + 18);
                offset += 16;
            }
            return sPST;
        }
    }
}
