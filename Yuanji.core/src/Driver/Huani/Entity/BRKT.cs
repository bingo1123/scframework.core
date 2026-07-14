using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using  Yuanji.core.src.Driver.Huani.Controller;
using YS.Yuanji.Commom;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("班次设定时间")]
    [Serializable]
    public struct TIME
    {
        [Description("每班开始时间(秒)")]
        public uint Start; // 开始时间（秒）

        [Description("每班结束时间(秒)")]
        public uint End;   // 结束时间（秒）
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("一周内已设定的休息时间")]
    [Serializable]
    public struct BRKT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]

        public TIME[][][] Breaks; // 7天，每天4个班次，每班次3个休息时间

        public BRKT()
        {
            Breaks = new TIME[7][][];
            for (int i = 0; i < 7; i++)
            {
                Breaks[i] = new TIME[4][];
                for (int j = 0; j < 4; j++)
                {
                    Breaks[i][j] = new TIME[3];
                }
            }
        }
    }

    #region BRKT（休息时间）
    public class BRKTParser 
    {
        public static BRKT Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.M5)
        {
            BRKT brkt = new BRKT();
            int offset = HuaniConst.M5ReadDataOffset;

            for (int day = 0; day < 7; day++)
            {
                for (int shift = 0; shift < 4; shift++)
                {
                    for (int breakIndex = 0; breakIndex < 3; breakIndex++)
                    {
                        brkt.Breaks[day][shift][breakIndex].Start = BitConverter.ToUInt32(response, offset);
                        brkt.Breaks[day][shift][breakIndex].End = BitConverter.ToUInt32(response, offset + 4);
                        offset += 8; // 每个休息时间占8字节
                    }
                }
            }

            return brkt;
        }
    }
    #endregion
}
