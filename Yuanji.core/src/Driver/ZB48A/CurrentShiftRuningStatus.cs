// 查询机器性能
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    ///// <summary>
    ///// 机器性能
    ///// <remark>
    ///// 运行时间、等待时间、停机时间、最大运行时长、平均运行时长、设备运行效率、设备综合效率、设备状态等
    ///// </remark>
    ///// </summary>
    //public class MachinePerformanceClient 
    //{
    //    private readonly ApiClient _client;

    //    public MachinePerformanceClient(ApiClientConfig config)
    //    {
    //        _client = ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<MachinePerformanceResponse> GetMachinePerformanceAsync()
    //    {

    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        return JsonConvert.DeserializeObject<MachinePerformanceResponse>(response);
    //    }

    //    //public MachinePerformanceResponse GetMachinePerformance()
    //    //{
    //    //    string endpoint = "/runningStatus/getCurrentShiftRunningStatus";
    //    //    var response =  _client.Get(endpoint, new Dictionary<string, string>());
    //    //    return JsonConvert.DeserializeObject<MachinePerformanceResponse>(response);
    //    //}
    //}
    /// <summary>
    /// 机器性能
    /// </summary>
    public class MachinePerformanceResponse
    {
        public int? Code { get; set; }
        public List<MachinePerformanceData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class MachinePerformanceData
    {
        /// <summary>
        /// 产线名称
        /// </summary>
        public string? placeName { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string? equipmentName { get; set; }

        /// <summary>
        /// 单机编码
        /// </summary>
        public string? plcCode { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? updateDateTime { get; set; }

        /// <summary>
        /// 理论产量
        /// </summary>
        public int? theoreticalYield { get; set; }

        /// <summary>
        /// 班次 id
        /// </summary>
        public int? shiftId { get; set; }

        /// <summary>
        /// 班次名称
        /// </summary>
        public string? shiftName { get; set; }

        /// <summary>
        /// 运行时长
        /// </summary>
        public string? runningTime { get; set; }

        /// <summary>
        /// 停机时长
        /// </summary>
        public string? stopTime { get; set; }

        /// <summary>
        /// 等待时长
        /// </summary>
        public string? waitTime { get; set; }

        public string? outStopTime { get; set; }

        public string? insideStopTime { get; set; }

        /// <summary>
        /// 客户预设其他原因停机时长
        /// </summary>
        public string? presettime { get; set; }

        /// <summary>
        /// 有效作业率
        /// </summary>
        public float? efficiency { get; set; }

        /// <summary>
        /// 综合效率
        /// </summary>
        public float? overallEfficiency { get; set; }

        /// <summary>
        /// 最大运行时长
        /// </summary>
        public string? maxRunningTime { get; set; }

        /// <summary>
        /// 平均运行时长
        /// </summary>
        public string? avgRunningTime { get; set; }

        /// <summary>
        /// 停机次数
        /// </summary>
        public int? stopCount { get; set; }

        public string production { get; set; }

        public float useRatio { get; set; }

        public string downStopTime { get; set; }


        public string upStopTime { get; set; }


    }
}

