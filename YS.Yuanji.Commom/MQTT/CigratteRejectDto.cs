using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    /// <summary>
    /// 卷烟机剔除数据
    /// </summary>
    [Serializable]
    public class CigratteRejectDto
    {
        /// <summary>
        /// 采集时间
        /// </summary>
        public string PRODUCEDATE_IN { get; set; }

        /// <summary>
        /// 班次号
        /// </summary>
        public int PB_SHIFT_ID { get; set; } = 1;

        /// <summary>
        /// 上位机代码
        /// </summary>
        public string SWJDM { get; set; } = string.Empty;

        /// <summary>
        /// 机台代码(下位机代码)
        /// </summary>
        public string XWJDM { get; set; } = "JJ101";

        /// <summary>
        /// 批次号
        /// </summary>
        public int NO { get; set; } = 0;

        /// <summary>
        /// 部位(卷烟机时为0)
        /// </summary>
        public string PM_MP_MACHINEPART_ID = "0";

        /// <summary>
        /// 牌号代码
        /// </summary>
        public string PB_PRODUCT_ID { get; set; } = "hongtashan1956";

        public List<RejectInfo> REJECTS { get; set; }

        /// <summary>
        /// 数采数据类型；A：当前活动数据；H：换牌结帐数据；E：交班结帐数据
        /// </summary>
        public string GATHERTYPE { get; set; }
    }

    public struct RejectInfo
    {
        /// <summary>
        /// 剔除代码
        /// </summary>
        public string PM_MP_REJECTCODE_ID { get; set; }

        /// <summary>
        /// 剔除值
        /// </summary>
        public int REJECTVALUE { get; set; }
    }
}
