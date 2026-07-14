using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using NModbus;
using System.Collections.ObjectModel;

namespace ModbusReader
{

    public partial class MainWindow : Window
    {
        // Modbus连接
        private TcpClient _tcpClient;
        private IModbusMaster _modbusMaster;
        private volatile bool _isConnected;

        // 配置数据
        private AppConfig _config;
        private ObservableCollection<PointData> _pointData = new ObservableCollection<PointData>();
        private CacheContext CacheContext;
        // 重连和自动读取
        private CancellationTokenSource _cts;
        private bool _isAutoReading;
        private DispatcherTimer _autoReadTimer;

        // 托盘图标
        private NotifyIcon _notifyIcon;

        // 心跳检测
        private System.Threading.Timer _heartbeatTimer;
        private volatile bool _isReconnecting;
        private const int HeartbeatInterval = 5000; // 心跳间隔(ms)

        public MainWindow()
        {
            InitializeComponent();
            InitializeTrayIcon();

        }

        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new System.Drawing.Icon("Resources/tray.ico");
            _notifyIcon.Text = "在线水分仪数据监控";
            _notifyIcon.Visible = false;

            // 添加托盘菜单
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("显示主界面", null, (s, args) => ShowMainWindow());
            contextMenu.Items.Add("退出", null, (s, args) => CloseApplication());
            _notifyIcon.ContextMenuStrip = contextMenu;

            // 双击托盘图标事件
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
            _notifyIcon.Visible = false;
        }

