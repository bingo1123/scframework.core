using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;
using YS.Yuanji.Log;

namespace YS.PLC.BeckHoff
{
    public class JgdkAdsController : BeckHoffController
    {
        private string  deviceid;
        private string pubtopic;
        public JgdkAdsController(MqttController mqttController) : base(mqttController)
        {
        }

        public override Task Initialization()
        {
            deviceid = DeviceConfig.ParameterDict.ContainsKey("DeviceId") ? DeviceConfig.ParameterDict["DeviceId"] : "";
            pubtopic = DeviceConfig.ParameterDict.ContainsKey("Pubtopic")? DeviceConfig.ParameterDict["Pubtopic"] : $"realtime/{MachineCode}";
            subwrite = string.Format(MqttConst.Subwrite, deviceid);
            pubwrite = string.Format(MqttConst.Pubwrite, deviceid);
            return base.Initialization();
        }

        protected override async Task RealtimeAsync(List<Item> items)
        {
            LogController.Instance.Log(items.Count + " 个数据项正在处理...");
            RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
            realTimeDataDto.MachineId = MachineCode;
            realTimeDataDto.Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DataItemDetailDto>();

            var advalues = await _chanle.ReadAsync(items);
            //var advalues = adsOperate.Read(paras);
            if (advalues.Item1)
            {
                ComputeItems(advalues.Item3);
                foreach (var item in advalues.Item3)
                {
                    var item2 = items.FirstOrDefault(l => l.Address == item.Key.Address);
                    if (item2 != null)
                    {
                        var code = MachineCode + item.Key.Address;
                        var detail = new DataItemDetailDto
                        {
                            Code = code,
                            Value = item.Value,
                            IsGood = true
                        };
                        realTimeDataDto.Details.Add(detail);
                    }
                }
            }
            else
            {
                LogController.Instance.Error(advalues.Item2);
            }

            await PublishRealtimeData(realTimeDataDto);
        }

        public override IEnumerable<WriteModel> TryGetWriteItems(WriteDataDto writeDataDto)
        {
            var _items = new List<WriteModel>();
            if (writeDataDto.Command == "Set_Pulselength")
            {
                var lap = items.Values.FirstOrDefault(l => l.Address == ".laser1_p");
                if (lap != null)
                {
                    _items.Add(new WriteModel(lap, writeDataDto.Value.Target));
                }
              
                var dataSureItem = items.Values.FirstOrDefault(l => l.Address == ".data_sure");
                if (dataSureItem != null)
                {
                    _items.Add(new WriteModel(dataSureItem,true,1000));
                    _items.Add(new WriteModel(dataSureItem, false, 1000));
                }
            }

            return _items;
        }

    }
}
