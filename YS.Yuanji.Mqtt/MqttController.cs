using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using YS.Yuanji.Mqtt;
using YS.Yuanji.Log;

public class MqttController : IDisposable
{
    private IMqttClient _mqttClient;

    private volatile bool _isConnected;

    private static readonly object _lockflag = new object();

    private static MqttController _instance;

    private MqttClientOptionsBuilder mqttClientOptionsBuilder;

    private ConcurrentBag<Action<string, string>> recevice;
    private List<(string, int)> topics = new List<(string, int)>();
    private bool _autoConnect = false;

    private Task autotask;

    private string mqttUserName;
    private string mqttPassword;

    private bool disposedValue;
    private string server;
    private int port;
    private MqttConfig mqttConfig;
    public MqttController(IOptions<MqttConfig> options)
    {
        recevice = new ConcurrentBag<Action<string, string>>();
        _mqttClient = new MqttFactory().CreateMqttClient();
        _mqttClient.DisconnectedAsync += async delegate
        {
            LogController.Instance.Error("MQTT client disconnected.");
        };
        mqttConfig = options.Value;
        server = mqttConfig.Host;
        port = mqttConfig.Port;
        mqttUserName = mqttConfig.UserName;
        mqttPassword = mqttConfig.PassWord;
    }

    public async Task<bool> PublishMessageAsync(string topic, string message, bool isSaveMessage = true, [CallerMemberName] string? name = "", bool AtMostOnce = false)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                return false;
            }
            MqttApplicationMessage mqttMsg = new MqttApplicationMessageBuilder().WithTopic(topic).WithPayload(message).WithQualityOfServiceLevel((!AtMostOnce) ? MqttQualityOfServiceLevel.AtLeastOnce : MqttQualityOfServiceLevel.AtMostOnce)
                .Build();
            await _mqttClient.PublishAsync(mqttMsg, CancellationToken.None);
            if (!isSaveMessage)
            {
            }
            LogController.Instance.MqttLog($"caller:{name},MQTT Message '{message}' published to topic '{topic}' success.");
            return true;
        }
        catch (Exception ex)
        {
            Exception ex2 = ex;
            LogController.Instance.Error("caller:" + name + " Failed to publish message: " + ex2.Message);
            LogController.Instance.MqttError("caller:" + name + " Failed to publish message: " + ex2.Message);
            return false;
        }
    }

    public void LoadConfig(bool isautoconnect = true)
    {
        _autoConnect = isautoconnect;
    }

    public async Task<bool> ConnectAsync([CallerMemberName] string? name = "")
    {
        if (_mqttClient.IsConnected)
        {
            return true;
        }
        string clientId = Guid.NewGuid().ToString();
        MqttClientOptionsBuilder optionsBuilder = (mqttClientOptionsBuilder = new MqttClientOptionsBuilder().WithClientId(clientId).WithTcpServer(server, port).WithCredentials(mqttUserName, mqttPassword)
            .WithCleanSession());
        try
        {
            lock (_lockflag)
            {
                if (_mqttClient.IsConnected)
                {
                    return true;
                }
                _mqttClient.TryDisconnectAsync().Wait();
				_mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None).Wait();
                _mqttClient.ApplicationMessageReceivedAsync += delegate (MqttApplicationMessageReceivedEventArgs e)
                {
                    
                    string topic = e.ApplicationMessage.Topic;
                    string message = e.ApplicationMessage.ConvertPayloadToString();
                    LogController.Instance.Log($"subscribed from topic:{topic}.  payload:{message}.");
                    Parallel.ForEach(recevice, delegate (Action<string, string> action)
                    {
                        try
                        {
                            action?.Invoke(topic, message);
                        }
                        catch (Exception ex2)
                        {
                            LogController.Instance.Error("Error in MQTT message handler: " + ex2.Message);
                        }
                    });
                    return Task.CompletedTask;
                };
            }
            await Subcscribes();
            LogController.Instance.Log(name + " Connected to MQTT server successfully.");
        }
        catch (Exception ex)
        {
            LogController.Instance.Error("Caller:" + name + " Failed to connect to MQTT server: " + ex.Message);
            LogController.Instance.MqttError("Caller:" + name + " Failed to connect to MQTT server: " + ex.Message);
        }
        if (_autoConnect && autotask == null)
        {
            autotask = Task.Run(async delegate
            {
                while (!disposedValue)
                {
                    await Task.Delay(5000);
                    await EnsureConnectionAsync();
                }
            });
        }
        return _mqttClient.IsConnected;
    }

    private async Task EnsureConnectionAsync()
    {
        if (_mqttClient.IsConnected)
        {
            return;
        }
        try
        {
            lock (_lockflag)
            {
                if (_mqttClient.IsConnected)
                {
                    return;
                }
                _mqttClient.ConnectAsync(mqttClientOptionsBuilder.WithClientId(Guid.NewGuid().ToString()).Build()).Wait();
            }
            await Subcscribes();
            LogController.Instance.Log("MQTT client reconnected successfully.");
        }
        catch (Exception ex)
        {
            LogController.Instance.Error("Failed to reconnect MQTT client: " + ex.Message);
        }
    }

    public void RecvieData(Action<string, string> recvie, bool remove = false)
    {
        if (!remove)
        {
            recevice.Add(recvie);
            return;
        }
        Action<string, string> item = recevice.FirstOrDefault((Action<string, string> x) => (Delegate?)x == (Delegate?)recvie);
        if (item != null)
        {
            recevice.TryTake(out item);
        }
    }

    public async Task Unsubscribe(string topic)
    {
        if (!_mqttClient.IsConnected)
        {
            LogController.Instance.Error("MQTT client is not connected.");
            return;
        }
        try
        {
            await _mqttClient.UnsubscribeAsync(topic);
            topics.RemoveAll(((string, int) t) => t.Item1 == topic);
            LogController.Instance.Log("Unsubscribed from topic '" + topic + "' successfully.");
        }
        catch (Exception ex)
        {
            LogController.Instance.Error("Failed to unsubscribe from topic '" + topic + "': " + ex.Message);
        }
    }

    public async Task Subscribe(string topic, int level = 0)
    {
        try
        {
            topics.Add((topic, level));

            MqttQualityOfServiceLevel mqttQualityOfServiceLevel = level switch
            {
                0 => MqttQualityOfServiceLevel.AtMostOnce,
                1 => MqttQualityOfServiceLevel.AtLeastOnce,
                2 => MqttQualityOfServiceLevel.ExactlyOnce,
                _ => MqttQualityOfServiceLevel.AtMostOnce,
            };

            MqttQualityOfServiceLevel lev = mqttQualityOfServiceLevel;
            if (!_isConnected || !_mqttClient.IsConnected)
            {
                //LogController.Instance.Error("MQTT client is not connected.");
            }
            else
            {
                await _mqttClient.SubscribeAsync(topic, lev);
            }
        }
        catch (Exception ex)
        {
            Exception ex2 = ex;
            LogController.Instance.Error($"Subscribe  exception:{ex2},{ex2?.StackTrace}");
        }
    }

    private async Task Subcscribes()
    {
        try
        {
            if (!_mqttClient.IsConnected || topics.Count == 0)
            {
                return;
            }
            MqttClientSubscribeOptions tt = new MqttClientSubscribeOptions
            {
                TopicFilters = topics.Select<(string, int), MqttTopicFilter>(delegate ((string, int) topics)
                {
                    MqttTopicFilter mqttTopicFilter = new MqttTopicFilter
                    {
                        Topic = topics.Item1
                    };
                    int item = topics.Item2;
                    MqttQualityOfServiceLevel qualityOfServiceLevel = item switch
                    {
                        0 => MqttQualityOfServiceLevel.AtMostOnce,
                        1 => MqttQualityOfServiceLevel.AtLeastOnce,
                        2 => MqttQualityOfServiceLevel.ExactlyOnce,
                        _ => MqttQualityOfServiceLevel.AtMostOnce,
                    };
                    mqttTopicFilter.QualityOfServiceLevel = qualityOfServiceLevel;
                    return mqttTopicFilter;
                }).ToList()
            };
            //LogController.Instance.Log("MQTT client subscribed to topics: " + string.Join(", ", tt.TopicFilters.Select<MqttTopicFilter, string>(delegate (MqttTopicFilter topic)
            //{
            //    return topic.Topic;
            //})));
            await _mqttClient.SubscribeAsync(tt);
        }
        catch (Exception ex)
        {
            Exception ex2 = ex;
            LogController.Instance.Error("Failed to subscribe " + ex2.Message);
        }
    }

    public void OnStop()
    {
        try
        {
            _mqttClient?.DisconnectAsync().Wait();
            recevice.Clear();
            topics.Clear();
            _mqttClient?.Dispose();
            _mqttClient = null;
        }
        catch (Exception ex)
        {
            LogController.Instance.Error($"MqttController OnStop exception:{ex},{ex?.StackTrace}");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnStop();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
