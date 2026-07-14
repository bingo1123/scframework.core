using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace YS.Yuanji.Log;

public class HttpClientService
{
	private ConcurrentDictionary<string, string> _lastRedisLog = new();
	private ConcurrentDictionary<string, DateTime> _lastUpdateTime = new();
	private Timer _timer;
	private IConfiguration configuration;
	private readonly HttpClient _httpClient;

	public HttpClientService()
	{
		_httpClient = new HttpClient()
		{
			Timeout = TimeSpan.FromSeconds(3)
		};
		var json =File.ReadAllText("appsettings.json");

		var dymc = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

		// 如果配置里有根地址，设置 BaseAddress 方便后续使用相对地址
		if(dymc?.ContainsKey("ServerRoot") == true)
		{
			_httpClient.BaseAddress = new Uri(dymc["ServerRoot"].Replace("8183", "8188"));
		}
		_timer = new Timer(SendApi, null, 10000, 10000); // 主动降频发送
	}

	public HttpClientService(IConfiguration configuration)
	{
		this.configuration = configuration;
		_httpClient = new HttpClient()
		{
			Timeout = TimeSpan.FromSeconds(3)
		};

		// 如果配置里有根地址，设置 BaseAddress 方便后续使用相对地址
		if(!string.IsNullOrWhiteSpace(configuration["ServerRoot"]))
		{
			_httpClient.BaseAddress = new Uri(configuration["ServerRoot"]);
		}

		_timer = new Timer(SendApi, null, 10000, 10000); // 主动降频发送
	}

	private void SendApi(object? state)
	{
		try
		{
			//删除一直没有更新的Key，避免内存泄漏
			_lastUpdateTime.Where(w => w.Value < DateTime.Now.AddSeconds(-60 * 3))
				.ToList()
				.ForEach(f =>
				{
					_lastRedisLog.TryRemove(f.Key, out _);
					_lastUpdateTime.TryRemove(f.Key, out _);
				} );

			var updateKeys = _lastUpdateTime
				.Where(w => w.Value > DateTime.Now.AddSeconds(-18))
				.Select(s => s.Key)
				.ToList(); // 取10秒內值发生变化的Key

			foreach(var key in updateKeys)
			{
				if(_lastRedisLog.TryGetValue(key, out var value))
				{
					// Fire-and-forget 安全调用，内部捕获异常以免 Timer 崩溃
					_ = CallPostApiAsync(key, value);
				}
			}
		}
		catch
		{
			// 发送错误信息失败时不要再对外抛异常了，以免导致调用程序崩溃
		}
	}

	private async Task CallPostApiAsync(string key, string value)
	{
		try
		{
			var requestDto = new SendRedisLogDto()
			{
				Topic = "YjconError",
				Key = key,
				Value = value
			};

			var json = JsonConvert.SerializeObject(requestDto);
			using var content = new StringContent(json, Encoding.UTF8, "application/json");

			// 使用相对路径，如果 BaseAddress 未设置，构建绝对 URI
			var requestUri = "/realapi/wgreject/WgReject/SendRedisLog";
			HttpResponseMessage response;
			if(_httpClient.BaseAddress != null)
			{
				response = await _httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
			}
			else
			{
				var fullUri = (configuration["ServerRoot"] ?? "") + requestUri;
				response = await _httpClient.PostAsync(fullUri, content).ConfigureAwait(false);
			}

			response.EnsureSuccessStatusCode();
			var retContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			// 可根据需要处理 retContent 或记录日志
		}
		catch(TaskCanceledException)
		{
			// 超时或取消，按需记录或忽略
		}
		catch(Exception)
		{
			// 捕获所有异常以保证不抛出到 Timer 线程，按需记录日志
		}
	}


	#region Post请求
	public Task Post(string key, string value)
	{
		_lastRedisLog[key] = value;
		_lastUpdateTime[key] = DateTime.Now;
		return Task.CompletedTask;
	}
	#endregion
}
public class SendRedisLogDto
{
	public string Topic { get; set; }
	public string Key { get; set; }
	public string Value { get; set; }
}