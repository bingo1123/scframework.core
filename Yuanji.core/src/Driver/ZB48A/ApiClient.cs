using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using  Yuanji.core.src.Driver.Huani.Entity;
using YS.Yuanji.Log;


namespace  Yuanji.core.src.Driver.ZB48A
{
    public class ApiClient
    {
        #region Property

        private readonly ApiClientConfig config;

        private  HttpClient httpClient ;

        //public ApiClient()
        //{
        //    //默认连接数限制为2，增加连接数限制
        //    ServicePointManager.DefaultConnectionLimit = 512;
        //    //启用保活机制（保持活动超时设置为 2 小时，并将保持活动间隔设置为 1 秒。）
        //    ServicePointManager.SetTcpKeepAlive(true, 7200000, 1000);

        //    //var handler = new SocketsHttpHandler
        //    //{
        //    //    // 定期刷新连接池，避免 DNS 缓存问题
        //    //    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        //    //    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
        //    //    MaxConnectionsPerServer = 10
        //    //};
        //    httpClient = new HttpClient()
        //    {
        //        Timeout = TimeSpan.FromSeconds(15) // 设置超时时间，防止请求无限等待
        //    };
        //    //增加保活机制，表明连接为长连接
        //    httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

        //    httpClient.DefaultRequestHeaders.Add("apiAuth", "API");
        //}

        public ApiClient(ApiClientConfig config)
        {
            //默认连接数限制为2，增加连接数限制
            ServicePointManager.DefaultConnectionLimit = 512;
            //启用保活机制（保持活动超时设置为 2 小时，并将保持活动间隔设置为 1 秒。）
            ServicePointManager.SetTcpKeepAlive(true, 7200000, 1000);

            //var handler = new SocketsHttpHandler
            //{
            //    // 定期刷新连接池，避免 DNS 缓存问题
            //    PooledConnectionLifetime = TimeSpan.FromMinutes(5),
            //    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
            //    MaxConnectionsPerServer = 10
            //};
            httpClient = new HttpClient()
            {
                //Timeout = TimeSpan.FromSeconds(15) // 设置超时时间，防止请求无限等待
            };
            //增加保活机制，表明连接为长连接
            httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");

            httpClient.DefaultRequestHeaders.Add("apiAuth", "API");
            this.config = config;
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取签名后的URL
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GenerateSignedUrl(string endpoint, Dictionary<string, string> parameters, int? port = null)
        {
            config.ApiId = "mro-api";
            parameters["_apiId"] = config.ApiId;
            parameters["_timestamp"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            parameters["_version"] = "1.0";

            var sign = GenerateSignature(parameters);
            parameters["_sign"] = sign;

            //var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            var queryString = string.Join("&", parameters.Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));
            var baseUrl = port.HasValue
            ? $"http://{config.IpAddress}:{port.Value}"
            : config.baseUrl;
            return $"{baseUrl}{endpoint}?{queryString}";
        }

        /// <summary>
        /// 生成签名
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GenerateSignature(Dictionary<string, string> parameters)
        {
            var sortedParams = new SortedDictionary<string, string>(parameters);

            var signData = new StringBuilder();

            foreach (var param in sortedParams)
            {
                if (param.Key != "_sign" && param.Value != null)
                {
                    signData.Append(param.Key).Append(param.Value);
                }
            }

            signData.Append(config.ApiSecret);

            // 生成 MD5 签名
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(signData.ToString()));
                return string.Concat(hashBytes.Select(b => b.ToString("x2")));
            }
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<string> GetAsync(string endpoint, Dictionary<string, string> parameters, int? port = null, [CallerMemberName] string? name = null)
        {
            string url = config.baseUrl;
            try
            {
                url = GenerateSignedUrl(endpoint, parameters,port);
                var response = await httpClient.GetAsync(url);
                var msg = await response.Content.ReadAsStringAsync();
                LogController.Instance.Log($"caller:{name}-sendRequest{url},{endpoint},GetAsync response before EnsureSuccessStatusCode:{msg}");
                //response.EnsureSuccessStatusCode();
                return msg;
            }
            catch (TaskCanceledException ex)
            {
                LogController.Instance.Error($"{name}-sendRequest{url},{endpoint}in GetAsync Request timed out.{ex}");
                return $"Request timed out.{ex}";
            }
            catch (HttpRequestException ex)
            {
                LogController.Instance.Error($" {name}-sendRequest{url},endpoint{endpoint}in GetAsync HTTP request failed.{ex}");
                return  $"HTTP request failed: {ex.Message}";
            }
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string> parameters, int? port = null, [CallerMemberName] string? name = null) where T : class
        {
            string url = config.baseUrl;
            try
            {
                url = GenerateSignedUrl(endpoint, parameters, port);
                var response = await httpClient.GetAsync(url);
                var msg = await response.Content.ReadAsStringAsync();
                LogController.Instance.Log($"caller:{name}-sendRequest{url},{endpoint},GetAsync response before EnsureSuccessStatusCode:{msg}");
                //response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<T>(msg); // Ensure T? is nullable to handle potential null values.
            }
            catch (TaskCanceledException ex)
            {
                LogController.Instance.Error($"{name}-sendRequest{url},{endpoint}in GetAsync Request timed out.{ex}");
                return null; // Return null explicitly in case of an exception.
            }
            catch (HttpRequestException ex)
            {
                LogController.Instance.Error($" {name}-sendRequest{url},endpoint{endpoint}in GetAsync HTTP request failed.{ex}");
                return null; // Return null explicitly in case of an exception.
            }
        }

        public async Task<string> PostAsync(string endpoint, Dictionary<string, string> parameters, int? port = null)
        {
            var url = config.baseUrl;
            try
            {
                GenerateSignedUrl(endpoint, parameters);
                using (var request = new HttpRequestMessage(HttpMethod.Post, url))
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json");
                    var res = await httpClient.SendAsync(request);
                    //res.EnsureSuccessStatusCode();
                    var msg = await res.Content.ReadAsStringAsync();
                    LogController.Instance.Log($"sendRequest{url},{endpoint},PostAsync response before EnsureSuccessStatusCode:{msg}");

                    return msg;
                }
            }
            catch (TaskCanceledException ex)
            {
                LogController.Instance.Error($"{url} in PostAsync Request timed out.{ex}");
                return $"in PostAsync Request timed out.{ex}";
            }
            catch (HttpRequestException ex)
            {
                LogController.Instance.Error($"{url} in PostAsync HTTP request failed: {ex}");
                return $"in PostAsync HTTP request failed: {ex}";
            }
        }


