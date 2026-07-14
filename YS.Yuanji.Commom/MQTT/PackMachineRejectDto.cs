using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    [Serializable]
    public class PackMachineRejectDto
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
        /// 机台代码
        /// </summary>
        public string XWJDM { get; set; }

        /// <summary>
        /// 机台位(卷烟机时为0)
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

        public List<PackRejectInfo> REJECTS { get; set; }

        /// <summary>
        /// 数采数据类型；A：当前活动数据；H：换牌结帐数据；E：交班结帐数据
        /// </summary>
        public string GATHERTYPE { get; set; }
    }

    [Serializable]
    public struct PackRejectInfo
    {  
        /// <summary>
       /// 剔除项Id
       /// </summary>
        public string PM_MP_REJECTCODE_ID { get; set; }

        /// <summary>
        /// 剔除数
        /// </summary>
        public string REJECTVALUE { get; set; }

    }
}
