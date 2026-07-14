using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
  
    //public class RejectDataClient
    //{
    //    private readonly ApiClient _client;

    //    public RejectDataClient(ApiClientConfig config)
    //    {
    //        _client = ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<RejectDataResponse> GetRejectDataAsync()
    //    {
    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        return JsonConvert.DeserializeObject<RejectDataResponse>(response);
    //    }
    //}

    public class RejectDataResponse
    {
        public int? Code { get; set; }
        public List<RejectData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class RejectData
    {
        /// <summary>
        /// 产线名称
        /// </summary>
        public string? Place { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// 单机编码
        /// </summary>
        public string? PlcCode { get; set; }

        /// <summary>
        /// 班次编码（1 早班、2 午班、3 晚班）
        /// </summary>
        public int? ShiftId { get; set; }

        /// <summary>
        /// 剔除原因 id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 剔除原因中文名称
        /// </summary>
        public string? CounterName { get; set; }

        /// <summary>
        /// 剔除值
        /// </summary>
        public int? Value { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createDateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? updateDateTime { get; set; }

        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime? shiftStartTime { get; set; }

        /// <summary>
        /// 各剔除原因占比
        /// </summary>
        public float? RejectRate { get; set; }
    }
}
