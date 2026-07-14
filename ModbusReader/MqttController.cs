using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using System.Runtime.CompilerServices;

public class MqttController
{
    #region Property
    private IMqttClient _mqttClient;

    private volatile bool _isConnected;

    private readonly static object _lockflag = new object();
    #endregion

    #region Sington
    private static readonly object _lock = new object();

    private static MqttController _instance;

    private MqttClientOptionsBuilder mqttClientOptionsBuilder;

    private Task _conTask;

    public static MqttController Instance
    {
        get
        {
            if (_instance == null)
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new MqttController();
                }
            return _instance;
        }
    }

    private MqttController()
    {
        _mqttClient = new MqttFactory().CreateMqttClient();

        _mqttClient.DisconnectedAsync += (async e =>
        {
            LogController.Instance.Error("MQTT client disconnected.");
            _isConnected = false;

        });

        _conTask = new Task(async () =>
        {
            while (true)
            {
                await Task.Delay(5000);
                EnsureConnectionAsync();
            }
        });

    }
    #endregion

    #region Method

    public async Task<bool> PublishMessageAsync(string topic, string message, bool isSaveMessage = true, [CallerMemberName] string? name = "", bool AtMostOnce = false)
    {
        try
        {
            if (!_isConnected || !_mqttClient.IsConnected)
            {
                return false;
            }

            var mqttMsg = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(message)
            .WithQualityOfServiceLevel(AtMostOnce ? MqttQualityOfServiceLevel.AtMostOnce : MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

            await _mqttClient.PublishAsync(mqttMsg, CancellationToken.None);
            string msg = isSaveMessage == true ? message : "isNoeSaveMessage";
            LogController.Instance.MqttLog($"caller:{name},MQTT Message '{message}' published to topic '{topic}' success.");
            return true;
        }
        catch (Exception ex)
        {
            lock (_lockflag)
                _isConnected = false;
            LogController.Instance.Error($"caller:{name} Failed to publish message: {ex.Message}");
            LogController.Instance.MqttError($"caller:{name} Failed to publish message: {ex.Message}");
            return false;
        }

    }

    private string mqttUserName;

    private string mqttPassword;

    public void LoadConfig(string user, string password)
    {
        mqttUserName = user;
        mqttPassword = password;
    }

    public async Task<bool> ConnectAsync(string server, int port, [CallerMemberName] string? name = "")
    {
        if (_isConnected) return true;
        string clientId = Guid.NewGuid().ToString();
        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(clientId)
            .WithTcpServer(server, port)
            .WithCredentials(mqttUserName, mqttPassword)
            .WithCleanSession();
        mqttClientOptionsBuilder = optionsBuilder;
        try
        {
            lock (_lockflag)
            {
                if (_isConnected) return true;
                _mqttClient.ConnectAsync(optionsBuilder.Build(), CancellationToken.None).Wait();
                _isConnected = true;
            }
            return true;
        }
        catch (Exception ex)
        {
            LogController.Instance.Error($"Caller:{name} Failed to connect to MQTT server: {ex.Message}");
            LogController.Instance.MqttError($"Caller:{name} Failed to connect to MQTT server: {ex.Message}");
        }
        return false;
    }

    /// <summary>
    /// 连接状态判读
    /// </summary>
    /// <returns></returns>
    private void EnsureConnectionAsync()
    {
        if (!_isConnected || !_mqttClient.IsConnected)
        {
            try
            {
                lock (_lockflag)
                {
                    if (_isConnected) return;
                    _mqttClient.ConnectAsync(mqttClientOptionsBuilder.WithClientId(Guid.NewGuid().ToString())
                    .Build()).Wait();
                    _isConnected = true;
                }
                LogController.Instance.Log("MQTT client reconnected successfully.");
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"Failed to reconnect MQTT client: {ex.Message}");
            }
        }
    }

    #endregion
}
