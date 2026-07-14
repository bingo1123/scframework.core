using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    ///// <summary>
    ///// 参数修改记录接口
    ///// <remark>
    ///// 记录每次参数修改记录，包含旧值、新值和修改时间</remark>
    ///// </summary>
    //public class ParameterModificationClient 
    //{
    //    private readonly ApiClient _client;

    //    public ParameterModificationClient(ApiClientConfig config)
    //    {
    //        _client=ApiClientFactory.CreateClient(config);
    //    }

    //    public async Task<ParameterModificationResponse> GetParameterModificationsAsync(int pageSize, int pageNum = 0, string orderBy = "id", string direction = "DESC")
    //    {

    //        var parameters = new Dictionary<string, string>
    //    {
    //        { "pageNum", pageNum.ToString() },
    //        { "pageSize", pageSize.ToString() },
    //        { "orderBy", orderBy },
    //        { "direction", direction }
    //    };
    //        var response = await _client.PostAsync(endpoint, parameters);
    //        return JsonConvert.DeserializeObject<ParameterModificationResponse>(response);
    //    }
    //}

    public class ContentItem
    {
        public int? Id { get; set; }
        public string? ParameterId { get; set; }
        public string? EquipmentCode { get; set; }
        public string? EquipmentName { get; set; }
        public string? Organization { get; set; }
        public string? Place { get; set; }
        public int? EquipmentTypeId { get; set; }
        public string? PlcCode { get; set; }
        public DateTime? DateTime { get; set; }
        public string? Paracn { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? ValDeafult { get; set; }
        public DateTime? SendTime { get; set; }
        public string? GroupName { get; set; }
        public string? FunctionName { get; set; }
        public string? Unit { get; set; }
        public bool? Status { get; set; }
        public string? UpdateUser { get; set; }
    }

    public class Sort
    {
        public bool? Empty { get; set; }
        public bool? Sorted { get; set; }
        public bool? Unsorted { get; set; }
    }

    public class Pageable
    {
        public Sort? Sort { get; set; }
        public int? Offset { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public bool? Paged { get; set; }
        public bool? Unpaged { get; set; }
    }

    public class Data
    {
        public List<ContentItem>? Content { get; set; }
        public Pageable? Pageable { get; set; }
        public bool? Last { get; set; }
        public int? TotalPages { get; set; }
        public int? TotalElements { get; set; }
        public int? Number { get; set; }
        public int? Size { get; set; }
        public Sort? Sort { get; set; }
        public int? NumberOfElements { get; set; }
        public bool? First { get; set; }
        public bool? Empty { get; set; }
    }

    public class ParameterModificationResponse
    {
        public int? Code { get; set; }
        public Data? Data { get; set; }
        public string? Msg { get; set; }
    }

}
