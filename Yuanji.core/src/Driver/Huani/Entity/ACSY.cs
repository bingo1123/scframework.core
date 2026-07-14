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
    /// ACSY（实际系统消息），只适用于 PROTOS 1-8 ,ZJ119
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前激活的系统消息")]
    [Serializable]
    public struct ACSY_ENTRY
    {
        [Description("节点ID")]
        public byte Node;           // 节点 ID

        [Description("功能编号")]
        public byte Function;       // 功能编号

        [Description("消息编号")]
        public ushort No;           // 消息编号

        [Description("文本ID")]
        public ushort Tid;          // 文本 ID

        [Description("帮助索引")]
        public ushort Hid;          // 帮助索引

        [Description("消息优先级")]
        public ushort Priority;     // 消息优先级

        [Description("保留字段")]
        public ushort Dummy;        // 保留字段
    }

    /// <summary>
    /// 当前激活的系统消息列表
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前激活的系统消息列表")]
    [Serializable]
    public struct ACSY
    {
        public ushort Index;                         // 条目数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public ACSY_ENTRY[] Message;                // 系统故障消息的数组，最好采用集合，这样也不会出现超出数组大小的集合
    }


    public class ACSYParser
    {
		
		public ACSY Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.P18)
        {
            ACSY result = new ACSY();
            int Offset = HuaniConst.P18ReadDataOffset;//12表示符号地址的开始数据
            result.Index = BitConverter.ToUInt16(response, Offset);//获取数组实际长度，其占了两个字节
            result.Message = new ACSY_ENTRY[Math.Min((ushort)100, result.Index)];

            for (int i = 0; i < result.Message.Length; i++)
            {
                ACSY_ENTRY aCSY_ENTRY = new ACSY_ENTRY();

                aCSY_ENTRY.Node = response[Offset + 2];
                aCSY_ENTRY.Function = response[Offset + 3];

                aCSY_ENTRY.No = BitConverter.ToUInt16(response, Offset + 4);
                aCSY_ENTRY.Tid = BitConverter.ToUInt16(response, Offset + 6);
                aCSY_ENTRY.Hid = BitConverter.ToUInt16(response, Offset + 8);
                aCSY_ENTRY.Priority = BitConverter.ToUInt16(response, Offset + 10);
                aCSY_ENTRY.Dummy = BitConverter.ToUInt16(response, Offset + 12);
                Offset += 12;
            }
            return result;
        }
    }
}
