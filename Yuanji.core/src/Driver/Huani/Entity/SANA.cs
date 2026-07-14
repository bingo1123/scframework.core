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
    /// <summary>
    /// 当前班的停机分析信息
    /// M5 协议中这些字段为Uint，P18为Ushort<br/>
    /// 前三班停机分析数据保存在结构一样的SAN1、SAN2、SAN3中
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前班的停机分析信息")]
    public struct SANA_ENTRY
    {
        [Description("停机编号")]
        public uint No;         // 停机编号

        [Description("停机的文本ID")]
        public uint Tid;        // 停机的文本ID编号

        [Description("此停机当前班发生次数")]
        public uint Count;      // 此停机当前班发生次数

        [Description("停机类型：0内部/1外部")]
        public uint SType;      // 停机类型：0内部/1外部

        [Description("停机总持续时间(秒)")]
        public uint Downtime;   // 停机总持续时间（秒）
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前班的停机分析列表")]
    public struct SANA
    {
        [Description("总停机分析个数")]
        public uint Index;                               // 总停机原因个数

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        [Description("停机原因表")]
        public SANA_ENTRY[] Stops;                       // 停机原因表
    }

    #region SANA（停机分析）
    public class SANAParser 
    {
        public static SANA Parse(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.P18)
        {
            SANA sana = new SANA();
            int offset;
            bool isM5Type = typeEnum == ProtocolTypeEnum.M5 || typeEnum == ProtocolTypeEnum.BOBME;
            bool is118Type = typeEnum == ProtocolTypeEnum.ZJ118;
            offset = isM5Type ? HuaniConst.M5ReadDataOffset : HuaniConst.P18ReadDataOffset;
            sana.Index = isM5Type ? BitConverter.ToUInt32(response, offset) : BitConverter.ToUInt16(response, offset);
            sana.Stops = new SANA_ENTRY[sana.Index];
            offset = isM5Type ? offset + 4 : offset + 2;
            for (int i = 0; i < sana.Stops.Length; i++)
            {
                SANA_ENTRY entry = new SANA_ENTRY();
                entry.No = isM5Type ? BitConverter.ToUInt32(response, offset ) : BitConverter.ToUInt16(response, offset );
                entry.Tid = isM5Type ? BitConverter.ToUInt32(response, offset + 4) : BitConverter.ToUInt16(response, offset + 2);
                entry.Count = isM5Type ? BitConverter.ToUInt32(response, offset + 8) : BitConverter.ToUInt16(response, offset + 4);
                entry.SType = isM5Type ? BitConverter.ToUInt32(response, offset + 12) : BitConverter.ToUInt16(response, offset + 6);
                entry.Downtime = isM5Type ? BitConverter.ToUInt32(response, offset + 16) : BitConverter.ToUInt32(response, offset + 8);
                sana.Stops[i] = entry;
                offset = isM5Type ? offset + 20 : offset + 12; // 每个条目占20字节
            }
            return sana;
        }
    }
    #endregion
}
