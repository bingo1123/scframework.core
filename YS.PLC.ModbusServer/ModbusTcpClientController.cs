using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;

namespace YS.PLC.ModbusServer
{
    public class ModbusTcpClientController : DataCollectContoller
    {
        private string deviceid;
        private string pubtopic;
        public ModbusTcpClientController(MqttController mqttController) : base(mqttController)
        {
        }

        protected override async  Task RealtimeAsync(List<Item> items)
        {
            var its = items;
            var result = await _chanle.ReadAsync(its);
            var realtime = new RealTimeDataWithDeviceDto
            {
                DeviceId = deviceid,
                MachineId = MachineCode,
                Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                Details = new List<DataItemDetailDto>()
            };
            if (result.Item1 == true)
            {
                ComputeItems(result.Item3);
                foreach (var item in result.Item3)
                {
                    realtime.Details.Add(new DataItemDetailDto
                    {
                        Name = item.Key.Name,
                        Code = item.Key.Code,
                        Value = item.Value,
                        IsGood = true,
                    });
                }
            }
            else
            {
                LogController.Instance.Log($"ModbusTcpClient读取失败，原因:{result.Item2}");
            }

            if(realtime.Details.Count > 0)
            {
                await PublishDto(pubtopic, realtime);
            }
        }

        public override async Task Initialization()
        {
            try
            {
                deviceid = DeviceConfig.ParameterDict.ContainsKey("DeviceId") ? DeviceConfig.ParameterDict["DeviceId"] : "";
                pubtopic = DeviceConfig.ParameterDict.ContainsKey("Pubtopic") ? DeviceConfig.ParameterDict["Pubtopic"] : $"realtime/{MachineCode}";
                subwrite = string.Format(MqttConst.Subwrite, deviceid);
                pubwrite = string.Format(MqttConst.Pubwrite, deviceid);
                foreach (var ivs in interItems.Values)
                {
                    if (ivs?.Params == null || ivs.Params.Count == 0)
                    {
                        continue;
                    }

                    foreach (var config in ivs.Params)
                    {
                        if (!string.IsNullOrEmpty(config.Extended))
                        {
                            var file = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, config.Extended);
                            if (File.Exists(file))
                            {
                                var item = await File.ReadAllLinesAsync(file);
                                config.SymbAdrs = item.Select(x => new Item
                                {
                                    Name = x.Split("\t")[3],
                                    Code = x.Split("\t")[4],
                                    Address = x.Split("\t")[0],
                                    ValueType = x.Split("\t")[1],
                                    AddressType = x.Split("\t").Length > 5 ? x.Split("\t")[5] : string.Empty,
                                    Linear = x.Split("\t").Length > 6 ? x.Split("\t")[6]:string.Empty,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogController.Instance.Error("InitData:" + e.Message);
            }
        }

        protected override Task<(bool, string)> WriteDataAynsc(Item writeDataDto, object value, CancellationToken token = default)
        {
            return _chanle.WriteAsync(writeDataDto, value);
        }

        public override IEnumerable<WriteModel> TryGetWriteItems(WriteDataDto writeDataDto)
        {
             var _items = new List<WriteModel>();
            //if ( writeDataDto.Command == "Set_Pulselength")
            //{
            //    _items =  items.Values.Where(l=>l.Address == "40003" || l.Address == "40033").Select(l=>new WriteModel(l,writeDataDto.Value.Target));
            //}

            if (writeDataDto.Command == "Set_Pulselength")
            {
                var lap = items.Values.FirstOrDefault(l => l.Address == "40003");
                if (lap != null)
                {
                    _items.Add(new WriteModel(lap, writeDataDto.Value.Target,1000));
                }

                var dataSureItem = items.Values.FirstOrDefault(l => l.Address == "40040");
                if (dataSureItem != null)
                {
                    _items.Add(new WriteModel(dataSureItem, 10, 1000));
                }
            }

			if(writeDataDto.Command == "Set_Dkjwarn")
			{
				var lap = items.Values.FirstOrDefault(l => l.Address == "40041");
				if(lap != null)
				{
					_items.Add(new WriteModel(lap, writeDataDto.Value.Target, 1000));
				}
			}

			return _items;
        }
    }
}
