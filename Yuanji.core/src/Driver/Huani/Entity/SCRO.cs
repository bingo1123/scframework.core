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
    /// 故障信息和状态栏注释 只适用于ZJ116
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("故障信息和状态栏注释")]
    [Obsolete]
    public struct SCRO_ENTRY
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string Msg;         // 单条消息的缓冲区
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("MLP消息列表")]
    public struct SCRO
    {
        public ushort Index;                            // 当前条目数量
        public ushort LogIndex;                         // 下一个条目的索引
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public SCRO_ENTRY[] Lst;                        // 所有消息的列表
    }

    [Obsolete]
    public class SCROParser /*: ISymbEntityParser<SCRO>*/
    {
        public SCRO Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.M5)
        {
            SCRO sCRO = new SCRO();
            int offset = HuaniConst.P18ReadDataOffset;
            sCRO.Index = BitConverter.ToUInt16(response, offset);
            sCRO.Lst = new SCRO_ENTRY[sCRO.Index];
            for (int i = 0; i < sCRO.Lst.Length; i++)
            {
                SCRO_ENTRY sCRO_ENTRY = new SCRO_ENTRY();
                byte[] str = new byte[81];
                Array.Copy(response, offset + 2, str, 0, str.Length);
                sCRO_ENTRY.Msg = Encoding.ASCII.GetString(str);
                offset += 83;
            }
            return sCRO;
        }
    }
}
