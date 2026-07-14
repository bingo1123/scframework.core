using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{

    //public class HourConsumptionClient
    //{
    //    private readonly ApiClient _client;

    //    public HourConsumptionClient(ApiClientConfig config)
    //    {
    //        config.Port = 8189;
    //        _client = ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<EnergyConsumptionResponse> GetEnergyConsumptionAsync(string equipmentCode)
    //    {

    //        var parameters = new Dictionary<string, string> { { "equipmentCode", equipmentCode } };
    //        var response =await _client.GetAsync(endpoint, parameters);
    //        return JsonConvert.DeserializeObject<EnergyConsumptionResponse>(response);
    //    }
    //}

    public class EnergyConsumptionResponse
    {
        public int? Code { get; set; }
        public List<EnergyConsumptionData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class EnergyConsumptionData
    {
        /// <summary>
        /// 测点编码（主机 Q416 辅机 Q511611）
        /// </summary>
        public string? SampleCode { get; set; }

        /// <summary>
        /// 时间点
        /// </summary>
        public DateTime? SendHour { get; set; }

        /// <summary>
        /// 该时间消耗量
        /// </summary>
        public double? HourConsumption { get; set; }

        /// <summary>
        /// 是否为当天数据
        /// </summary>
        public bool? IsToday { get; set; }
    }

}
