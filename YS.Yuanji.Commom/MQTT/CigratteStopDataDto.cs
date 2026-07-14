using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    /// <summary>
    /// 卷烟机停机数据
    /// </summary>
    [Serializable]
    public class CigratteStopDataDto
    {
        /// <summary>
        /// 采集时间  
        /// </summary>
        public string PRODUCEDATE_IN {  get; set; }

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
        public string XWJDM {  get; set; }

        /// <summary>
        /// 部位(卷烟机时为0)
        /// </summary>
        public string PM_MP_MACHINEPART_ID { get; set; } = "0";

        /// <summary>
        /// 批次号
        /// </summary>
        public int NO {  get; set; }

        /// <summary>
        /// 牌号代码
        /// </summary>
        public string PB_PRODUCT_ID {get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        public string PLANWORKORDERNO { get; set; }

        /// <summary>
        /// 工单执行号
        /// </summary>
        public string PLANNO {  get; set; }

        ///// <summary>
        ///// 停机代码
        ///// </summary>
        //public string PM_MP_STOPCODE_ID {  get; set; }

        ///// <summary>
        ///// 停机次数
        ///// </summary>
        //public int STOPCNT {  get; set; }

        ///// <summary>
        ///// 停机时间
        ///// </summary>
        //public int STOPTIME { get; set; }

        /// <summary>
        /// 停机数据
        /// </summary>
        public List<MachineGd110DetailModel> STOPS { get; set; }

        /// <summary>
        /// 数采数据类型；A：当前活动数据；H：换牌结帐数据；E：交班结帐数据
        /// </summary>
        public string GATHERTYPE {  get; set; }
    }
}
