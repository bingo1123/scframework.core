using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    /// <summary>
    /// 包装机停机数据
    /// </summary>
    [Serializable]
    public class PackMachineStopDataDto
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
        /// 机台部位
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
        /// 停机数据
        /// </summary>
        public List<MachineGd110DetailModel> STOPS { get; set; }

        /// <summary>
        ///  数采数据类型；A：当前活动数据；H：换牌结帐数据；E：交班结帐数据
        /// </summary>
        public string GATHERTYPE { get; set; } 
    }

    [Serializable]
    public struct MachineGd110DetailModel
    {
        /// <summary>
        /// 停机时长（秒）
        /// </summary>
        public int? STOPTIME { get; set; }
        /// <summary>
        /// 停机次数
        /// </summary>
        public int? STOPCNT { get; set; }
        /// <summary>
        /// 停机代码
        /// </summary>
        public object PM_MP_STOPCODE_ID { get; set; }
    }
}
