using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    ///// <summary>
    ///// 故障记录接口-设备报警信息（不导致停机）
    ///// </summary>
    //public class MalfunctionDataClient 
    //{
    //    private readonly ApiClient _client;

    //    public MalfunctionDataClient(ApiClientConfig config)
    //    {
    //        _client=ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<MalfunctionDataResponse> GetMalfunctionDataAsync()
    //    {
            
    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        Console.WriteLine(response);
    //        return JsonConvert.DeserializeObject<MalfunctionDataResponse>(response);
    //    }
    //}

    public class MalfunctionDataResponse
    {
        public int? Code { get; set; }
        public List<MalfunctionData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class MalfunctionData
    {
        /// <summary>
        /// 设备名称
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 产线名称
        /// </summary>
        public string? Place { get; set; }

        /// <summary>
        /// 故障名称
        /// </summary>
        public string? ErrorName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// Plc_name  单机编码
        /// </summary>
        public string? PlcName { get; set; }

        /// <summary>
        /// 故障原因 id
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// 班次编码（1 早班、2 午班、3 晚班）
        /// </summary>
        public int? shiftId { get; set; }

        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime? ShiftStartTime { get; set; }

        /// <summary>
        /// 故障开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 故障结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 故障等级
        /// </summary>
        public int? ErrorType { get; set; }
    }

}
