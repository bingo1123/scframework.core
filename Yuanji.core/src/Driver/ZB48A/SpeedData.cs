// 查询设备实际车速
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    ///// <summary>
    ///// 实际车速
    ///// <remark>设备各个单机的实际车速。</remark>
    ///// </summary>
    //public class SpeedDataClient 
    //{
    //    private readonly ApiClient _client;

    //    public SpeedDataClient(ApiClientConfig config)
    //    {
    //        config.Port = 8189;
    //        _client = ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<object> GetSpeedDataAsync()
    //    {

    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        try
    //        {
    //            //Console.WriteLine(response);
    //            return JsonConvert.DeserializeObject<SpeedDataResponse>(response);
    //        }
    //        catch (Exception ex)
    //        {
    //            return JsonConvert.DeserializeObject<SpeedDataResponseAn>(response);
    //        }
    //    }
    //}

    public class SpeedDataResponse
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long? SendTime { get; set; }

        public List<SpeedMap>? SpeedMap { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }
    }

    public class SpeedDataResponseAn
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public long? SendTime { get; set; }

        public Dictionary<string, int>? SpeedMap { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }
    }


    public class SpeedMap
    {
        /// <summary>
        /// YB416 车速
        /// </summary>
        public int? XX { get; set; }

        /// <summary>
        /// YB511 车速
        /// </summary>
        public int? BV { get; set; }

        /// <summary>
        /// YB611 车速
        /// </summary>
        public int? C800 { get; set; }
    }
}

