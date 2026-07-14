using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    ///// <summary>
    ///// 全流程消耗接口
    ///// <remark>查询包装机各个单元出入口产量、剔除量、损耗率</remark>
    ///// </summary>
    //public class ProcessConsumptionClient 
    //{
    //    private readonly ApiClient _client;

    //    public ProcessConsumptionClient(ApiClientConfig config)
    //    {
    //        _client=ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<ProcessConsumptionResponse> GetProcessConsumptionAsync()
    //    {
           
    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        Console.WriteLine(response);
    //        return JsonConvert.DeserializeObject<ProcessConsumptionResponse>(response);
    //    }
    //}

    public class ProcessConsumptionResponse
    {
        public int? Code { get; set; }
        public Dictionary<string, ProcessConsumptionData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class ProcessConsumptionData
    {
        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime? ShiftDateStart { get; set; }

        /// <summary>
        /// 班次编码（1 早班、2 午班、3 晚班）
        /// </summary>
        public int? ShiftId { get; set; }

        /// <summary>
        /// 数值
        /// </summary>
        public float? Count { get; set; }

        /// <summary>
        /// 变量名（对照关系见下方）
        /// </summary>
        public string? ProductName { get; set; }

        /// <summary>
        /// 设备编码
        /// </summary>
        public string? EquipmentCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string? EquipmentName { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SendTime { get; set; }

        /// <summary>
        /// 产线名称
        /// </summary>
        public string? Place { get; set; }
    }

    /*
     * 
    xx_entry_pdt 小包机入口量
    xx_exit_pdt 小包机产量
    xx_total_reject 小包机剔除量
    xx_waste_rate 小包机损耗率
    ch_entry_pdt 小透机入口量
    ch_exit_pdt 小透机产量
    ch_reject 小透机剔除量
    ch_waste_rate 小透机损耗率
    ct_pdt 条包机产量
    ct_reject 条包机剔除量
    xx_label_paper_total_expend 小盒商标纸总耗
    ct_label_paper_total_expend 条盒商标纸总耗
    packer_yield_rate 烟包成品率
    packer_waste_rate 烟包废品率
    */
}
