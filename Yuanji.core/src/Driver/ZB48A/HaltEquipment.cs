using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
 
    public class HaltEquipment
    {
        public int? Code { get; set; }
        public List<HaltEquipmentData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class HaltEquipmentData
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string? equipmentName { get; set; }

        /// <summary>
        /// 单机名称
        /// </summary>
        public string? plcName { get; set; }

        /// <summary>
        /// 停机原因id
        /// </summary>
        public int? haltId { get; set; }

        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime? shiftStartTime { get; set; }

        /// <summary>
        /// 班次编码
        /// </summary>
        public int? shiftId { get; set; }

        /// <summary>
        /// 停机开始时间
        /// </summary>
        public string? startTime { get; set; }

        /// <summary>
        /// 停机结束时间
        /// </summary>
        public string? endTime { get; set; }

        /// <summary>
        /// 停机原因
        /// </summary>
        public string? pauseReason { get; set; }

        /// <summary>
        /// 产线名称
        /// </summary>
        public string? place { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public float? Percent { get; set; }
    }

}
