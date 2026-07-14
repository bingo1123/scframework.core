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
    [Description("当前班的停机历史信息")]
    public struct SHIS_ENTRY
    {
        /// <summary>
        /// p18 需要将uint bitCoverter.getBytes() 转换回去，然后byte[1] 与byte[0] 进行string拼接
        /// $"{bytes[1].ToString("X2")}{bytes[0].ToString("x2")}"
        /// </summary>
        [Description("停机编号")]
        public uint No;             // 停机编号

        [Description("停机的文本ID")]
        public uint Tid;            // 停机的文本ID编号

        [Description("停机开始时间（秒）")]
        public uint StartTime;      // 停机开始时间（秒）

        [Description("停机结束时间（秒）")]
        public uint EndTime;        // 停机结束时间（秒）

        [Description("停机持续时间（秒）")]
        public uint Downtime;       // 停机时间（秒）

        //-------M5 协议多的字段------------
        [Description("原始停机编号（如果停机被覆盖）")]
        public uint? OrgNo;          // 原始停机编号（如果停机被覆盖）

        [Description("扩展版的开始时间")]
        public DATETIME? StartTimeX; // 扩展版的开始时间

        [Description("扩展版的结束时间")]
        public DATETIME? EndTimeX;   // 扩展版的结束时间
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前班的停机历史信息列表")]
    ///<summary>
    ///当前班的停机历史数据列表，
    ///P18: 前三班停机历史数据保存在结构一样的SHI1、SHI2、SHI3中
    ///M5:  前三班停机历史数据保存在结构一样的SHIS1, SHIS2 and SHIS3
    ///M5协议中 所有的字段都是DWord 类型，P18所有字段是Word 类型
    ///</summary>
    public struct SHIS
    {
        [Description("条目数")]
        public uint Index;                           // 条目数

        [Description("是否记录停机分析")]
        public bool Analysis;                        // 是否记录停机分析(P18:长度四个字节,M5:8个字节)
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]

        [Description("停机历史列表")]
        public SHIS_ENTRY[] Stops;                   // 停机历史列表，最多200条记录
    }


    #region SHIS（停机历史）
    public class SHISParser
    {
        public static SHIS Parse(byte[] response, ProtocolTypeEnum typeEnum = ProtocolTypeEnum.M5)
        {
            SHIS shis = new SHIS();

            bool type = typeEnum == ProtocolTypeEnum.M5 || typeEnum == ProtocolTypeEnum.BOBME;
            bool type118 = typeEnum == ProtocolTypeEnum.ZJ118;
            int offset = type ? HuaniConst.M5ReadDataOffset : HuaniConst.P18ReadDataOffset;//p18解析的地址时从12开始的
            shis.Index = type ? BitConverter.ToUInt32(response, offset) : BitConverter.ToUInt16(response, offset);

            shis.Analysis = type ? BitConverter.ToUInt32(response, offset + 4) != 0 :
                type118 ? BitConverter.ToUInt32(response, offset + 2) != 0: BitConverter.ToUInt16(response, offset + 2) != 0;

            shis.Stops = new SHIS_ENTRY[shis.Index];
            offset += type ? 8 : type118? 6: 4;
            for (int i = 0; i < shis.Stops.Length; i++)
            {
                SHIS_ENTRY entry = new SHIS_ENTRY();


                entry.No = type ? BitConverter.ToUInt32(response, offset) : BitConverter.ToUInt16(response, offset);

                entry.Tid = type ? BitConverter.ToUInt32(response, offset + 4) : BitConverter.ToUInt16(response, offset + 2);

                entry.StartTime = type ? BitConverter.ToUInt32(response, offset + 8) : BitConverter.ToUInt32(response, offset + 4);
                entry.EndTime = type ? BitConverter.ToUInt32(response, offset + 12) : BitConverter.ToUInt32(response, offset + 8);

                entry.Downtime = type ? BitConverter.ToUInt32(response, offset + 16) : BitConverter.ToUInt32(response, offset + 12);
                if (type)
                {
                    entry.OrgNo = BitConverter.ToUInt32(response, offset + 20);//24
                    entry.StartTimeX = ByteArrayToDatetime(response, offset + 24);//7
                    entry.EndTimeX = ByteArrayToDatetime(response, offset + 31);//7
                }
                else
                {
                    entry.OrgNo = null;
                    entry.StartTimeX = null;
                    entry.EndTimeX = null;
                }
                shis.Stops[i] = entry;
                offset = type ? offset + 38 : offset + 16; // 每个条目占40字节
            }
            return shis;
        }

        public static DATETIME ByteArrayToDatetime(byte[] data, int offset)
        {
            DATETIME datetime = new DATETIME();
            datetime.Day = data[offset];
            datetime.Month = data[offset + 1];
            datetime.Year = BitConverter.ToUInt16(data, offset + 2);
            datetime.Hour = data[offset + 4];
            datetime.Minute = data[offset + 5];
            datetime.Second = data[offset + 6];
            return datetime;
        }

    }
    #endregion
}
