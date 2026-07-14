using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
 
    //public class HaltAnalysisClient 
    //{
    //    private readonly ApiClient _client;

    //    public HaltAnalysisClient(ApiClientConfig config)
    //    {
    //        _client = ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<HaltAnalysisResponse> GetHaltAnalysisAsync()
    //    {
           
    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        return JsonConvert.DeserializeObject<HaltAnalysisResponse>(response);
    //    }

    //    //public HaltAnalysisResponse GetHaltAnalysis()
    //    //{
    //    //    string endpoint = "/api/haltAnalysis";
    //    //    var response =  _client.Get(endpoint, new Dictionary<string, string>());
    //    //    return JsonConvert.DeserializeObject<HaltAnalysisResponse>(response);
    //    //}
    //}

    public class HaltAnalysisResponse
    {
        public int? Code { get; set; }
        public List<HaltAnalysisData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class HaltAnalysisData
    {
        /// <summary>
        /// 设备编码
        /// </summary>
        public string? equipmentCode { get; set; }

        /// <summary>
        /// 单机名称
        /// </summary>
        public string? plcName { get; set; }

        /// <summary>
        /// 停机次数
        /// </summary>
        public int? count { get; set; }

        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime? shiftStartTime { get; set; }

        /// <summary>
        /// 班次编码
        /// </summary>
        public int? shiftId { get; set; }

        /// <summary>
        /// 总时长
        /// </summary>
        public string? totalTime { get; set; }

        /// <summary>
        /// 最大时长
        /// </summary>
        public string? MaxTime { get; set; }

        /// <summary>
        /// 平均时长
        /// </summary>
        public string? AvgTime { get; set; }

        /// <summary>
        /// 停机原因
        /// </summary>
        public string? LabelChinese { get; set; }

        /// <summary>
        /// 占比
        /// </summary>
        public float? Percent { get; set; }
    }

}
