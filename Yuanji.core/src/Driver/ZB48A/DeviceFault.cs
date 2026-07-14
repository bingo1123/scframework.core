// 获取设备故障信息
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{

    //public class DeviceFaultClient 
    //{
    //    private readonly ApiClient _client;

    //    public DeviceFaultClient(ApiClientConfig config)
    //    {
    //        _client=ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<DeviceFaultResponse> GetDeviceFaultAsync()
    //    {
           
    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        return JsonConvert.DeserializeObject<DeviceFaultResponse>(response);
    //    }

    //    //public DeviceFaultResponse GetDeviceFault()
    //    //{
    //    //    string endpoint = "/api/haltEquipment";
    //    //    var response =  _client.Get(endpoint, new Dictionary<string, string>());
    //    //    return JsonConvert.DeserializeObject<DeviceFaultResponse>(response);
    //    //}
    //}

    public class DeviceFaultResponse
    {
        public int? Code { get; set; }
        public List<DeviceFaultData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class DeviceFaultData
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
        /// 停机原因 id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 停机原因
        /// </summary>
        public string? PauseReason { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// 单机编码
        /// </summary>
        public string? Plc_name { get; set; }

        /// <summary>
        /// 班次编码（1 早班、2 午班、3 晚班）
        /// </summary>
        public int? ShiftId { get; set; }

        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime? ShiftStartTime { get; set; }

        /// <summary>
        /// 停机开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 停机结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}