        private void CloseApplication()
        {
            if (_isAutoReading)
            {
                var result = MessageBox.Show("自动读取仍在运行，确定要退出吗？",
                    "确认退出", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No) return;
            }

            _notifyIcon?.Dispose();
            Application.Current.Shutdown();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 初始化窗口位置
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            await LoadConfig();
            InitializePointData();

            // 设置数据网格源
            dataGrid.ItemsSource = _pointData;

            // 初始化自动读取计时器
            _autoReadTimer = new DispatcherTimer();
            _autoReadTimer.Tick += AutoReadTimer_Tick;
            int interval = _config.Connection.ReadInterval; // 默认2秒
            _autoReadTimer.Interval = TimeSpan.FromMilliseconds(interval);
            _autoReadTimer.Start();
            BtnConnect_Click(null, null);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                _notifyIcon.Visible = true;
                _notifyIcon.ShowBalloonTip(1000, "在线水分仪数据监控", "应用程序已最小化到托盘", ToolTipIcon.Info);
            }
            else if (WindowState == WindowState.Normal)
            {
                _notifyIcon.Visible = false;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // 取消关闭操作，改为最小化到托盘
            e.Cancel = true;
            WindowState = WindowState.Minimized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ?
                WindowState.Normal : WindowState.Maximized;

            // 更新最大化按钮文本
            btnMaximize.Content = WindowState == WindowState.Maximized ? "❐" : "□";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // 加载配置文件
        private async Task LoadConfig()
        {
            try
            {
                string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (!File.Exists(configFile))
                {
                    LogController.Instance.Error("配置文件 config.json 不存在！");
                    //MessageBox.Show("配置文件 config.json 不存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string json = File.ReadAllText(configFile);
                _config = JsonConvert.DeserializeObject<AppConfig>(json);
                CacheContext = new CacheContext();
                await CacheContext.InitConfig(_config.Basic);
                _config.Connection = CacheContext.TerminalConfig;
                MqttController.Instance.LoadConfig(_config.Connection.MqttUserName, _config.Connection.MqttUserPassword);
                // 更新状态栏
                MachineName.Text = $"机台名称:{_config.Basic.TerminalCode}";
                txtDeviceStatus.Text = $"设备: {_config.Connection.MachineIPAddress}:{_config.Connection.MachinePort} [ID:{_config.Connection.SlaveId}]";
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"加载配置失败: {ex.Message}");
                MessageBox.Show($"加载配置失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 初始化点位数据
        private void InitializePointData()
        {
            foreach (var point in _config.Points)
            {
                _pointData.Add(new PointData
                {
                    Name = point.Name,
                    Description = point.Description,
                    Address = point.Address,
                    RegisterType = point.RegisterType,
                    DataType = point.DataType,
                    Unit = point.Unit,
                    Value = "N/A",
                    Status = "未读取"
                });
            }
        }

        // 连接设备
        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (_isConnected) return;

            try
            {
                //btnConnect.IsEnabled = false;
                progressStatus.IsIndeterminate = true;
                txtConnectionStatus.Text = "正在连接...";

                await ConnectAsync();
               
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"连接失败: {ex.Message}");
                UpdateStatus($"连接失败: {ex.Message}", true);
                //btnConnect.IsEnabled = true;
                progressStatus.IsIndeterminate = false;
            }
        }

        // 异步连接方法
        private async Task ConnectAsync()
        {
            _cts = new CancellationTokenSource();

            try
            {
                // 创建TCP客户端
                _tcpClient = new TcpClient
                {
                    ReceiveTimeout = 3000,
                    SendTimeout = 3000
                };

                await _tcpClient.ConnectAsync(_config.Connection.MachineIPAddress, _config.Connection.MachinePort);

                var factory = new ModbusFactory();
                // 创建Modbus主站
                _modbusMaster = factory.CreateMaster(_tcpClient);

                // 测试连接
                await _modbusMaster.ReadHoldingRegistersAsync(_config.Connection.SlaveId, 0, 1);

                // 更新连接状态
                _isConnected = true;
                _isAutoReading = true;

                // 更新UI
                //btnDisconnect.IsEnabled = true;
                //btnRead.IsEnabled = true;
                //btnAutoRead.IsEnabled = true;
                UpdateStatus($"已连接到 {_config.Connection.MachineIPAddress}:{_config.Connection.MachinePort}", false);

                // 启动心跳检测
                StartHeartbeat();
               
                // 立即执行一次读取
                //Dispatcher.BeginInvoke(new Action(async () =>
                //{
                //    await ReadAllPointsAsync();
                //    txtLastUpdate.Text = $"最后更新: {DateTime.Now:HH:mm:ss}";
                //}));
            }
            catch (Exception ex)
            {
                if (_config.Connection.AutoReconnect)
                {
                    LogController.Instance.Error($"连接失败: {ex.Message}，尝试重连...");
                    UpdateStatus($"连接失败: {ex.Message}，尝试重连...", true);
                    StartReconnect();
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    progressStatus.IsIndeterminate = false;
                });
            }
        }

        // 判断是否为网络错误
        private bool IsNetworkError(Exception ex)
        {
            return ex is SocketException ||
                   ex is TimeoutException ||
                   ex is OperationCanceledException;
        }

        // 启动重连机制
        private void StartReconnect()
        {
            if (_isReconnecting) return;

            _isReconnecting = true;
            _isConnected = false;
            _isAutoReading = false;

            // 停止心跳检测
            StopHeartbeat();
           
            Task.Run(async () =>
            {
                try
                {
                    int attempts = 0;
                    int delay = _config.Connection.ReconnectDelay;

                    while (attempts < int.MaxValue && !_cts.IsCancellationRequested)
                    {
                        attempts++;
                        UpdateStatus($"尝试重新连接 ({attempts}/{_config.Connection.MaxReconnectAttempts}),等待 {delay / 1000}秒......", true);

                        await Task.Delay(delay, _cts.Token);

                        try
                        {
                            await ConnectAsync();
                            if (_isConnected) return;
                            // 失败后增加延迟
                            delay = (int)(delay * 1.5);
                            if (attempts >= _config.Connection.MaxReconnectAttempts)
                            {
                                attempts = 0;
                                delay = _config.Connection.ReconnectDelay;
                            }
                        }
                        catch (Exception ex)
                        {
                            // 失败后增加延迟
                            delay = (int)(delay * 1.5);
                            if (attempts >= _config.Connection.MaxReconnectAttempts)
                            {
                                attempts = 0;
                                delay = _config.Connection.ReconnectDelay;
                            }
                        }
                    }

                    if (!_isConnected)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            btnConnect.IsEnabled = true;
                            UpdateStatus($"重连失败，请检查设备状态", true);
                        });
                    }
                }
                finally
                {
                    _isReconnecting = false;
                }
            }, _cts.Token);
        }

        // 启动心跳检测
        private void StartHeartbeat()
        {
            _heartbeatTimer?.Dispose();
            _heartbeatTimer = new System.Threading.Timer(async _ =>
            {
                try
                {
                    if (_isConnected && _modbusMaster != null)
                    {
                        byte slaveId = _config.Connection.SlaveId;

                        // 简单读取测试连接
                        await _modbusMaster.ReadHoldingRegistersAsync(slaveId, 0, 1);
                    }
                }
                catch (Exception ex)
                {
                    LogController.Instance.Error($"连接丢失，尝试重连...{ex}");
                    // 心跳失败触发重连
                    UpdateStatus("连接丢失，尝试重连...", false);
                    StartReconnect();
                }

            }, null, _config.Connection.HeartbeatInterval, _config.Connection.HeartbeatInterval);
        }

        // 停止心跳检测
        private void StopHeartbeat()
        {
            _heartbeatTimer?.Dispose();
            _heartbeatTimer = null;
        }


        // 断开连接
        private void Disconnect()
        {
            try
            {
                _cts?.Cancel();
                _tcpClient?.Close();

                _isConnected = false;
                _isAutoReading = false;
                _autoReadTimer.Stop();

                // 更新UI
                btnConnect.IsEnabled = true;
                btnDisconnect.IsEnabled = false;
                btnRead.IsEnabled = false;
                btnAutoRead.IsEnabled = false;
                btnAutoRead.Content = "自动读取";

                UpdateStatus("已断开连接", false);
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"断开连接错误: {ex.Message}");
                UpdateStatus($"断开连接错误: {ex.Message}", true);
            }
        }

        // 断开按钮事件
        private void BtnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        // 手动读取数据
        private async void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConnected) return;

            try
            {
                progressStatus.IsIndeterminate = true;
                await ReadAllPointsAsync();
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"读取错误: {ex.Message}");
                UpdateStatus($"读取错误: {ex.Message}", true);
            }
            finally
            {
                progressStatus.IsIndeterminate = false;
                txtLastUpdate.Text = $"最后更新: {DateTime.Now:HH:mm:ss}";
            }
        }

        // 自动读取开关
        private void BtnAutoRead_Click(object sender, RoutedEventArgs e)
        {
            if (!_isConnected) return;

            _isAutoReading = !_isAutoReading;

            if (_isAutoReading)
            {
                //btnAutoRead.Content = "停止自动";

                // 获取读取间隔
                int interval = _config.Connection.ReadInterval; // 默认2秒
                //switch (cmbReadInterval.SelectedIndex)
                //{
                //    case 0: interval = 1000; break;
                //    case 1: interval = 2000; break;
                //    case 2: interval = 5000; break;
                //    case 3: interval = 10000; break;
                //}
                _autoReadTimer.Interval = TimeSpan.FromMilliseconds(interval);
                _autoReadTimer.Start();

                // 立即执行一次读取
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await ReadAllPointsAsync();
                    txtLastUpdate.Text = $"最后更新: {DateTime.Now:HH:mm:ss}";
                }));
            }
            else
            {
                //btnAutoRead.Content = "自动读取";
                _autoReadTimer.Stop();
            }
        }

        // 自动读取计时器事件
        private async void AutoReadTimer_Tick(object sender, EventArgs e)
        {
            if (!_isConnected || !_isAutoReading) return;

            try
            {
                await ReadAllPointsAsync();
                txtLastUpdate.Text = $"最后更新: {DateTime.Now:HH:mm:ss}";
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"自动读取错误: {ex.Message}");
                UpdateStatus($"自动读取错误: {ex.Message}", true);
            }
        }

        // 读取所有点位数据
        private async Task ReadAllPointsAsync()
        {
            int successCount = 0;
            int errorCount = 0;

            // 使用并行处理提高读取效率
            var tasks = new List<Task>();

            foreach (var point in _pointData)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        object value = await ReadPointAsync(point);

                        // 在UI线程更新数据
                        Dispatcher.Invoke(() =>
                        {
                            point.Value = FormatValue(value, point.DataType);
                            point.Status = "成功";
                            point.Timestamp = DateTime.Now;
                            successCount++;
                        });
                    }
                    catch (Exception ex)
                    {
                        LogController.Instance.Error($"ReadPointAsync读取数据失败: {ex.Message}");
                        Dispatcher.Invoke(() =>
                        {
                            point.Value = "N/A";
                            point.Status = $"错误: {ex.Message}";
                            point.Timestamp = DateTime.Now;
                            errorCount++;
                        });
                    }
                }));
            }

            await Task.WhenAll(tasks);

            await PublishDataAsync();

            // 更新状态栏计数
            Dispatcher.Invoke(() =>
            {
                txtDataCount.Text = $"{successCount}/{_pointData.Count} 点";
                txtErrorCount.Text = $"错误: {errorCount}";
                txtLastUpdate.Text = $"最后更新: {DateTime.Now:HH:mm:ss}";

                // 如果有错误，在托盘显示通知
                if (errorCount > 0 && !IsVisible)
                {
                    _notifyIcon.ShowBalloonTip(3000, "数据读取错误",
                        $"检测到 {errorCount} 个点位读取失败，请检查连接！",
                        ToolTipIcon.Error);
                }
            });
        }

        // 读取单个点位
        private async Task<object> ReadPointAsync(PointData point)
        {

            byte slaveId = _config.Connection.SlaveId;
            ushort address = point.Address;
            int step = GetDataTypeStep(point.DataType);

            switch (point.RegisterType)
            {
                case "Coil":
                    bool[] coils = await _modbusMaster.ReadCoilsAsync(slaveId, address, 1);
                    return coils[0];

                case "DiscreteInput":
                    bool[] inputs = await _modbusMaster.ReadInputsAsync(slaveId, address, 1);
                    return inputs[0];

                case "InputRegister":
                case "HoldingRegister":
                    ushort[] registers = point.RegisterType == "InputRegister" ?
                        await _modbusMaster.ReadInputRegistersAsync(slaveId, address, (ushort)step) :
                        await _modbusMaster.ReadHoldingRegistersAsync(slaveId, address, (ushort)step);

                    return ParseRegisterValue(registers, point.DataType);

                default:
                    throw new NotSupportedException($"不支持的寄存器类型: {point.RegisterType}");
            }
        }

        // 格式化值显示
        private string FormatValue(object value, string dataType)
        {
            return dataType switch
            {
                "Float" => $"{((float)value):F2}",
                "Bool" => (bool)value ? "True" : "False",
                "UInt16" => $"{value}",
                "Int16" => $"{value}",
                "UInt32" => $"{value}",
                "Int32" => $"{value}",
                _ => value.ToString()
            };
        }

        // 解析寄存器值
        private object ParseRegisterValue(ushort[] registers, string dataType)
        {
            switch (dataType)
            {
                case "UInt16":
                    return registers[0];

                case "Int16":
                    return (short)registers[0];

                case "UInt32":
                    return ((uint)registers[0] << 16) | registers[1];

                case "Int32":
                    return ((int)registers[0] << 16) | registers[1];

                case "Float":
                    byte[] bytes = {
                    (byte)(registers[0] >> 8),
                    (byte)registers[0],
                    (byte)(registers[1] >> 8),
                    (byte)registers[1]
                };
                    // 转换为浮点数（.NET 默认使用小端序，需要处理字节顺序）
                    if (BitConverter.IsLittleEndian)
                    {
                        // 反转字节顺序以适应小端序系统
                        Array.Reverse(bytes);
                    }
                    return BitConverter.ToSingle(bytes, 0);

                default:
                    return registers[0];
            }
        }

        // 获取数据类型所需寄存器数量
        private int GetDataTypeStep(string dataType)
        {
            return dataType switch
            {
                "UInt16" => 1,
                "Int16" => 1,
                "UInt32" => 2,
                "Int32" => 2,
                "Float" => 2,
                _ => 1
            };
        }

        // 更新状态信息
        private void UpdateStatus(string message, bool isError)
        {
            if (isError)
            {
                LogController.Instance.Error(message);
            }
            else
            {
                LogController.Instance.Log(message);
            }

            Dispatcher.Invoke(() =>
            {
                txtConnectionStatus.Text = message;
                txtConnectionStatus.Foreground = isError ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Black;
            });
        }

        /// <summary>
        /// 推送数据.
        /// </summary>
        /// <returns></returns>
        public async Task PublishDataAsync()
        {
            try
            {
                RealTimeDataDto realTimeDataDto = new RealTimeDataDto();
                realTimeDataDto.MachineId = _config.Connection.MachineName;
                realTimeDataDto.Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                realTimeDataDto.Details = new List<DataItemDetailDto>();
                foreach (var point in _pointData)
                {
                    realTimeDataDto.Details.Add(new DataItemDetailDto
                    {
                        Code = $"{_config.Connection.MachineName}.{_config.Connection.MachineCode}.{point.Name}",
                        Value = point.Value,
                        IsGood = true,
                    });
                }
                var su = await MqttController.Instance.ConnectAsync(_config.Connection.MqttIPAddress, _config.Connection.MqttPort);

                if (su)
                {
                    var msg = JsonConvert.SerializeObject(realTimeDataDto);
                    await MqttController.Instance.PublishMessageAsync($"realtime/{_config.Connection.MachineName}/{_config.Connection.MachineCode}", msg);
                }
            }
            catch(Exception ex)
            {
                LogController.Instance.Error($"PublishDataAsync发送mqtt数据失败: {ex.ToString()}");
            }

        }
    }
}