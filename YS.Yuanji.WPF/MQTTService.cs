using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace YS.Yuanji.WPF
{
    public class MQTTService : IDisposable
    {
        private IManagedMqttClient _mqttClient;
        private MqttConfig _config;
        private bool _isDisposed = false;

        public event EventHandler<RealTimeDataDto> RealTimeDataReceived;
        public event EventHandler<bool> ConnectionStateChanged;

        public bool IsConnected => _mqttClient?.IsConnected ?? false;
        public bool IsConnecting => _mqttClient?.IsStarted ?? false;

        private List<string> topics = new List<string> {  };
        public MQTTService(MqttConfig config,List<string> topic = null) 
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            topics = topic ?? topics;
        }

        public async Task StartAsync()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(MQTTService));

            try
            {
                // 配置MQTT客户端选项
                var clientOptionsBuilder = new MqttClientOptionsBuilder()
                    .WithClientId(_config.ClientId)
                    .WithTcpServer(_config.Server, _config.Port)
                    .WithCleanSession(false) // 保持会话
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(30)) // 保持连接心跳
                    .WithTimeout(TimeSpan.FromSeconds(10)); // 操作超时

                // 如果提供了用户名和密码，则添加认证信息
                if (!string.IsNullOrEmpty(_config.Username))
                {
                    clientOptionsBuilder = clientOptionsBuilder.WithCredentials(_config.Username, _config.Password);
                }

                // 配置托管客户端选项
                var options = new ManagedMqttClientOptionsBuilder()
                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(5)) // 自动重连间隔
                    .WithClientOptions(clientOptionsBuilder.Build())
                    .Build();

                // 创建或重新创建MQTT客户端
                if (_mqttClient == null)
                {
                    var factory = new MqttFactory();
                    _mqttClient = factory.CreateManagedMqttClient();

                    // 注册事件处理程序
                    _mqttClient.ConnectedAsync += OnConnectedAsync;
                    _mqttClient.DisconnectedAsync += OnDisconnectedAsync;
                    _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;
                }

                // 启动客户端
                await _mqttClient.StartAsync(options);

                Console.WriteLine($"MQTT客户端启动: {_config.Server}:{_config.Port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MQTT客户端启动失败: {ex.Message}");
                throw;
            }
        }

        public async Task StopAsync()
        {
            if (_mqttClient != null && _mqttClient.IsStarted)
            {
                await _mqttClient.StopAsync();
            }
        }

        public async Task SubscribeToTopicAsync(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                topics?.ForEach(async f=> await _mqttClient.SubscribeAsync($"realtime/{f}"));
                Console.WriteLine($"已订阅主题: {topic}");
            }
        }

        public async Task UnsubscribeFromTopicAsync(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));

            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.UnsubscribeAsync(topic);
                Console.WriteLine($"已取消订阅主题: {topic}");
            }
        }

        private async Task OnConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("MQTT客户端已连接");
            ConnectionStateChanged?.Invoke(this, true);

            // 自动订阅实时数据主题
            await SubscribeToTopicAsync("realtime/#");
        }

        private async Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine($"MQTT客户端断开连接: {arg.Reason}");
            ConnectionStateChanged?.Invoke(this, false);

            // 如果是意外断开，会自动重连（由ManagedMqttClient处理）
        }

        private async Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
        {
            try
            {
                var topic = arg.ApplicationMessage.Topic;
                var payload = arg.ApplicationMessage.PayloadSegment;

                // 处理实时数据主题
                if (topic.StartsWith("realtime/"))
                {
                    var message = System.Text.Encoding.UTF8.GetString(payload);
                    var data = JsonConvert.DeserializeObject<RealTimeDataDto>(message);
                    if (data != null)
                    {
                        RealTimeDataReceived?.Invoke(this, data);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"处理MQTT消息失败: {ex.Message}");
            }

            return ;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    // 清理托管资源
                    if (_mqttClient != null)
                    {
                        _mqttClient.Dispose();
                        _mqttClient = null;
                    }
                }

                _isDisposed = true;
            }
        }
    }
}