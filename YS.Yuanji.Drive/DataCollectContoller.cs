using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using YS.Yuanji.Commom;
using YS.Yuanji.Log;

namespace YS.Yuanji.Drive
{
    public abstract class DataCollectContoller : IDevice
    {
        #region Property
        public DataCollectConfig config { get; private set; }
        public IChanlel _chanle;
        public string _packMachineName;
        public List<ParameterItems> DataConfig { get; private set; }
        public DeviceConfig DeviceConfig { get; private set; }

        private Task _machineTask;
        private CancellationToken _mainToken;

        private CancellationTokenSource _mainTokenSource;
        protected ConcurrentDictionary<int, AddressItemsList> interItems { get; private set; }

        private DateTime StartOnTime = DateTime.Now;
        private Task _heartTask;
        private Task _writeTask;
        private string uploadtopic;
        protected string subwrite;
        protected string pubwrite;
        protected MqttController _mqttController;
        private Timer _push;
        private DataTable _compute; 
       
        public string MachineCode { get; private set; }
        public string MachinePart { get; private set; }
        public bool IsContaiName { get; private set; } = false;
        public bool Controlwrite { get; private set; } = false;

        public ProtocolTypeEnum typeEnum;
        public ConcurrentDictionary<string, Item> items { get; private set; }
        public string Prex { get; private set; }
        public DataManage DataManage { get; private set; }
        /// <summary>
        /// 状态，0关机(网络故障)，1停机,2运行，3网络故障
        /// </summary>
        public MachineStatusEnum MachineStatus { get; set; } = 0;
        #endregion

        #region Sington

        public DataCollectContoller(MqttController mqttController)
        {
            config = new DataCollectConfig();
            _mainTokenSource = new CancellationTokenSource();
            _mqttController = mqttController;
            interItems = new ConcurrentDictionary<int, AddressItemsList>();
            items = new ConcurrentDictionary<string, Item>();
            DataManage = new DataManage();
            _compute = new DataTable();
        }
        #endregion

        public Action<RealTimeDataDto> LogDisPlayAction { get; set; } = null;

        public ConcurrentQueue<WriteDataDto> writebuffer = new ConcurrentQueue<WriteDataDto>();

