using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    public class ParameterDataDto
    {
        /// <summary>
        /// 采集日期
        /// </summary>
        public string PRODUCEDATE_IN { get; set; }

        /// <summary>
        ///  班次号
        /// </summary>
        public int PB_SHIFT_ID { get; set; }

        /// <summary>
        /// 上位机代码
        /// </summary>
        public int SWJDM { get; set; }

        /// <summary>
        /// 机台代码(下位机代码)
        /// </summary>
        public int XWJDM { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public int NO { get; set; }

        /// <summary>
        /// 牌号代码
        /// </summary>
        public string PB_PRODUCT_ID { get; set; }


        public List<PARAMETER> PARAMETERS {  get; set; }


        public string GATERDATETIME {  get; set; }
    }

    [Serializable]
    public class PARAMETER
    {
        public string PARAMETERCODE { get; set; }     

        public int PARAMETERVAL {  get; set; }
    }
}
