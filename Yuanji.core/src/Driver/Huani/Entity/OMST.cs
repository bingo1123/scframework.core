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
    /// 当前运行/停机时间 只适用于ZJ116,ZJ119
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Description("当前运行/停机时间")]
    public struct OMST
    {
        [Description("当前运行时长[秒]")]
        public uint actRunTime;

        [Description("当前停机时长[秒]")]
        public uint actDowntime;
    }

    public class OMSTParser
    {
        public OMST Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.M5)
        {
            OMST omst = new OMST();
            int offset = HuaniConst.P18ReadDataOffset;
            omst.actRunTime = BitConverter.ToUInt32(response, offset);
            omst.actDowntime = BitConverter.ToUInt32(response, offset + 4);
            return omst;
        }
    }
}
