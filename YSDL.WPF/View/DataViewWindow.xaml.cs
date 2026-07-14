using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YSDL.WPF.ViewModel;
using static MaterialDesignThemes.Wpf.Theme;

namespace YSDL.WPF.View
{
    public partial class DataViewWindow : Window
    {
        private  ObservableCollection<ParameterData> _data;

        public DataViewWindow(ObservableCollection<ParameterData> data)
        {
            InitializeComponent();
            _data = data;
            dataGrid.ItemsSource = data;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "文本文件|*.txt|所有文件|*.*",
                DefaultExt = ".txt",
                FileName = $"参数数据_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    // 构建文本内容
                    var sb = new StringBuilder();
                    sb.AppendLine("参数值\t参数名称");
                    foreach (var item in _data)
                    {
                        sb.AppendLine($"{item.Code}\t{item.Value}");
                    }

                    // 写入文件
                    File.WriteAllText(saveDialog.FileName, sb.ToString());
                    MessageBox.Show($"成功导出 {_data.Count} 条数据", "导出成功",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"导出失败: {ex.Message}", "错误",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // 2. DataViewWindow 增加公开方法用于外部更新数据源：
        public void UpdateData(List<ParameterData> newData)
        {
            // 检查是否在主线程
            if (!Dispatcher.CheckAccess())
            {
                // 如果不是主线程，则通过Dispatcher调用
                Dispatcher.Invoke(() => UpdateData(newData));
                return;
            }

            foreach (var newItem in newData)
            {
                var existing = _data.FirstOrDefault(x => x.Code == newItem.Code);
                if (existing != null)
                {
                    existing.Value = newItem.Value;
                    existing.IsGood = newItem.IsGood;
                    existing.Desc = newItem.Desc;
                    // 如果需要通知UI属性变化，DataItemDetailDto应实现INotifyPropertyChanged
                }
                else
                {
                    _data.Add(new ParameterData
                    {
                        Code = newItem.Code,
                        Value = newItem.Value,
                        IsGood = newItem.IsGood,
                        Desc = newItem.Desc

                    });
                }
            }
    }
    }

    public class ParameterData : INotifyPropertyChanged
    {
        private string _code;
        private object _value;
        private bool _isGood;

        public string Code
        {
            get => _code;
            set { _code = value; OnPropertyChanged(nameof(Code)); }
        }
        public object Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(nameof(Value)); }
        }
        public bool IsGood
        {
            get => _isGood;
            set { _isGood = value; OnPropertyChanged(nameof(IsGood)); }
        }

        private string _desc;
        public string Desc
        {
            get => _desc;
            set { _desc = value; OnPropertyChanged(nameof(Desc)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
