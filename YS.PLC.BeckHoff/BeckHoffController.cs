using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using YS.Yuanji.Commom.Common;
using YS.Yuanji.Log;
using Newtonsoft.Json;

namespace YS.PLC.BeckHoff
{
    public class BeckHoffController : DataCollectContoller
    {

        public BeckHoffController(MqttController mqttController) : base(mqttController)
        {
        }

        public override async Task Initialization()
        {
            InitData();
            //await adsOperate.Connect(DeviceConfig.ParameterDict["amsNetIdSource"], adsnetid,
            //    DeviceConfig.ParameterDict["amsNetIdTarget"], adsport);
            return ;
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
                foreach (var item in advalues.Item3)
                {
                    var item2 = items.FirstOrDefault(l => l.Address == item.Key.Address);
                    if (item2 != null)
                    {
                       var detail = new DataItemDetailDto
                       {
                           Name = item2.Name,
                           Code = $"{Prex}.{item2.Address.ToString()}",
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

        private void InitData()
        {
            try
            {
                foreach (var ivs in interItems.Values)
                {
                    if(ivs?.Params == null || ivs.Params.Count == 0)
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
                                var item = File.ReadAllLines(file);
                                config.SymbAdrs = item.Select(x => new Item
                                {
                                    Name = x.Split("\t")[2],
                                    Address = x.Split("\t")[0],
                                    ValueType = x.Split("\t")[1],
                                    Linear = x.Split("\t").Length > 3 ? x.Split("\t")[3] : string.Empty,
                                }).ToList();
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LogController.Instance.Error("InitData:" + e.Message);
            }
        }

        protected override async Task<(bool,string)> WriteDataAynsc(Item writeDataDto,object value, CancellationToken token = default)
        {
            if (writeDataDto != null)
            {
                var items = writeDataDto;
                var advalues =await _chanle.WriteAsync(items, value);
                if (advalues.Item1)
                {
                    return (true,string.Empty);
                }
                else
                {
                    LogController.Instance.Error(advalues.Item2);
                    return (false, advalues.Item2);
                }
            }

            return (false, string.Empty);
        }

    }
}
