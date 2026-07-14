using System.Net.Http.Headers;
using System.Net;
using YS.Yuanji.Commom;
using YS.Yuanji.Drive;
using YS.Yuanji.Log;
using System.Collections.Concurrent;
using System.Diagnostics.SymbolStore;
using System.ComponentModel.DataAnnotations;

namespace YS.ShareFile.Picture
{
    public class PictureController : DataCollectContoller
    {
        private string _minoUrl = string.Empty;
        private string _sharePath = string.Empty;
        private string _deviceId = string.Empty;
        private string _savePath = "D:\\WgPic";
        private List<string> _fileExtends;
        private List<string> _fiterName;
        private List<string> _timeDate;
        //当前程序运行目录
        private string staticFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "search_state.json");
        public PictureController(MqttController mqttController) : base(mqttController)
        {

        }

        public override Task Initialization()
        {
            _minoUrl = DeviceConfig.ParameterDict["MinoUrl"];
            _sharePath = DeviceConfig.ParameterDict["SharePath"];
            _deviceId = DeviceConfig.ParameterDict["DeviceId"];
            _savePath = DeviceConfig.ParameterDict.TryGetValue("SavePath", out var _sp) ? _sp: _savePath;
            _fileExtends = DeviceConfig.ParameterDict.TryGetValue("FileExtends", out var _fe) ? _fe.Split(',').ToList() : _fileExtends;
            _fiterName = DeviceConfig.ParameterDict.TryGetValue("FiterName", out var _fn) ? _fn.Split(',').ToList() : _fiterName;
            _timeDate = DeviceConfig.ParameterDict.TryGetValue("TimeDate", out var _t) ? _t.Split(',').ToList() : _timeDate;
            return base.Initialization();
        }

        protected override async Task RealtimeAsync(List<Item> items)
        {
            _timeDate = _timeDate ?? new List<string> { DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") };

            var re = await IncrementalFileSearcher.SearchFilesIncrementalAsync(_sharePath, _timeDate, _fileExtends, _fiterName,
               async file =>
            {
                // 将file文件替换成指定后缀为txt的相同文件名
                var newFile = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".txt");
                var fileName = Path.GetFileName(file);
                string cameraCode = ""; string moduleCode = ""; string rejectCode = "";
                if (File.Exists(newFile))
                {
                    var rej = await GetImageString(newFile);
                    var cacode = rej.Split(".").Length > 0 ? rej.Split(".")[0].Substring(3, 1) : "1";
                    cameraCode = int.TryParse(cacode, out var cc) ? (cc - 1).ToString() : "xx";
                    rejectCode = rej.Split(".").Length > 1 ? rej.Split(".")[1].Substring(5) : "";
                    fileName = rej.Split(".").Length > 1 ? rej.Split(".")[1] + " " + fileName : fileName;
                }

                var fileTime = DateTime.Now.ToString();    //File.GetLastWriteTime(file).ToString("yyyy-MM-dd HH:mm:ss");
                var fileBytes = await GetImage(file);

                var put = await SendImage(_minoUrl, _deviceId, fileTime, fileBytes, fileName, cameraCode, moduleCode, rejectCode);
                if(put)
                {
                   var savePath = Path.Combine(_savePath, _deviceId, DateTime.Now.ToString("yyyy-MM-dd"), Path.GetFileName(fileName));
                   await SaveImages(savePath, fileBytes);
                }
                return put;
            }, staticFilePath);
            re.PrintSummary();
            _timeDate = null;
            return;
        }


        public async Task<byte[]> GetImage(string filePath, int timeoutMilliseconds = 5000)
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(timeoutMilliseconds); // 设置超时后取消[citation:5]

                try
                {
                    // 将取消令牌传递给异步读取任务
                    return await File.ReadAllBytesAsync(filePath, cts.Token);
                }
                catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
                {
                    // 处理因超时而取消的情况[citation:5]
                    throw new TimeoutException($"Reading the file '{filePath}' timed out after {timeoutMilliseconds}ms.");
                }
                // 其他异常（如FileNotFoundException）会正常抛出
            }
        }

        public async Task<string> GetImageString(string filePath, int timeoutMilliseconds = 5000)
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(timeoutMilliseconds); // 创建取消令牌

                try
                {
                    return await File.ReadAllTextAsync(filePath, cts.Token);
                }
                catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
                {
                    // 处理超时异常
                    throw new TimeoutException($"Reading the file '{filePath}' timed out after {timeoutMilliseconds}ms.");
                }
                // 其他异常（如FileNotFoundException）会正常抛出
            }
        }

        private async Task<bool> SendImage(string url, string deviceId, string fileTime, byte[] fileBytes, string fileName,
            string cameraCode = "", string moduleCode = "", string rejectCode = "")
        {
            using (var clientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            using (var client = new HttpClient(clientHandler))
            {
                var requestUri = new Uri(url);
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(deviceId), "deviceId");
                content.Add(new StringContent(fileTime), "testtime");
                content.Add(new StringContent(cameraCode), "cameraCode");
                content.Add(new StringContent(moduleCode), "moduleCode");
                content.Add(new StringContent(rejectCode), "rejectCode");

                var fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = fileName
                };
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(fileContent);

                var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
                {
                    Headers =
                    {
                        { "Accept", "*/*" },
                        { "User-Agent", "PostmanRuntime-ApipostRuntime/1.1.0" },
                        { "Connection", "keep-alive" }
                    },
                    Content = content
                };

                using (var response = await client.SendAsync(request))
                {
                    try
                    {
                        response.EnsureSuccessStatusCode();
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            LogController.Instance.Log($"上传图片{fileName}成功");
                            return true;
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        LogController.Instance.Error($"上传图片{fileName}异常 Error: {ex}");
                        return false;
                    }
                }
            }
            return false;
        }

        private async Task SaveImages(string path, byte[] content)
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(5000); // 创建取消令牌
                try
                {
                    if(!Directory.Exists(Path.GetDirectoryName(path)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    await File.WriteAllBytesAsync(path, content, cts.Token);
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"保存图片文件{path}异常 Error: {ex}");
                }
            }
        }
    }
}
