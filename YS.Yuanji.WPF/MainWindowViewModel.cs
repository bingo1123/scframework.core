using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using ClosedXML.Excel;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

namespace YS.Yuanji.WPF
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private string _searchText = "";
        private string _selectedMachineNumber = "";
        private bool _isConnected = false;
        private ObservableCollection<DataItem> _allData;
        private ObservableCollection<DataItem> _filteredDataView;
        private MQTTService _mqttService;
        private AppConfig _appConfig;
        public MainWindowViewModel()
        {
            _allData = new ObservableCollection<DataItem>();
            _filteredDataView = new ObservableCollection<DataItem>();

            // 加载配置
            LoadConfiguration();

            // 初始化示例数据
            InitializeData();

            // 初始化MQTT服务
            InitializeMQTTService();

            // 模拟连接状态（实际项目中应该根据真实连接状态更新）
            SimulateConnectionStatus();
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                FilterData();
            }
        }

        public string SelectedMachineNumber
        {
            get => _selectedMachineNumber;
            set
            {
                SetProperty(ref _selectedMachineNumber, value);
                IsConnected = false;
                // 当机台号改变时，可以刷新数据
                RefreshData();
            }
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        public ObservableCollection<string> MachineNumbers { get; private set; }

        public ObservableCollection<DataItem> FilteredDataView
        {
            get => _filteredDataView;
            set => SetProperty(ref _filteredDataView, value);
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchText = "";
        }

        [RelayCommand]
        private void ExportToExcel()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel文件|*.xlsx",
                    FileName = $"数据监控_{SelectedMachineNumber}_{System.DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportDataToExcel(saveFileDialog.FileName);
                    MessageBox.Show("Excel数据导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Excel导出失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void ExportToTxt()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "文本文件|*.txt",
                    FileName = $"数据监控_{SelectedMachineNumber}_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportDataToTxt(saveFileDialog.FileName);
                    MessageBox.Show("TXT数据导出成功！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"TXT导出失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConfiguration()
        {
            // 加载配置文件
            _appConfig = ConfigManager.LoadConfig();

            // 初始化机台号列表
            MachineNumbers = new ObservableCollection<string>(_appConfig.MachineNumbers);

            // 如果没有机台号，添加默认值
            if (MachineNumbers.Count == 0)
            {
                MachineNumbers.Add("MT-001");
                _appConfig.MachineNumbers.Add("MT-001");
                ConfigManager.SaveConfig(_appConfig);
            }

            // 设置默认选中的机台号
            if (string.IsNullOrEmpty(_selectedMachineNumber) && MachineNumbers.Count > 0)
            {
                _selectedMachineNumber = MachineNumbers[0];
            }
        }
        private void InitializeData()
        {
            //// 添加示例数据
            //_allData.Add(new DataItem { Id = 1, Name = "温度传感器1", Value = "25.6°C", Status = "正常", UpdateTime = System.DateTime.Now, Remark = "主厂房区域" });
            //_allData.Add(new DataItem { Id = 2, Name = "压力传感器2", Value = "1.2MPa", Status = "正常", UpdateTime = System.DateTime.Now.AddSeconds(-30), Remark = "液压系统" });
            //_allData.Add(new DataItem { Id = 3, Name = "液位传感器3", Value = "85%", Status = "警告", UpdateTime = System.DateTime.Now.AddMinutes(-1), Remark = "储罐区" });
            //_allData.Add(new DataItem { Id = 4, Name = "流量计4", Value = "120m³/h", Status = "正常", UpdateTime = System.DateTime.Now.AddMinutes(-2), Remark = "供水系统" });

            //// 初始化过滤视图
            //foreach (var item in _allData)
            //{
            //    _filteredDataView.Add(item);
            //}
        }

        private async void InitializeMQTTService()
        {
            // 初始化MQTT服务并订阅实时数据
            _mqttService = new MQTTService(_appConfig.Mqtt,new List<string> (MachineNumbers.Select(l=>$"realtime/{l}").ToList())); // 使用默认配置，可根据需要传递参数
            _mqttService.RealTimeDataReceived += OnRealTimeDataReceived;
            await _mqttService.StartAsync();
        }

        private void OnRealTimeDataReceived(object sender, RealTimeDataDto data)
        {
            // 在UI线程上更新数据
            Application.Current.Dispatcher.Invoke(() =>
            {
                UpdateDataFromMQTT(data);
            });
        }

        private void UpdateDataFromMQTT(RealTimeDataDto data)
        {
            if(data.MachineId == SelectedMachineNumber)
            {
                IsConnected = true;
            }
            else
            {
                return;
            }

            DateTimeOffset DateTimeUnix = DateTimeOffset.FromUnixTimeMilliseconds(data.Ts);
            // 根据MQTT接收到的实时数据更新监控界面
            foreach (var detail in data.Details)
            {
                var existingItem = _allData.FirstOrDefault(item => item.Name == detail.Code);
                if (existingItem != null)
                {
                    // 更新现有项
                    existingItem.Value = detail.Value?.ToString() ?? "";
                    existingItem.Status = detail.IsGood ? "正常" : "异常";
                    existingItem.UpdateTime = DateTimeUnix.DateTime.ToLocalTime();
                }
                else
                {
                    // 添加新项
                    var newItem = new DataItem
                    {
                        Id = _allData.Count + 1,
                        Name = detail.Code,
                        Value = detail.Value?.ToString() ?? "",
                        Status = detail.IsGood ? "正常" : "异常",
                        UpdateTime = DateTimeUnix.DateTime.ToLocalTime(),
                        Remark = "实时数据"
                    };
                    _allData.Add(newItem);
                }
            }

            // 重新应用过滤
            FilterData();
        }

        private void FilterData()
        {
            _filteredDataView.Clear();

            var filteredItems = _allData.Where(item =>
            item.Name.Contains(_selectedMachineNumber, System.StringComparison.OrdinalIgnoreCase) && (
                string.IsNullOrEmpty(_searchText) ||
                item.Name.Contains(_searchText, System.StringComparison.OrdinalIgnoreCase) ||
                item.Value.Contains(_searchText, System.StringComparison.OrdinalIgnoreCase) ||
                item.Status.Contains(_searchText, System.StringComparison.OrdinalIgnoreCase) ||
                item.Remark.Contains(_searchText, System.StringComparison.OrdinalIgnoreCase)));

            foreach (var item in filteredItems)
            {
                _filteredDataView.Add(item);
            }
        }

        private void RefreshData()
        {
            // 根据选择的机台号刷新数据
            // 这里是示例实现，实际项目中应该从对应的机台获取数据
            FilterData();
        }

        private void ExportDataToExcel(string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("数据监控");

                // 添加标题行
                worksheet.Cell(1, 1).Value = "机台号";
                worksheet.Cell(1, 2).Value = "ID";
                worksheet.Cell(1, 3).Value = "名称";
                worksheet.Cell(1, 4).Value = "数值";
                worksheet.Cell(1, 5).Value = "状态";
                worksheet.Cell(1, 6).Value = "更新时间";
                worksheet.Cell(1, 7).Value = "备注";

                // 设置标题样式
                var headerRange = worksheet.Range(1, 1, 1, 7);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                // 填充数据
                for (int i = 0; i < FilteredDataView.Count; i++)
                {
                    var item = FilteredDataView[i];
                    worksheet.Cell(i + 2, 1).Value = SelectedMachineNumber;
                    worksheet.Cell(i + 2, 2).Value = item.Id;
                    worksheet.Cell(i + 2, 3).Value = item.Name;
                    worksheet.Cell(i + 2, 4).Value = item.Value;
                    worksheet.Cell(i + 2, 5).Value = item.Status;
                    worksheet.Cell(i + 2, 6).Value = item.UpdateTime;
                    worksheet.Cell(i + 2, 7).Value = item.Remark;
                }

                // 自动调整列宽
                worksheet.Columns().AdjustToContents();

                // 保存文件
                workbook.SaveAs(filePath);
            }
        }

        private void ExportDataToTxt(string filePath)
        {
            var sb = new StringBuilder();

            // 添加标题行
            sb.AppendLine($"机台号\tID\t名称\t数值\t状态\t更新时间\t备注");

            // 填充数据
            foreach (var item in FilteredDataView)
            {
                sb.AppendLine($"{SelectedMachineNumber}\t{item.Id}\t{item.Name}\t{item.Value}\t{item.Status}\t{item.UpdateTime}\t{item.Remark}");
            }

            // 保存文件
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        }

        private async void SimulateConnectionStatus()
        {
            //// 模拟连接状态变化（实际项目中应该根据真实连接情况更新）
            //await System.Threading.Tasks.Task.Delay(2000);
            //IsConnected = true;
        }

        public void Dispose()
        {
            _mqttService?.Dispose();
        }
    }

    public partial class DataItem : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name = "";

        [ObservableProperty]
        private string _value = "";

        [ObservableProperty]
        private string _status = "";

        [ObservableProperty]
        private System.DateTime _updateTime;

        [ObservableProperty]
        private string _remark = "";
    }
}