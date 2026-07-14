// 查询包装机生产数据
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    ///// <summary>
    ///// 生产数据-
    ///// <remark>查询包装机产量、剔除量、小盒商标纸、条盒纸总耗、烟包成品率、废品率</remark>
    ///// </summary>
    //public class ProductionDataClient 
    //{
    //    private readonly ApiClient _client;
    //    public ProductionDataClient(ApiClientConfig config)
    //    {
    //        _client=ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<ProductionDataResponse> GetProductionDataAsync()
    //    {

    //        var response = await _client.GetAsync(endpoint, new Dictionary<string, string>());
    //        return JsonConvert.DeserializeObject<ProductionDataResponse>(response);
    //    }
    //}

    public class ProductionDataResponse
    {
        public int? Code { get; set; }
        public Dictionary<string, ProductionData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class ProductionData
    {
        /// <summary>
        /// 班次开始时间
        /// </summary>
        public DateTime? ShiftDateStart { get; set; }

        /// <summary>
        /// 班次编码
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
     
    xx_exit_pdt 小包机产量
    xx_total_reject 小包机剔除量
    ch_exit_pdt 小透机产量
    ch_reject 小透机剔除量
    ct_pdt 条包机产量
    ct_reject 条包机剔除量
    xx_label_paper_total_expend 小盒商标纸总耗
    ct_label_paper_total_expend 条盒商标纸总耗
    packer_yield_rate 烟包成品率
    packer_waste_rate 烟包废品率
     
     */
}

