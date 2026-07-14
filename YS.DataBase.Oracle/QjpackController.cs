using System;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using YS.Yuanji.Drive;
using System.Diagnostics.Metrics;
using YS.Yuanji.Log;
using YS.Yuanji.Commom;
using Newtonsoft.Json;
using YS.Yuanji.Commom.helper;

namespace YS.DataBase.Oracle
{
    public class QjpackController : DataCollectContoller
    {
        private string connString = @"User Id=micro3;Password=micro3;Data Source=localhost:1521/GDORACLEXDB;";
        private Dictionary<string, string> mahcinpart = new Dictionary<string, string> { { "5055", "CH" }, { "5056", "CT" } };
        public QjpackController(MqttController mqttController) : base(mqttController)
        {
        }

        public override async Task Initialization()
        {
            connString = DeviceConfig.Ip;
        }

        protected async override Task OthersAsync(List<Item> items)
        {

        }

        protected async override Task ProductAsync(List<Item> items)
        {
            foreach (var part in mahcinpart)
            {
                try
                {

                    CigaretteProductDto productDto = new CigaretteProductDto();
                    CigratteRejectDto rejectDto = new CigratteRejectDto();
                    DateTime tt = DateTime.Now;
                    var data = DataManage.GetAllData();
                    //productDto.PRODUCEDATE_IN = tt.ToString("yyyy-MM-dd");
                    //productDto.PB_SHIFT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().classtimeid;
                    //productDto.PB_PRODUCT_ID = WorkInfoHepler.Instance.GetCurrentWorkInfo().productid;
                    //productDto.SWJDM = string.Empty;
                    //productDto.XWJDM = config.MachineName;
                    //productDto.NO = 1;
                    //productDto.TOTALPRODUCTION = data.GetValueOrDefault($"{MachineCode}.{part.Value}.}");
                    //productDto.TOTALWASTE = (int)ZJ118proc.TotalWasteCount;
                    //productDto.TOTALWASTEPCT = ZJ118proc.TotalWastePercent;
                    //productDto.EFFMACHINE = ZJ118.EffMachine;
                    //productDto.EFFPRODUCTION = ZJ118.EffProduction;
                    //productDto.RUNTIME = (int)ZJ118.RunTime;
                    //productDto.TOTALSTOPCNT = (int)ZJ118.Stops;
                    //productDto.TOTALSTOPTIME = (int)ZJ118.StopTime;
                    //productDto.batch = WorkInfoHepler.Instance.GetCurrentWorkInfo().workorder;
                    //productDto.PRODUCTIONSTARTTIME = VisuHelper.P18Uint32ToDateTime(ZJ118.ShiftStart);
                    //productDto.PAPER = ZJ118.Tobacco_Cons;
                    //productDto.WPAPER = ZJ118.Filter_Cons;
                    //productDto.TobaccoSilk = ZJ118.MLP_Air_Count;
                    //productDto.FILTERTIP = (int)ZJ118.Paper_Cons;
                    //productDto.PAPERPAN = (int)ZJ118.MLP_FilterCount;
                    //productDto.WPAPERPAN = (int)ZJ118.MLP_MAXBOB;
                    //productDto.PAOTIAOS = ZJ118.MLP_CurrentDrain;
                    //productDto.GATHERTYPE = "A";

                    rejectDto.PRODUCEDATE_IN = tt.ToString("yyyy-MM-dd");
                    rejectDto.PB_SHIFT_ID = -1;
                    rejectDto.PB_PRODUCT_ID = "";
                    rejectDto.PM_MP_MACHINEPART_ID = part.Value;
                    rejectDto.SWJDM = string.Empty;
                    rejectDto.XWJDM = config.MachineName;
                    rejectDto.REJECTS = new List<RejectInfo>();

                    foreach (var reject in data.Keys.Where(l => l.Contains($"{part.Value}.reject")))
                    {
                        rejectDto.REJECTS.Add(new RejectInfo
                        {
                            PM_MP_REJECTCODE_ID = reject,
                            REJECTVALUE = int.TryParse(data[reject].ToString(), out var _v) ? _v : 0
                        });
                    }


                if (rejectDto.REJECTS.Count > 0)
                {
                    await _mqttController.ConnectAsync("QjpackController");
                    await _mqttController.PublishMessageAsync(config.MachineName + "/1057", JsonConvert.SerializeObject(rejectDto), isSaveMessage: false, "QjpackController", AtMostOnce: true);
                }

                }
                catch (Exception e)
                {
                    LogController.Instance.Error($"在 ProductAsync occur exception:{e}");
                }
            }
        }

