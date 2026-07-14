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
    /// 当前激活的操作员消息
    /// 此标签只适用于ZJ119,ZJ116
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前激活的操作员消息")]
    [Serializable]
    public struct ACOP_ENTRY
    {
        [Description("消息编号")]
        public ushort No;            // 消息编号

        [Description("文本ID")]
        public ushort Tid;           // 文本索引

        [Description("帮助索引")]
        public ushort Hid;           // 帮助索引

        [Description("消息优先级")]
        public ushort Priority;      // 消息优先级

        [Description("保留字段")]
        public ushort Dummy;         // 保留字段
    }


    /// <summary>
    /// 当前激活的操作员列表
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前激活的操作员列表")]
    [Serializable]
    public struct ACOP
    {
        public ushort Index;                           // 条目数

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public ACOP_ENTRY[] Message;                   // 操作员消息数组
    }



    public class ACOPParser
    {
        public ACOP Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.P18)
        {
            ACOP aCOP = new ACOP();
            aCOP.Index = BitConverter.ToUInt16(response, HuaniConst.P18ReadDataOffset);
            aCOP.Message = new ACOP_ENTRY[aCOP.Index];
            int offset = HuaniConst.P18ReadDataOffset;

            for (int i = 0; i < aCOP.Message.Length; i++)
            {
                ACOP_ENTRY aCOP_ENTRY = new ACOP_ENTRY();
                aCOP_ENTRY.No = BitConverter.ToUInt16(response, offset + 2);
                aCOP_ENTRY.Tid = BitConverter.ToUInt16(response, offset + 4);
                aCOP_ENTRY.Hid = BitConverter.ToUInt16(response, offset + 6);
                aCOP_ENTRY.Priority = BitConverter.ToUInt16(response, offset + 8);
                aCOP_ENTRY.Dummy = BitConverter.ToUInt16(response, offset + 10);
                offset += 10;
            }
            return aCOP;
        }
    }
}
