using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  Yuanji.core.src.Driver.Huani.Controller;
using YS.Yuanji.Commom;

namespace  Yuanji.core.src.Driver.Huani.Entity
{
    [Description("production counter")]
    [Obsolete]
    public struct CNTX
    {
        /// <summary>
        /// MAX 排放点的单支香烟编号（由 B7M 检测到）。
        /// 该数字显示在基本报告中（卷烟产量）。
        /// </summary>
        [Description("MAX 排放点的单支香烟编号")]
        public UInt32 Max_AUSGANG { get; set; }

        /// <summary>
        /// MAX 入口点的单支卷烟编号（由 B0M 检测）。
        /// MAX_EINGANG 和 MAX_AUSGANG 之间的差值在基本报告中显示为废品。
        /// </summary>
        [Description("MAX 入口点的单支卷烟编号")]
        public UInt32 MAX_EINGANG { get; set; }


        /// <summary>
        /// 丢失的过滤嘴卷烟编号（卷烟掉落在检查鼓区的卷烟）的数量。
        /// 该数字显示在分析报告中。
        /// </summary>
        [Description("丢失的过滤嘴卷烟编号")]
        public UInt32 ABGEFALLEN_BEREICH_4 { get; set; }

        /// <summary>
        /// 检测到含有金属的香烟编号。
        /// 数字显示在分析报告中
        /// </summary>
        [Description("检测到含有金属的香烟编号")]
        public UInt32 METIS_ENTFERNT { get; set; }
    }

    [Obsolete]
    public class CNTXarser
    {
        public static CNTX Parse(byte[] response, ProtocolTypeEnum type = ProtocolTypeEnum.P18)
        {
            CNTX cnt = new CNTX();
            int offset = HuaniConst.P18ReadDataOffset;
            cnt.Max_AUSGANG = BitConverter.ToUInt32(response, offset);
            cnt.MAX_EINGANG = BitConverter.ToUInt32(response, offset + 4);
            cnt.ABGEFALLEN_BEREICH_4 = BitConverter.ToUInt32(response, offset + 8);
            cnt.METIS_ENTFERNT = BitConverter.ToUInt32(response, offset + 16);
            return cnt;
        }
    }
}
