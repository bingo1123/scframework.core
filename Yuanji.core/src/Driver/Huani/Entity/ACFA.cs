using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;
using  Yuanji.core.src.Driver.Huani.Controller;

namespace  Yuanji.core.src.Driver.Huani.Entity
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前活动的故障信息")]
    [Serializable]
    public struct ACFA_ENTRY
    {
        [Description("消息编号")]
        public uint No;         // 消息编号

        [Description("消息的文本ID")]
        public uint Tid;        // 消息文本编号

        [Description("消息优先级")]
        public uint Priority;   // 消息优先级

        [Description("消息标志：活跃/需要确认")]
        public uint Flags;      // 消息标志：活跃/需要确认

        [Description("保留字段")]
        public ushort? Dummy;        // PROTOS 1-8 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前激活的故障信息列表")]
    [Serializable]
    public struct ACFA
    {
        [Description("条目数")]
        public uint Index;                            // 条目数， 在 PROTOS 1-8 此结构体中的Uint 都为UShort
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public ACFA_ENTRY[] Messages;                 // 停止/警告/信息消息
    }

    #region ACFA
    public class ACFAParser
    {
        public static ACFA Parse(byte[] response, ProtocolTypeEnum e = ProtocolTypeEnum.M5)
        {
            ACFA acfa = new ACFA();
            bool M5flag = e == ProtocolTypeEnum.M5 || e == ProtocolTypeEnum.BOBME;
            int offset = M5flag ? HuaniConst.M5ReadDataOffset : HuaniConst.P18ReadDataOffset;
            acfa.Index = M5flag ? BitConverter.ToUInt32(response, offset) : BitConverter.ToUInt16(response, offset);
            acfa.Messages = new ACFA_ENTRY[acfa.Index];
            for (int i = 0; i < acfa.Messages.Length; i++)
            {
                ACFA_ENTRY entry = new ACFA_ENTRY();
                entry.No = M5flag ? BitConverter.ToUInt32(response, offset + 4) : BitConverter.ToUInt16(response, offset + 2); // 消息编号
                entry.Tid = M5flag ? BitConverter.ToUInt32(response, offset + 8) : BitConverter.ToUInt16(response, offset + 4); // 文本ID
                entry.Priority = M5flag ? BitConverter.ToUInt32(response, offset + 12) : BitConverter.ToUInt16(response, offset + 6);// 优先级
                entry.Flags = M5flag ? BitConverter.ToUInt32(response, offset + 16) : BitConverter.ToUInt16(response, offset + 8); // 消息标志
                if (M5flag)
                    entry.Dummy = null;
                else
                    entry.Dummy = BitConverter.ToUInt16(response, offset + 10);
                acfa.Messages[i] = entry;  // 将解析的条目存储到数组中

                offset = M5flag ? offset + 16 : offset + 10; 
            }
            return acfa;
        }
    }
    #endregion
}