        #region Method
        private async Task StationActionsCaller()
        {
            try
            {
                if (interItems == null || interItems.Count == 0)
                {
                    return;
                }
                await _chanle.ConnectAsync();
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"启动报错:exception:{ex}");
            }
            Stopwatch stopwatch = new Stopwatch();
            while (!_mainTokenSource.IsCancellationRequested)
            {
                try
                {
                    stopwatch.Start();
                    if (!_chanle.IsConnected())
                    {
                        MachineStatus = MachineStatusEnum.Shutdown;
                        await Task.Delay(2000);
                        continue;
                    }
                    MachineStatus = MachineStatusEnum.Stop;
                    foreach (var item in interItems)
                    {
                        try
                        {
                            if ((DateTime.Now - item.Value.LastIntervalTime).TotalMilliseconds >= item.Key)
                            {
                                item.Value.LastIntervalTime = DateTime.Now;
                                await CigMachineMqttCaller(item.Key, item.Value.Params);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogController.Instance.Error("StationActionsCaller error:" + ex.Message + ",StackTrace:" + ex.StackTrace);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error("StationActionsCaller异常：" + ex.Message);
                }
                finally
                {
                    if (stopwatch.IsRunning)
                    {
                        var elapse = stopwatch.ElapsedMilliseconds;
                        if (elapse < 100)
                        {
                            await Task.Delay(100 - (int)elapse);
                        }
                        stopwatch.Restart();
                    }
                }
            }
        }

        protected virtual async Task CigMachineMqttCaller(int interval, List<AddressItems> addressItems)
        {
            var _item = addressItems;
            if (_item == null)
            {
                return;
            }

            foreach (var item in _item)
            {
                switch (item.Key)
                {
                    case IntervelEnum.product:
                        if (item.Enabled && item.Interval == interval)
                        {
                            await ProductAsync(item.SymbAdrs);
                        }
                        break;
                    case IntervelEnum.realtime:
                        if (item.Enabled && item.Interval == interval)
                        {
                            await RealtimeAsync(item.SymbAdrs);
                        }
                        break;
                    case IntervelEnum.others:
                        if (item.Enabled && item.Interval == interval)
                        {
                            await OthersAsync(item.SymbAdrs);
                        }
                        break;
                    default:
                        break;
                }
            }

            return;
        }

        protected virtual async Task OthersAsync(List<Item> items)
        {

        }

        protected virtual async Task RealtimeAsync(List<Item> items)
        {

        }

        protected virtual async Task ProductAsync(List<Item> items)
        {

        }

        private void MqttReviceData(string topic, string msg)
        {
            try
            {
                //if (!topic.Contains(config.MachineName))
                //{
                //    return;
                //}
                if (topic == subwrite)
                {
                    var data = JsonConvert.DeserializeObject<WriteDataDto>(msg);
                    if (data != null)
                        writebuffer.Enqueue(data);
                }

            }
            catch (Exception ex)
            {
                LogController.Instance.Error("MqttReviceData error:" + ex.Message + ",StackTrace:" + ex.StackTrace);
            }

        }

        #endregion

        public async Task<bool> StartAsync()
        {
            try
            {
                if (_machineTask == null)
                {
                    _machineTask = Task.Factory.StartNew(StationActionsCaller, TaskCreationOptions.LongRunning);
                }
                if (_writeTask == null)
                {
                    _writeTask = Task.Factory.StartNew(WriteData, TaskCreationOptions.LongRunning);
                }
                if (_heartTask == null)
                {
                    _heartTask = Task.Factory.StartNew(HeartActionsCaller, TaskCreationOptions.LongRunning);
                }
                _push = new Timer(Synchron, null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(DeviceConfig.Intervaltime));
                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"WorkInfoHelp:exception:{ex}");

                return false;
            }
        }

        private async Task WriteData()
        {
            int i = 0;
            while (!_mainTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (!_chanle.IsConnected())
                    {
                        await Task.Delay(2000);
                        //i++;
                        //if (i > 30)
                        //{
                        //    writebuffer.Clear();
                        //}
                        //continue;
                    }

                    if (writebuffer.TryDequeue(out var pvalue))
                    {
                        var reponse = new ReponseWriteDataDto
                        {
                            DeviceId = MachineCode,
                            Tstamp = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                            Id = pvalue.Id,
                        };
                        if (!Controlwrite)
                        {
                            reponse.Status = "Failed";
                            reponse.ErrorCodde = "设备端不允许下发指令";
                        }
                        else
                        {
                            if (pvalue.DeviceId == MachineCode)
                            {
                                var _writes = TryGetWriteItems(pvalue);
                                if (_writes != null && _writes.Count() > 0)
                                {
                                    if (!_chanle.IsConnected())
                                    {
                                        reponse.Status = "Failed";
                                        reponse.ErrorCodde = "设备未连接，不能下发指令";
                                    }
                                    else
                                    {
                                        foreach (var _it in  _writes)
                                        {
                                            var write = await WriteDataAynsc(_it.Key, _it.Value);
                                            if (write.Item1)
                                            {
                                                reponse.Status = "Success";
                                                reponse.ActualValue = reponse.ActualValue ==  null ? _it.Value : reponse.ActualValue + "," + _it.Value;
                                                //reponse.ActualValue = reponse.ActualValue == null ?  : reponse.ActualValue + "," + pvalue.Value.Target;
                                            }
                                            else
                                            {
                                                reponse.Status = "Failed";
                                                reponse.ErrorCodde = _it.Key.Address + write.Item2;
                                                break;
                                            }
                                            await Task.Delay(_it.Delay);
                                        }
                                    }
                                }
                                else
                                {
                                    reponse.Status = "Failed";
                                    reponse.ErrorCodde = "Command not found";
                                }
                            }
                            else
                            {
                                reponse.Status = "Failed";
                                reponse.ErrorCodde = "DeviceId not found";
                            }
                        }
                        LogController.Instance.Log($"WriteData reponse:{JsonConvert.SerializeObject(reponse)}");
                        await _mqttController.PublishMessageAsync(pubwrite, JsonConvert.SerializeObject(reponse));
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"WriteData:exception:{ex}");
                }
                finally
                {
                    await Task.Delay(100);
                }
            }
        }

