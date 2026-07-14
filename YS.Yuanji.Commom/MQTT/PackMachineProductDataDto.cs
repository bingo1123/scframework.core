using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    /// <summary>
    /// 包装机生产数据
    /// </summary>
    [Serializable]
    public class PackMachineProductDataDto
    {
        /// <summary>
        /// 采集日期
        /// </summary>
        public string PRODUCEDATE_IN { get; set; }

        /// <summary>
        /// 班次号
        /// </summary>
        public int PB_SHIFT_ID { get; set; }

        /// <summary>
        /// 上位机代码
        /// </summary>
        public string SWJDM { get; set; }

        /// <summary>
        /// 机台代码(下位机代码)
        /// </summary>
        public string XWJDM { get; set; }

        /// <summary>
        /// 部位(卷烟机时为0)
        /// </summary>
        public string PM_MP_MACHINEPART_ID { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 牌号代码
        /// </summary>
        public string PB_PRODUCT_ID { get; set; }

        /// <summary>
        /// 生产开始时刻
        /// </summary>
        public string? PRODUCTIONSTARTTIME { get; set; }

        /// <summary>
        /// 关电源时间（秒）
        /// </summary>
        public int POWEROFFTIME { get; set; }

        /// <summary>
        /// 准备时间（秒）
        /// </summary>
        public int PREPARATIONTIME { get; set; }

        /// <summary>
        /// 外部停机时间（秒）
        /// </summary>
        public int EXTERNALSTOPTIME { get; set; }

        /// <summary>
        /// 内部停机时间（秒）
        /// </summary>
        public int INTERNALSTOPTIME { get; set; }

        /// <summary>
        /// 运行时间（秒）
        /// </summary>
        public int RUNTIME { get; set; }

        /// <summary>
        /// 生产时间（秒）
        /// </summary>
        public int PRODUCETIME { get; set; }

        /// <summary>
        /// 待料时间（秒）
        /// </summary>
        public int WAITINGMATERIALTIME { get; set; }

        /// <summary>
        /// 单次最大速度运行时间（秒）
        /// </summary>
        public int MAXRUNTIME { get; set; }

        /// <summary>
        /// 平均运行时间（秒）
        /// </summary>
        public int? AVGRUNTIME { get; set; }

        /// <summary>
        /// 理论产量（包）
        /// </summary>
        public int? THEORETICALPRODUCTION { get; set; }

        /// <summary>
        /// 实际产量（包）
        /// </summary>
        public int? REALPRODUCTION { get; set; }

        /// <summary>
        /// 剔除数量（包）
        /// </summary>
        public int? REJECTPRODUCTION { get; set; }

        /// <summary>
        /// 生产效率
        /// </summary>
        public float? EFFPRODUCTION { get; set; }

        /// <summary>
        /// 机器效率
        /// </summary>
        public float? EFFMACHINE { get; set; }

        /// <summary>
        /// 停机次数
        /// </summary>
        public int? STOPCNT { get; set; }

        /// <summary>
        /// 机器速度（包/分钟）
        /// </summary>
        public int MACHINESPEED { get; set; }

        /// <summary>
        /// 额定速度（包/分钟）
        /// </summary>
        public int NORMALSPEED { get; set; }

        /// <summary>
        /// 预留，条烟产量
        /// </summary>
        public int? BARPRODUCTION { get; set; }

        /// <summary>
        /// 商标纸消耗张
        /// </summary>
        public int? LABELPAPER { get; set; }

        /// <summary>
        /// 条盒消耗张
        /// </summary>
        public int? BARREL { get; set; }

        /// <summary>
        /// 铝箔纸消耗张
        /// </summary>
        public int? ALUMFOILPAPER { get; set; }

        /// <summary>
        /// 内框纸消耗张
        /// </summary>
        public int? INTERNALFRAMEPAPER { get; set; }

        /// <summary>
        /// 小包透明纸消耗张
        /// </summary>
        public int? SMALLTRANSPAPER { get; set; }

        /// <summary>
        /// 条包透明纸消耗张
        /// </summary>
        public int? LOAFTRANSPAPER { get; set; }

        /// <summary>
        /// 封签纸消耗张
        /// </summary>
        public int? SEALPAPER { get; set; }

        /// <summary>
        /// 金拉线消耗米
        /// </summary>
        public int? GOLDTHREAD { get; set; }

        /// <summary>
        /// 小包透明纸消耗(米)
        /// </summary>
        public int? SMALLTRANSPAPERMI { get; set; }

        /// <summary>
        /// 条包透明纸消耗(米)
        /// </summary>
        public int? LOAFTRANSPAPERMI { get; set; }

        /// <summary>
        /// 铝箔纸消耗(米)
        /// </summary>
        public int? ALUMFOILPAPERMI { get; set; }

        /// <summary>
        /// 内框纸消耗(米)
        /// </summary>
        public int? INTERNALFRAMEPAPERMI { get; set; }

        /// <summary>
        /// 生产计划工单号
        /// </summary>
        public string PLANWORKORDERNO { get; set; }

        /// <summary>
        /// 数采数据类型；A：当前活动数据；H：换牌结帐数据；E：交班结帐数据
        /// </summary>
        public string GATHERTYPE { get; set; }
    }
}
