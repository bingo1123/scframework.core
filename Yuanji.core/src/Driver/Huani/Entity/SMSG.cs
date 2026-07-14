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
    [Description("当前激活的采样信息")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public struct SMSG_ENTRY
    {
        [Description("采样信息编号")]
        public ushort No;

        [Description("信息时间戳")]
        public UInt32 Time;
    }

    [Description("当前激活的采样信息列表")]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public struct SMSG
    {
        [Description("条目数")]
        public ushort Index;                    // SMSG_ENTRY索引

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
        [Description("信息列表")]
        public SMSG_ENTRY[] Messages;           // 消息列表
    }

    public class SMSGParser
    {
        public static SMSG Parser(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            SMSG sMSG = new SMSG();
            int offset = HuaniConst.P18ReadDataOffset;
            sMSG.Index = BitConverter.ToUInt16(response, offset);
            sMSG.Messages = new SMSG_ENTRY[sMSG.Index];
            for (int i = 0; i < sMSG.Messages.Length; i++)
            {
                SMSG_ENTRY eNTRY = new SMSG_ENTRY();
                eNTRY.No = BitConverter.ToUInt16(response, offset + 2);
                eNTRY.Time = BitConverter.ToUInt32(response, offset + 4);
                offset += 6;
            }

            return sMSG;
        }
    }
}
