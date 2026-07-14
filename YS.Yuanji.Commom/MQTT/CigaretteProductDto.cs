using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  YS.Yuanji.Commom
{
    /// <summary>
    /// 卷烟机生产信息
    /// </summary>
    [Serializable]
    public class CigaretteProductDto
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
        public string SWJDM {  get; set; }

        /// <summary>
        /// 机台代码(下位机代码)
        /// </summary>
        public string XWJDM {  get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public int NO {  get; set; }

        /// <summary>
        /// 牌号代码
        /// </summary>
        public string PB_PRODUCT_ID {  get; set; }

        /// <summary>
        /// 总产量(千支)
        /// </summary>
        public int TOTALPRODUCTION {  get; set; }

        /// <summary>
        /// 咀棒消耗(千支)
        /// </summary>
        public int FILTERTIP { get; set; }

        /// <summary>
        /// 盘纸消耗米
        /// </summary>
        public float PAPER {  get; set; }

        /// <summary>
        /// 水松纸消耗米
        /// </summary>
        public float WPAPER {  get; set; }

        /// <summary>
        /// 烟丝消耗公斤
        /// </summary>
        public float TobaccoSilk { get; set; }

        /// <summary>
        /// 跑条数
        /// </summary>
        public float PAOTIAOS {  get; set; }

        /// <summary>
        /// 盘纸消耗盘
        /// </summary>
        public int PAPERPAN {  get; set; }

        /// <summary>
        /// 水松纸消耗盘
        /// </summary>
        public int WPAPERPAN { get; set; }

        /// <summary>
        /// 剔除率
        /// </summary>
        public float TOTALWASTEPCT {  get; set; }

        /// <summary>
        /// 剔除量
        /// </summary>
        public int TOTALWASTE { get; set; }

        /// <summary>
        /// 机器效率
        /// </summary>
        public float EFFMACHINE {  get; set; }

        /// <summary>
        /// 生产效率
        /// </summary>
        public float EFFPRODUCTION {  get; set; }

        /// <summary>
        /// 机器速度
        /// </summary>
        public int MACHINESPEED {  get; set; }

        /// <summary>
        /// 运行时间
        /// </summary>
        public int RUNTIME { get; set; }

        /// <summary>
        /// 生产开始时间
        /// </summary>
        public string PRODUCTIONSTARTTIME { get; set; }

        /// <summary>
        /// 停机时间
        /// </summary>
        public int TOTALSTOPTIME { get; set; }

        /// <summary>
        /// 停机次数
        /// </summary>
        public int TOTALSTOPCNT { get; set; }

        /// <summary>
        ///  测试车速停机次数
        /// </summary>
        public int TESTSPEEDSTOPCNT { get; set; }

        /// <summary>
        /// 工单号
        /// </summary>
        public string batch {  get; set; }

        /// <summary>
        ///  数采数据类型；A：当前活动数据；H：换牌结帐数据；E：交班结帐数据
        /// </summary>
        public string GATHERTYPE {  get; set; }
    }
}