        //public string Get(string endpoint, Dictionary<string, string> parameters)
        //{
        //    var url = GenerateSignedUrl(endpoint, parameters);

        //    // 创建 HttpRequestMessage，设置为 GET 方法并使用生成的 URL
        //    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
        //    {
        //        // 发送请求并获取响应
        //        using (var response = httpClient.Send(request))
        //        {
        //            response.EnsureSuccessStatusCode();
        //            return response.Content.ReadAsStringAsync().Result;  // 使用 Result 获取同步结果
        //        }
        //    }
        //}
        #endregion

        #region 业务逻辑
        public const int OKCode = 200;

        public const string OKMsg = "SUCCEED";

        public readonly static string[] PartNames = new string[] { "YB418", "YB518", "YB618" };

        public const string CodeFor418 = "xx_";

        public const string CodeFor518 = "ch_";

        public const string CodeFor618 = "ct_";

        public const string YB418Name = "YB418";

        public const string YB518Name = "YB518";

        public const string YB618Name = "YB618";

        public const string xx_exit_pdt = "小包机产量";

        public const string xx_total_reject = "小包机剔除量";

        public const string ch_exit_pdt = "小透机产量";

        public const string ch_reject = "小透机剔除量";

        public const string ct_pdt = "条包机产量";

        public const string ct_reject = "条包机剔除量";

        public const string xx_label_paper_total_expend = "小盒商标纸总耗";

        public const string ct_label_paper_total_expend = "条盒商标纸总耗";

        public const string packer_yield_rate = "烟包成品率";

        public const string packer_waste_rate = "烟包废品率";

        /// <summary>
        /// 设备单机状态接口
        /// </summary>
        public const string CurrentMachineStatusEndpoint = "/runningStatus/getCurrentMachineStatus";

        /// <summary>
        /// 机器性能
        /// <remark>
        /// 运行时间、等待时间、停机时间、最大运行时长、平均运行时长、设备运行效率、设备综合效率、设备状态等
        /// </remark>
        /// </summary>
        public const string CurrentShiftRunningStatusEndpoint = "/runningStatus/getCurrentShiftRunningStatus";

        /// <summary>
        /// 设备停机信息
        /// </summary>
        public const string HaltEquipmentEndpoint = "/api/haltEquipment";

        /// <summary>
        /// 设备参数当前值及单位
        /// </summary>
        public const string ParameterEquipmentEndpoint = "/api/parameterEquipment";

        /// <summary>
        /// 设备班次停机分析接口-对当班停机情况分析
        /// </summary>
        public const string HaltAnalysisEndpoint = "/api/haltAnalysis";

        /// <summary>
        /// 能耗统计接口-查询各个单机每小时能耗使用情况
        /// </summary>
        public const string HourConsumptionEndpoint = "/sampleElectric/hourConsumption";

        /// <summary>
        /// 故障记录接口-设备报警信息（不导致停机）
        /// </summary>
        public const string malfunctionEquipmentEndpoint = "/api/malfunctionEquipment";


        /// <summary>
        /// 参数修改记录接口
        /// <remark>
        /// 记录每次参数修改记录，包含旧值、新值和修改时间</remark>
        /// </summary>
        public const string ModifyRecordEndpoint = "/api/parameterModifyRecord";

        /// <summary>
        /// 全流程消耗接口
        /// <remark>查询包装机各个单元出入口产量、剔除量、损耗率</remark>
        /// </summary>
        public const string ShiftCountEndpoint = "/api/senior/shiftCounts";

        /// <summary>
        /// 剔除数据接口
        /// <remark>查询当班次设备剔除明细及其占比</remark>
        /// </summary>
        public const string RejectShiftCountsEndpoint = "/api/rejectShiftCounts";

        /// <summary>
        /// 实际车速
        /// <remark>设备各个单机的实际车速。</remark>
        /// </summary>
        public const string SpeedEndpoint = "/state/speed";

        /// <summary>
        /// 生产数据-
        /// <remark>查询包装机产量、剔除量、小盒商标纸、条盒纸总耗、烟包成品率、废品率</remark>
        /// </summary>
        public const string ProductionDataEndpoint = "/api/standard/shiftCounts";
        #endregion
    }
}
