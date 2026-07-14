using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Commom
{
    public class MqttConst
    {
        public const string CollectDateFormat = "yyyy-MM-dd";

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public const string HHmmssFormat = "HH:mm:ss";

        public const string MqttTopicProductData = "102";

        public const string MqttTopicReject = "1057";

        /// <summary>
        /// 停机数据
        /// </summary>
        public const string MqttTopicStopData = "110";

        /// <summary>
        /// 停机流水
        /// </summary>
        public const string MqttTopicStopInfo = "SS";

        /// <summary>
        /// 实时数据
        /// </summary>
        public const string MqttTopicRealTime = "realtime";

        /// <summary>
        /// 写入数据
        /// </summary>
        public const string Subwrite = "device/{0}/write";

        /// <summary>
        /// 写入返回
        /// </summary>
        public const string Pubwrite = "device/{0}/wresponse";
    }
}
