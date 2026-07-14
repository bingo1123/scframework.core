// 查询设备参数
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{

    //public class DeviceParameterClient 
    //{
    //    private readonly ApiClient _client;

    //    public DeviceParameterClient(ApiClientConfig config)
    //    {
    //        _client=ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<DeviceParameterResponse> GetDeviceParameterAsync()
    //    {
           
    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        return JsonConvert.DeserializeObject<DeviceParameterResponse>(response);
    //    }

    //    //public DeviceParameterResponse GetDeviceParameter()
    //    //{
    //    //    string endpoint = "/api/parameterEquipment";
    //    //    var response =  _client.Get(endpoint, new Dictionary<string, string>());
    //    //    return JsonConvert.DeserializeObject<DeviceParameterResponse>(response);
    //    //}
    //}

    public class DeviceParameterResponse
    {
        public int? Code { get; set; }
        public List<DeviceParameterData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class DeviceParameterData
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 参数所属部位编码
        /// <remark>（xx 主机、cb 主机电柜、cs 铝箔纸装载、
        ///bv 条盒条透、bt 条盒装载、
        ///be条盒提升、ch透明纸小包、
        ///cr 辅机电柜）</remark>
        /// </summary>
        public string? PlcCode { get; set; }

        /// <summary>
        /// 所属部件
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// 所属功能
        /// </summary>
        public string? FunctionName { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string? ValDefault { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        public string? ValMin { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public string? ValMax { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string? Unit { get; set; }

        /// <summary>
        /// 参数中文名
        /// </summary>
        public string? Paracn { get; set; }

        /// <summary>
        /// 状态 1 表示被删除 0 未删除
        /// </summary>
        public bool? Status { get; set; } 

        /// <summary>
        /// 当前值
        /// </summary>
        public string? ValCurrent { get; set; }


        /// <summary>
        /// 产线名称
        /// </summary>
        public string? PlaceName { get; set; }

        public override bool Equals(object obj) => obj is DeviceParameterData p && PlcCode == p.PlcCode && Paracn == p.Paracn;
        public override int GetHashCode() => (PlcCode+Paracn)?.GetHashCode() ?? 0;
    }
}