        protected async override Task RealtimeAsync(List<Item> items)
        {
            foreach (var part in mahcinpart)
            {
                RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
                realTimeDataDto.MachineId = MachineCode;
                realTimeDataDto.Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                realTimeDataDto.Details = new List<DataItemDetailDto>();
                using (OracleConnection conn = new OracleConnection(connString))
                {
                    try
                    {
                        conn.Open();
                        // 班次数据
                        string sql = $"SELECT * FROM ( select  * from RTS_SHIFT_MACHINE_STAT where MACHINE_NAME = '{part.Key}' ORDER BY SHIFT_DATE_START DESC ) WHERE ROWNUM =1";
                        OracleCommand cmd = new OracleCommand(sql, conn);
                        OracleDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                realTimeDataDto.Details.Add(new DataItemDetailDto
                                {
                                    Code = $"{MachineCode}.{part.Value}.{reader.GetName(i)}",
                                    Value = reader.GetValue(i),
                                    IsGood = true
                                });
                            }
                        }
                        reader.Close();

                        /// 剔除数据
                        sql = $"SELECT ID,VALUE,COUNTER_NAME from SHIFT_COUNTERS where MACH_CODE = '{part.Key}' and COUNTER_TYPE = 'R'";
                        cmd = new OracleCommand(sql, conn);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = $"{MachineCode}.{part.Value}.reject.{reader.GetValue(0)}",
                                Value = reader.GetValue(1),
                                IsGood = true
                            });
                        }
                        reader.Close();

                        /// 加热器数据
                        sql = $"SELECT OBJECTNAME,SETPOINT,TEMPERATURE,MIN,MAX FROM THERMOSTAT where MACH_CODE = '{part.Key}'";
                        cmd = new OracleCommand(sql, conn);
                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = $"{MachineCode}.{part.Value}.{reader.GetValue(0)}_set",
                                Value = reader.GetValue(1),
                                IsGood = true
                            });
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = $"{MachineCode}.{part.Value}.{reader.GetValue(0)}_tem",
                                Value = reader.GetValue(2),
                                IsGood = true
                            });
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = $"{MachineCode}.{part.Value}.{reader.GetValue(0)}_min",
                                Value = reader.GetValue(3),
                                IsGood = true
                            });
                            realTimeDataDto.Details.Add(new DataItemDetailDto
                            {
                                Code = $"{MachineCode}.{part.Value}.{reader.GetValue(0)}_max",
                                Value = reader.GetValue(4),
                                IsGood = true
                            });

                            DataManage.UpdateData(MachineCode, realTimeDataDto.Details);

                        }
                        reader.Close();

                    }
                    catch (Exception ex)
                    {
                        LogController.Instance.Error($"错误: {ex}");
                    }
                }

                if (realTimeDataDto.Details.Count > 0)
                {
                    LogDisPlayAction?.Invoke(realTimeDataDto);
                    DataManage.UpdateData(MachineCode, realTimeDataDto.Details);
                    await _mqttController.ConnectAsync("QjpackController");
                    await _mqttController.PublishMessageAsync("realtime/" + config.MachineName, JsonConvert.SerializeObject(realTimeDataDto), isSaveMessage: false, "QjpackController", AtMostOnce: true);
                }
            }
           
        }
    }
}
