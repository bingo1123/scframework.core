using System;
using MySql.Data.MySqlClient;
using System.Data;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;
using YS.Yuanji.Commom;
using Newtonsoft.Json;

namespace YS.DataBase.MySQL
{
    /// <summary>
    /// 缺陷记录 DTO - 对应 MySQL 表结构
    /// </summary>
    public class DefectRecordDto
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime date { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string brand { get; set; } = string.Empty;

        /// <summary>
        /// 缺陷类别
        /// </summary>
        public string DefectClass { get; set; } = string.Empty;

        /// <summary>
        /// 缺陷数量
        /// </summary>
        public int Count { get; set; }
    }

    /// <summary>
    /// MySQL 缺陷数据采集控制器
    /// 用于从 MySQL 数据库的 currentdefect 表采集缺陷数据
    /// 使用 RealTimeDataDto&lt;T&gt; 泛型结构上报数据
    /// </summary>
    public class MySQLDefectController : DataCollectContoller
    {
        private string _connectionString = string.Empty;
        private string _tableName = "currentdefect";
		private string pubtopic;

		public MySQLDefectController(MqttController mqttController) : base(mqttController)
        {
        }

        public override async Task Initialization()
        {
            // 从配置中解析连接字符串
            // Ip 字段存储连接字符串: Server=xxx;Port=3306;Database=xxx;Uid=xxx;Pwd=xxx;
            _connectionString = DeviceConfig.Ip;

            // 从 ParameterDict 获取可选配置
            if (DeviceConfig.ParameterDict.ContainsKey("TableName"))
            {
                _tableName = DeviceConfig.ParameterDict["TableName"];
            }

            if(DeviceConfig.ParameterDict.ContainsKey("Pubtopic"))
			{
				pubtopic = DeviceConfig.ParameterDict["Pubtopic"];
			}
			else
			{
				pubtopic = $"realtime/{MachineCode}";
			}

            LogController.Instance.Log($"MySQLDefectController [{MachineCode}] 初始化完成，表名: {_tableName}");
        }

        protected override async Task OthersAsync(List<Item> items)
        {
            // 其他数据类型的采集逻辑（如需要）
        }

        protected override async Task ProductAsync(List<Item> items)
        {
            // 生产数据的采集逻辑（如需要）
        }

        /// <summary>
        /// 实时采集缺陷数据
        /// 从 currentdefect 表读取最新的缺陷统计数据
        /// 使用 RealTimeDataDto&lt;DefectRecordDto&gt; 泛型结构上报
        /// </summary>
        protected override async Task RealtimeAsync(List<Item> items)
        {
            // 使用泛型结构创建数据对象
            RealTimeDataDto<DefectRecordDto> realTimeDataDto = new RealTimeDataDto<DefectRecordDto>();
            realTimeDataDto.MachineId = MachineCode;
            realTimeDataDto.Ts = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            realTimeDataDto.Details = new List<DefectRecordDto>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    LogController.Instance.Log($"[{MachineCode}] MySQL 连接成功");

                    // 查询当前最新的缺陷数据
                    // 表结构: date, brand, DefectClass, Count
                    string sql = $@"
                        SELECT
                            date,
                            brand,
                            DefectClass,
                            Count
                        FROM {_tableName}
                        WHERE date >= @startTime
                        ORDER BY date DESC, DefectClass";

                    // 默认查询最近1小时的数据，可根据配置调整
                    DateTime startTime = DateTime.Now.AddHours(-1);
                    if (DeviceConfig.ParameterDict.ContainsKey("QueryInterval"))
                    {
                        var interval = DeviceConfig.ParameterDict["QueryInterval"];
                        if (int.TryParse(interval, out var ms))
                        {
                            startTime = DateTime.Now.AddMilliseconds(-ms);
                        }
                    }

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@startTime", startTime);

                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var record = new DefectRecordDto
                                {
                                    date = reader.GetDateTime(0),
                                    brand = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    DefectClass = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Count = reader.IsDBNull(3) ? 0 : reader.GetInt32(3)
                                };

                                realTimeDataDto.Details.Add(record);
                            }
                        }
                    }

                    LogController.Instance.Log($"[{MachineCode}] 读取到 {realTimeDataDto.Details.Count} 条缺陷数据");
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"[{MachineCode}] MySQL 读取错误: {ex.Message}");
                    MachineStatus = MachineStatusEnum.Shutdown;
                }
            }

            // 上报数据 - 使用泛型 DTO 直接上报
            if (realTimeDataDto.Details.Count > 0)
            {
                await PublishDefectDataAsync(realTimeDataDto);
            }
        }

        /// <summary>
        /// 上报缺陷数据 - 使用泛型 RealTimeDataDto&lt;DefectRecordDto&gt;
        /// </summary>
        private async Task<bool> PublishDefectDataAsync(RealTimeDataDto<DefectRecordDto> defectDataDto)
        {
            try
            {
                var res = await _mqttController.ConnectAsync();
                if (!res)
                {
                    LogController.Instance.Error($"[{MachineCode}] MQTT 连接失败");
                    return false;
                }

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None
                };

                // 序列化泛型 DTO 为 JSON
                string jsonPayload = JsonConvert.SerializeObject(defectDataDto, settings);

                // 上报到 MQTT
                res = await _mqttController.PublishMessageAsync(pubtopic, jsonPayload, false);

                if (res)
                {
                    LogController.Instance.Log($"[{MachineCode}] 缺陷数据上报成功，共 {defectDataDto.Details.Count} 条记录");
                }
                else
                {
                    LogController.Instance.Error($"[{MachineCode}] 缺陷数据上报失败");
                }

                return res;
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"[{MachineCode}] 上报缺陷数据异常: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 执行自定义 SQL 查询（用于扩展功能）
        /// </summary>
        protected async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null)
        {
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            cmd.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.GetValue(i);
                            }
                            results.Add(row);
                        }
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    LogController.Instance.Log($"[{MachineCode}] MySQL 连接测试成功");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"[{MachineCode}] MySQL 连接测试失败: {ex.Message}");
                return false;
            }
        }
    }
}