        protected virtual async Task<(bool, string)> WriteDataAynsc(Item writeDataDto, object value, CancellationToken token = default)
        {
            return (false, string.Empty);
        }

        public async Task<bool> StoptAsync()
        {
            try
            {
                _push?.Dispose();
                _mainTokenSource?.Cancel();
                _mainTokenSource?.Dispose();
                _chanle?.DisconnectAsync();
                _chanle = null;
                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"StoptAsync:exception:{ex}");
                return false;
            }

        }

        public async Task<bool> InitAsync(IChanlel chanlel, DeviceConfig device, List<ParameterItems> dataConfig)
        {
            try
            {
                DeviceConfig = device;
                DataConfig = dataConfig;
                MachineCode = device.MachineCode;
                uploadtopic = $"{MqttConst.MqttTopicRealTime}/{MachineCode}";
                subwrite = string.Format(MqttConst.Subwrite, MachineCode);
                pubwrite = string.Format(MqttConst.Pubwrite, MachineCode);
                config.MachineName = MachineCode;
                MachinePart = device.MachinePart;
                IsContaiName = device.IsContaiName;
                Controlwrite = device.ControlWrite;
                Prex = string.IsNullOrEmpty(MachinePart) ? $"{MachineCode}" : $"{MachineCode}.{MachinePart}";
                typeEnum = Enum.TryParse<ProtocolTypeEnum>(DeviceConfig.MachineType, out var _type) ? _type : ProtocolTypeEnum.Other;
                config.HuaniProtocolTypeEnum = typeEnum;
                DataConfig?.ForEach(l =>
                {
                    try
                    {
                        var address = new AddressItemsList() { Params = new List<AddressItems>() };

                        if (interItems.ContainsKey(l.Interval))
                        {
                            interItems[l.Interval].Params.Add(new AddressItems
                            {
                                Enabled = true,
                                Key = Enum.TryParse<IntervelEnum>(l.Key, true, out var _v) ? _v : IntervelEnum.others,
                                SymbAdrs = l.SymbAdrs,
                                Interval = l.Interval,
                                Extended = l.Extended,
                            });
                        }
                        else
                        {
                            address.Params.Add(new AddressItems
                            {
                                Enabled = true,
                                Key = Enum.TryParse<IntervelEnum>(l.Key, true, out var _v) ? _v : IntervelEnum.others,
                                SymbAdrs = l.SymbAdrs,
                                Interval = l.Interval,
                                Extended = l.Extended,
                            });
                            interItems.TryAdd(l.Interval, address);
                        }
                        l.SymbAdrs.ForEach(t =>
                        {
                            items.TryAdd(t.Address, t);
                        });
                    }
                    catch (Exception e)
                    {
                        LogController.Instance.Error("load address convert error:" + e);
                    }
                });
                _chanle = chanlel;
                _chanle.CancellationToken = _mainTokenSource.Token;
                await _chanle.LoadAsync(new ChanleConfig
                {
                    IP = DeviceConfig.Ip,
                    Port = DeviceConfig.Port,
                    ChannelName = DeviceConfig.MachineName,
                    ChannelType = DeviceConfig.MachineType,
					ParameterDict = DeviceConfig.ParameterDict
                });

                _mqttController.RecvieData(MqttReviceData);
                LogController.Instance.Log($"{MachineCode} {DeviceConfig.MachineType} LoadConfig success");
                await Initialization();
                await _mqttController.Subscribe(subwrite);
                foreach (var item in interItems)
                {
                    if (item.Value.Params.Count > 0)
                    {
                        foreach (var item1 in item.Value.Params)
                        {
                            item1.SymbAdrs?.ForEach(t =>
                            {
                                items.TryAdd(t.Address, t);
                            });
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error("LoadConfig error:" + ex.Message + ",StackTrace:" + ex.StackTrace);
                return false;
            }

        }

        public virtual async Task Initialization()
        {
            try
            {
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
                                var item = File.ReadAllLines(file);
                                config.SymbAdrs = item.Select(x => new Item
                                {
                                    Name = x.Split("\t")[2],
                                    Address = x.Split("\t")[0],
                                    ValueType = x.Split("\t")[1],
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

            LogController.Instance.Log($"{MachineCode} Initialization");
        }

        public async Task<bool> PublishRealtimeData(RealTimeDataDto realTimeDataDto)
        {
            if (realTimeDataDto is RealTimeDataDto real)
            {
                DataManage.UpdateData(real.MachineId, real.Details);
            }

            var res = await _mqttController.ConnectAsync();
            if (realTimeDataDto.Details.Count > 0 && res)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                if (!IsContaiName)
                {
                    realTimeDataDto.Details = realTimeDataDto.Details.Select(l => new DataItemDetailDto { Code = l.Code, IsGood = l.IsGood, Value = l.Value }).ToList();
                }
                LogDisPlayAction?.Invoke(realTimeDataDto);
                res = await _mqttController.PublishMessageAsync(uploadtopic, JsonConvert.SerializeObject(realTimeDataDto, settings), false);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> PublishDto(string topic, object dataDto)
        {
            if (dataDto is RealTimeDataDto real)
            {
                DataManage.UpdateData(real.MachineId, real.Details);

                if (!IsContaiName)
                {
                    real.Details = real.Details.Select(l => new DataItemDetailDto { Code = l.Code, IsGood = l.IsGood, Value = l.Value }).ToList();
                }
               dataDto = real;
            }

            var res = await _mqttController.ConnectAsync();
            if (dataDto != null && res)
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                res = await _mqttController.PublishMessageAsync(topic, JsonConvert.SerializeObject(dataDto, settings), false);
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void ComputeItems(Dictionary<Item, object> Items)
        {
            var keys = Items.Keys.ToList();
            foreach (var key in keys)
            {
                try
                {
                    if (!string.IsNullOrEmpty(key.Linear))
                    {
                        Items[key] = _compute.Compute(Items[key] + key.Linear, null);
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error("ComputeItems add column error:" + ex.Message);
                }
            }
        }

        public virtual IEnumerable<WriteModel> TryGetWriteItems(WriteDataDto writeDataDto)
        {
            var writeitems = items.Values.Where(l =>
            {
                return l.Authority != null && (l.Authority?.ToLower().Contains("write") is true && l.Address == writeDataDto.Command);
            }).Select(s=> new WriteModel(s,writeDataDto.Value.Target)).ToList();
            return writeitems;
        }

        /// <summary>
        /// 心跳检测.
        /// </summary>
        private async Task HeartActionsCaller()
        {
            while (!_mainTokenSource.IsCancellationRequested)
            {
                try
                {
                    var res = await _mqttController.ConnectAsync();
                    if (res)
                    {
                        RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
                        realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                        realTimeDataDto.MachineId = MachineCode;
                        realTimeDataDto.Details = new List<DataItemDetailDto>();
                        var before = string.IsNullOrEmpty(MachinePart) ? $"{MachineCode}." : $"{MachineCode}.{MachinePart}.";
                        var detail = new DataItemDetailDto
                        {
                            Code = before + ControllerConst.MachineStatus,
                            Value = MachineStatus,
                            IsGood = true
                        };
                        realTimeDataDto.Details.Add(detail);
                        var detail1 = new DataItemDetailDto
                        {
                            Code = before + ControllerConst.StartOnTime,
                            Value = StartOnTime,
                            IsGood = true
                        };
                        realTimeDataDto.Details.Add(detail1);

                        res = await _mqttController.PublishMessageAsync($"realtime/status",
                           JsonConvert.SerializeObject(realTimeDataDto));
                    }
                }
                catch
                {
                }
                finally
                {
                    await Task.Delay(3000);
                }
            }
        }

        private async void Synchron(object? status)
        {
            try
            {
                if (DataManage.GetAllData().TryGetValue(MachineCode, out var _da))
                {
                    var realTimeDataDto = new RealTimeDataDto();
                    realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    realTimeDataDto.MachineId = MachineCode;
                    realTimeDataDto.Details = _da;
                    await PublishDto(MqttConst.MqttTopicRealTime + "/" + MachineCode, realTimeDataDto);
                }

                return;
            }
            catch
            {
                return;
            }
        }


    }
}
