using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.Huani.Controller
{
    public enum SymbAdrEnum
    {
        ACFA,
        ACDI,
        SLVL,

        /// <summary>
        /// stop analysis
        /// 前三个班次使用SAN1,SAN2,SAN3,
        /// </summary>
        SANA,

        /// <summary>
        /// stop history of the machine.
        /// 前三个班次使用SHIS1,SHIS2,SHIS3,
        /// 在Protos 1-8协议中，前三个班次使用SHI1,SHI2,SHI3,
        /// </summary>
        SHIS,

        /// <summary>
        /// 当前班次机器生产时间和停止时间的数据
        /// 前三个班次使用OMSC1, OMSC2 and OMSC3
        /// </summary>
        OMSC,

        /// <summary>
        /// 存储轮班时间（周日 = 0，...，周六 = 6）。
        ///<remark>输入的时间以秒为单位，从周日 0 时开始计算。
        ///如果开始时间和结束时间均为 0，则不定义条目。
        ///一天最多可定义 4 个班次。一天最多可定义 4 个班次。</remark>
        /// </summary>
        SHFT,

        /// <summary>
        /// 存储的是休息时间（周日 = 0，...，周六 = 6）。
        /// <remark>
        /// 时间以秒为单位，从周日 0 点开始计算。
        ///如果开始时间和结束时间均为 0，则不定义条目。
        ///每班最多可定义 3 次休息时间。每班最多可定义 3 次休息时间。
        /// </remark>
        /// </summary>
        BRKT,

        ProductionCounter,

        ShiftData,

        //以下是Protos1-8新加的
        /// <summary>
        /// 实际系统消息
        /// </summary>
        ACSY,

        /// <summary>
        /// 实际操作员消息
        /// </summary>
        ACOP,

        /// <summary>
        /// MLP Message List。 最多可存储 100 条故障信息和状态栏备注。未使用的条目将设为 0。
        /// </summary>
        SCRO,

        /// <summary>
        /// 故障编号/故障文本分配
        /// 故障文本被分配给故障编号。
        /// </summary>
        ENRO,

        /// <summary>
        /// 状态信息
        /// </summary>
        STAT,

        /// <summary>
        /// 
        /// </summary>
        PROC,
        WASP,
        OMST,

        /// <summary>
        /// 总线用户的状态设定点和变量。
        /// </summary>
        SP01,

        /// <summary>
        /// MLP 参数
        /// </summary>
        SP02,

        /// <summary>
        /// operator/shift allocation
        /// The index points to the relevant operator name in the list of names 
        /// stored under the symbolic address OPNM
        /// </summary>
        OPSH,

        /// <summary>
        /// 机器部件运行时监控的数据
        /// </summary>
        INSP,

        /// <summary>
        /// 当前品牌和所有品牌名称的列表
        /// </summary>
        BRND,

        /// <summary>
        /// 当前的班次数据。
        /// 前三次班次数据使用SHF1,SHF2,SHF3
        /// </summary>
        SHFD,

        /// <summary>
        /// 当前班次的机器停机历史记录.其内容与地址 SHIS 相同，但使用不同的时间格式。
        /// 前三个班次的数据使用SHX1,SHX2,SHX3
        /// </summary>
        SHXS,

        /// <summary>
        /// 与机器运行模式有关的当前班次的数据。其内容与地址 OMSC 相同，但使用不同的时间格式。
        /// 前三个班次的数据存储在地址 OMX1、OMX2 和 OMX3 下。
        /// </summary>
        OMXC,

        /// <summary>
        /// 当前班次的ProductionCounter
        /// </summary>
        CNT1,

        /// <summary>
        /// 前一个班次的ProductionCounter
        /// </summary>
        CNT2,

        /// <summary>
        /// 过去 10,000 支香烟的生产计数器。
        /// </summary>
        CNT3,
        OPMT,
        ACST,
        ZIGH,
        BPTR,
        TEMP,
        TEST,
        GNRL,
        MDRM,
        TPGL,
        QUAL,
        QUAC,
        msst,
        tsth,
        tmpm,
        zsgb,
        GSST,
        RSST,
        CSST,
        PSST,
        NSST,
        SSST,
        ITOS,
        SMN4
    }
}
