using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    ///// <summary>
    ///// 设备单机状态接口
    ///// </summary>
    //public class CurrentMachineStatus 
    //{


    //    public async Task<MachineStatusResponse?> GetMachineStatusAsync()
    //    {

    //        var response =await apiClient.GetAsync(endpoint, new Dictionary<string, string>());
    //        return JsonConvert.DeserializeObject<MachineStatusResponse>(response);
    //    }

    //    //public MachineStatusResponse GetMachineStatus()
    //    //{
    //    //    string endpoint = "/runningStatus/getCurrentMachineStatus";
    //    //    var response =  apiClient.Get(endpoint, new Dictionary<string, string>());
    //    //    return JsonConvert.DeserializeObject<MachineStatusResponse>(response);
    //    //}
    //}



    public class MachineStatusResponse
    {
        public int? Code { get; set; }
        public List<MachineStatusData>? Data { get; set; }
        public string? Msg { get; set; }
    }

    public class MachineStatusData
    {
        public string? EquipmentName { get; set; }
        public string? PlcCode { get; set; }
        public int? status { get; set; }
    }

}
