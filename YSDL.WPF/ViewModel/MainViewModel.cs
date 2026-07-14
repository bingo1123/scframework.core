// YSDL.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// YSDL.WPF.ViewModel.MainViewModel
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel.__Internals;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using YSDL.src;
using YSDL.src.Common;
using YSDL.src.Driver.Huani.Controller;
using YSDL.src.Driver.Huani.Entity;
using YSDL.src.Driver.ZB48A;
using YSDL.src.HelperControllers;
using YSDL.src.MQTT;
using YSDL.src.MQTT.helper;
using YSDL.Start;
using YSDL.WPF.View;
using Path = System.IO.Path;

namespace YSDL.WPF.ViewModel
{

    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? ipAddress;

        [ObservableProperty]
        private int port;

        [ObservableProperty]
        private string symbAddress;

        [ObservableProperty]
        private int nodeAddress;

        [ObservableProperty]
        private string funcAddress;

        [ObservableProperty]
        private string brandName;

        [ObservableProperty]
        private string paramNumber;

        [ObservableProperty]
        private string writeValue;

        [ObservableProperty]
        private ObservableCollection<string> protocolType;

        [ObservableProperty]
        private ObservableCollection<string> messages;

        private HuaniProtocolTypeEnum _protocolType;

        private string selectItem;

        [ObservableProperty]
        private ObservableCollection<string> valueType;

        [ObservableProperty]
        private string? valueTypeItem;

        [ObservableProperty]
        private bool isconnect;

        [ObservableProperty]
        private bool isdisconnect = true;

        [ObservableProperty]
        private string? deviceCode;

        [ObservableProperty]
        private string? mqtt;

        [ObservableProperty]
        private string? bZ1;

        [ObservableProperty]
        private string? bZ2;

        [ObservableProperty]
        private string? bZ3;

        [ObservableProperty]
        private string? bZPort;

        private ApiClientConfig _apiClientConfig;

        private ApiClient _apiClient;

        private DataCollectConfig conifg;

        private string MachineName = string.Empty;

        private VisuCommunication communication;

        private ServiceMain main;

        private ObservableCollection<ParameterData> _parameterData;

        private string topic;

        private static readonly object LOCKER_LOG = new object();

        private string BaseParamFilePath = AppDomain.CurrentDomain.BaseDirectory;

        private Dictionary<HuaniProtocolTypeEnum, List<string>> paramsDic = new Dictionary<HuaniProtocolTypeEnum, List<string>>();

        private Dictionary<HuaniProtocolTypeEnum, List<string>> brandsDic = new Dictionary<HuaniProtocolTypeEnum, List<string>>();


        public string SelectItem
        {
            get
            {
                return selectItem;
            }
            set
            {
                SetProperty(ref selectItem, value, "SelectItem");
                if (Enum.TryParse<HuaniProtocolTypeEnum>(SelectItem, out var type))
                {
                    _protocolType = type;
                }
                if (_protocolType == HuaniProtocolTypeEnum.M5)
                {
                    MachineName = "JJ101";
                }
                else if (_protocolType == HuaniProtocolTypeEnum.P18)
                {
                    MachineName = "JJ103";
                }
                else if (_protocolType == HuaniProtocolTypeEnum.ZJ119)
                {
                    MachineName = "JJ203";
                }
                else if (_protocolType == HuaniProtocolTypeEnum.ZJ116A)
                {
                    MachineName = "T15";
                }
            }
        }


        public MainViewModel()
        {
            List<List<string>> tripleValue = new List<List<string>>();
            string response = @"1	0	0.0001041667	1	
2	1	0.0015625000	6	
3	0	0.0000000000	0	
4	1	0.0129861111	4	
5	1	0.0000000000	0	
6	1	0.0000000000	0	
7	1	0.0000000000	0	
8	1	0.0000000000	0 ";
            var lines = response
                    .Trim().Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var ite = new List<string>();
                var xx = (from s in line.Split('\t')
                               where !string.IsNullOrWhiteSpace(s)
                               select s.TrimEnd()).ToList();
                tripleValue.Add(xx);
            }

            protocolType = new ObservableCollection<string>();
            valueType = new ObservableCollection<string>();
            Messages = new ObservableCollection<string>();
            _parameterData = new ObservableCollection<ParameterData>();
            protocolType.Add(HuaniProtocolTypeEnum.P18.ToString());
            protocolType.Add(HuaniProtocolTypeEnum.ZJ118.ToString());
            protocolType.Add(HuaniProtocolTypeEnum.ZJ116A.ToString());
            string[] res = Enum.GetNames(typeof(PvalueFormatEnum));
            string[] array = res;
            foreach (string name in array)
            {
                valueType.Add(name);
            }
            LogController.Instance.RegisterLogCallBackFunc(LogDisPlay);
            DeviceCode = ConfigHelper.GetTerminalCodeFromConfig("TerminalCode");
            string url = ConfigHelper.GetTerminalCodeFromConfig("ServerRoot") + "/api/workshop/sysmanager/TerminalUpdate/GetConfig?terminalCode=" + ConfigHelper.GetTerminalCodeFromConfig("TerminalCode") + "&program=" + ConfigHelper.GetTerminalCodeFromConfig("Program");
            try
            {
                CacheContext.Instance.InitConfig(url);
            }
            catch (Exception ex)
            {
                LogController.Instance.Error("InitConfig获取配置参数出错:" + ex.ToString());
            }
            ipAddress = CacheContext.Instance.TerminalConfig.ContainsKey("MachineIPAddress") ? CacheContext.Instance.TerminalConfig["MachineIPAddress"]:"127.0.0.1";
            port = CacheContext.Instance.TerminalConfig.ContainsKey("MachinePort") ? int.Parse(CacheContext.Instance.TerminalConfig["MachinePort"]):1200;
            _apiClientConfig = new ApiClientConfig("192.168.100.10", "mro-api", "123456789");
            _apiClient = new ApiClient(_apiClientConfig);
            conifg = new DataCollectConfig();
            conifg.LoadDictionary(CacheContext.Instance.TerminalConfig);
            topic = "realtime/" + DeviceCode;
        }

        private void LogDisPlay(string msg)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                lock (LOCKER_LOG)
                {
                    Messages.Insert(0, "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + msg);
                }
            });
        }

        public void LoadParamsFile(HuaniProtocolTypeEnum type, bool isBrand = false)
        {
            string filePath = Path.Combine(BaseParamFilePath, $"{type}_PARAMS.LST");
            string paramTitle = ((type == HuaniProtocolTypeEnum.M5 || type == HuaniProtocolTypeEnum.BOBME) ? "[Parameter]" : "[MachineParameter]");
            if (isBrand)
            {
                paramTitle = ((type == HuaniProtocolTypeEnum.M5 || type == HuaniProtocolTypeEnum.BOBME) ? "[Brands]" : "[BrandParameter]");
            }
            string[] lines = File.ReadAllLines(filePath, Encoding.GetEncoding("GBK"));
            bool isInParameterSection = false;
            Dictionary<HuaniProtocolTypeEnum, List<string>> dic = (isBrand ? brandsDic : paramsDic);
            string[] array = lines;
            foreach (string line in array)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.Trim().Equals(paramTitle, StringComparison.OrdinalIgnoreCase))
                {
                    isInParameterSection = true;
                }
                else if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]") && isInParameterSection)
                {
                    isInParameterSection = false;
                }
                else if (!string.IsNullOrEmpty(trimmedLine) && isInParameterSection)
                {
                    if (!dic.ContainsKey(type))
                    {
                        dic[type] = new List<string> { line.Trim() };
                    }
                    else
                    {
                        dic[type].Add(line.Trim());
                    }
                }
            }
        }

        public void OutputFile(List<string> inputs, bool isBrand = false)
        {
            List<string> parametersSection = (isBrand ? brandsDic[_protocolType] : paramsDic[_protocolType]);
            string outputTitle = (isBrand ? $"{_protocolType}BrandResult.txt" : $"{_protocolType}Result.txt");
            string resultFilePath = Path.Combine(BaseParamFilePath, outputTitle);
            if (File.Exists(resultFilePath) && isBrand)
            {
                File.Delete(resultFilePath);
            }
            foreach (string input in inputs)
            {
                string resultLine = null;
                foreach (string line in parametersSection)
                {
                    string[] parts = line.Split(',');
                    if (parts[0].Equals(input, StringComparison.OrdinalIgnoreCase))
                    {
                        string name = ((_protocolType != HuaniProtocolTypeEnum.M5 && _protocolType != HuaniProtocolTypeEnum.BOBME) ? parts[2].Trim() : (isBrand ? parts[1].Trim() : parts[1].Split(':')[1].Trim()));
                        resultLine = input + ";" + name;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(resultLine))
                {
                    File.AppendAllText(resultFilePath, resultLine + Environment.NewLine);
                    Console.WriteLine("结果已追加到 result.txt: " + resultLine);
                }
                else
                {
                    Console.WriteLine("未找到匹配项: " + input);
                }
            }
        }

        private byte ConvertToByte(string inut)
        {
            if (string.IsNullOrEmpty(inut))
            {
                return 0;
            }
            return Convert.ToByte(inut, 16);
        }

        public int ConvertToInt(string inut)
        {
            return int.Parse(inut, NumberStyles.HexNumber);
        }

        [RelayCommand]
        private async Task ReadBrandParamAndOutPutName()
        {
            try
            {
                byte NODE = Convert.ToByte(NodeAddress);
                byte FUNC = ConvertToByte(FuncAddress);
                byte[] command = HuaniCommand.PackCommand(NODE, FUNC, VisuCommandNameEnum.ReadBrandParameter, _protocolType, "", 0, "", ConvertToInt(ParamNumber)).Item1;
                List<ParameterData> inputs = HuaniCommand.ParseBrandPvalue(await communication.SendCommandAsync(command, "ReadBrandParamAndOutPutName"), _protocolType)?.Select((Pvalue x) => new ParameterData
                {
                    Code = x.ParaNumerName,
                    Value = x.ParameterValue
                }).ToList();
                DataViewWindow dataWindow = new DataViewWindow(new ObservableCollection<ParameterData>(inputs));
                dataWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dataWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error("ReadBrandParamAndOutPutName error:" + ex2.Message + ",StackTrace:" + ex2.StackTrace);
                MessageBox.Show("读取品牌参数失败，请检查连接或参数设置。错误信息：" + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        [RelayCommand]
        private async Task ReadMachineParamAndOutPutName()
        {
            try
            {
                byte NODE = Convert.ToByte(NodeAddress);
                byte FUNC = ConvertToByte(FuncAddress);
                byte[] command = HuaniCommand.PackCommand(NODE, FUNC, VisuCommandNameEnum.ReadMachineParameter, _protocolType, "", 0, "", ConvertToInt(ParamNumber)).Item1;
                List<ParameterData> inputs = HuaniCommand.PasrePvalue(await communication.SendCommandAsync(command, "ReadMachineParamAndOutPutName"), _protocolType)?.Select((Pvalue x) => new ParameterData
                {
                    Code = x.ParaNumerName,
                    Value = x.ParameterValue
                }).ToList();
                DataViewWindow dataWindow = new DataViewWindow(new ObservableCollection<ParameterData>(inputs));
                dataWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                dataWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error("ReadMachineParamAndOutPutName error:" + ex2.Message + ",StackTrace:" + ex2.StackTrace);
                MessageBox.Show("读取机器参数失败，请检查连接或参数设置。错误信息：" + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        [RelayCommand]
        private async Task M5ShiftDataTestAsync(string flag)
        {
            byte NODE = Convert.ToByte(NodeAddress);
            byte FUNC = ConvertToByte(FuncAddress);
            byte[] command = HuaniCommand.PackCommandBySymbArray(NODE, FUNC, new string[1] { SymbAddress }, _protocolType);
            byte[] response = await communication.SendCommandAsync(command, "M5ShiftDataTestAsync");
            switch (flag)
            {
                case "U":
                    {
                        uint ttt = HuaniCommand.ParseM5ShiftDataUnit(response);
                        LogController.Instance.Log($"M5ShiftDataTest {SymbAddress} ,Send:{BitConverter.ToString(command)},Response:{BitConverter.ToString(response)},uintValue:{ttt}");
                        break;
                    }
                case "F":
                    {
                        float aaaa = HuaniCommand.ParseM5ShiftDataFloat(response);
                        LogController.Instance.Log($"M5ShiftDataTest {SymbAddress} ,Send:{BitConverter.ToString(command)},Response:{BitConverter.ToString(response)},Float:{aaaa}");
                        break;
                    }
                case "S":
                    {
                        short ss = HuaniCommand.ParseM5ShiftDataShort(response);
                        LogController.Instance.Log($"M5ShiftDataTest {SymbAddress} ,Send:{BitConverter.ToString(command)},Response:{BitConverter.ToString(response)},Float:{ss}");
                        break;
                    }
            }
        }

        [RelayCommand]
        private void ClearLogMessage()
        {
            lock (LOCKER_LOG)
            {
                Messages = new ObservableCollection<string>();
            }
        }

        [RelayCommand]
        private async Task ReadData(string prop)
        {
            try
            {
                if (!communication.IsConnected())
                {
                    return;
                }
                switch (prop)
                {
                    case "readData":
                        {
                            byte FunctionAddress = ConvertToByte(FuncAddress);
                            byte NODE = Convert.ToByte(NodeAddress);
                            (byte[], string[] symbnums) res = HuaniCommand.PackCommand(NODE, FunctionAddress, VisuCommandNameEnum.ReadData, _protocolType, SymbAddress, 0);
                            byte[] command = res.Item1;
                            byte[] response = await communication.SendCommandAsync(command, "ReadData");
                            LogController.Instance.Log("ReadData,symbAdr:" + SymbAddress + ",Send:" + BitConverter.ToString(command));
                            LogController.Instance.Log("ReadData,symbAdr:" + SymbAddress + ",recv:" + BitConverter.ToString(response));
                            object result = HuaniCommand.ParseResponse(response, _protocolType, res.symbnums);
                            Array results = default(Array);
                            int num;
                            if (result != null)
                            {
                                results = result as Array;
                                num = ((results != null) ? 1 : 0);
                            }
                            else
                            {
                                num = 0;
                            }
                            if (num == 0)
                            {
                                break;
                            }
                            ProductionCounter counter = default(ProductionCounter);
                            CNTX CNT1 = default(CNTX);
                            PROC proc = default(PROC);
                            OMSC omsc = default(OMSC);
                            ACFA acfa = default(ACFA);
                            ACDI acdi = default(ACDI);
                            ACDI_P18 acdi_p18 = default(ACDI_P18);
                            SLVL slvl = default(SLVL);
                            SANA sana = default(SANA);
                            SHIS shis = default(SHIS);
                            SHFT shft = default(SHFT);
                            BRKT brkt = default(BRKT);
                            STAT stat = default(STAT);
                            SP02 p02 = default(SP02);
                            WASP wASP = default(WASP);
                            SHFD_P18 sHFD = default(SHFD_P18);
                            foreach (object te in results)
                            {
                                int num2;
                                if (te != null)
                                {
                                    if (te is ProductionCounter)
                                    {
                                        counter = (ProductionCounter)te;
                                        num2 = 1;
                                    }
                                    else
                                    {
                                        num2 = 0;
                                    }
                                }
                                else
                                {
                                    num2 = 0;
                                }
                                if (num2 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(counter, isShowFildName: true) ?? "");
                                }
                                int num3;
                                if (te != null)
                                {
                                    if (te is CNTX)
                                    {
                                        CNT1 = (CNTX)te;
                                        num3 = 1;
                                    }
                                    else
                                    {
                                        num3 = 0;
                                    }
                                }
                                else
                                {
                                    num3 = 0;
                                }
                                if (num3 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(CNT1) ?? "");
                                }
                                int num4;
                                if (te != null)
                                {
                                    if (te is PROC)
                                    {
                                        proc = (PROC)te;
                                        num4 = 1;
                                    }
                                    else
                                    {
                                        num4 = 0;
                                    }
                                }
                                else
                                {
                                    num4 = 0;
                                }
                                if (num4 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(proc) ?? "");
                                }
                                int num5;
                                if (te != null)
                                {
                                    if (te is OMSC)
                                    {
                                        omsc = (OMSC)te;
                                        num5 = 1;
                                    }
                                    else
                                    {
                                        num5 = 0;
                                    }
                                }
                                else
                                {
                                    num5 = 0;
                                }
                                if (num5 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(omsc) ?? "");
                                }
                                int num6;
                                if (te != null)
                                {
                                    if (te is ACFA)
                                    {
                                        acfa = (ACFA)te;
                                        num6 = 1;
                                    }
                                    else
                                    {
                                        num6 = 0;
                                    }
                                }
                                else
                                {
                                    num6 = 0;
                                }
                                if (num6 != 0)
                                {
                                    LogController.Instance.Log(JsonConvert.SerializeObject(acfa, Formatting.Indented) ?? "");
                                }
                                int num7;
                                if (te != null)
                                {
                                    if (te is ACDI)
                                    {
                                        acdi = (ACDI)te;
                                        num7 = 1;
                                    }
                                    else
                                    {
                                        num7 = 0;
                                    }
                                }
                                else
                                {
                                    num7 = 0;
                                }
                                if (num7 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(acdi) ?? "");
                                }
                                int num8;
                                if (te != null)
                                {
                                    if (te is ACDI_P18)
                                    {
                                        acdi_p18 = (ACDI_P18)te;
                                        num8 = 1;
                                    }
                                    else
                                    {
                                        num8 = 0;
                                    }
                                }
                                else
                                {
                                    num8 = 0;
                                }
                                if (num8 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(acdi_p18) ?? "");
                                }
                                int num9;
                                if (te != null)
                                {
                                    if (te is SLVL)
                                    {
                                        slvl = (SLVL)te;
                                        num9 = 1;
                                    }
                                    else
                                    {
                                        num9 = 0;
                                    }
                                }
                                else
                                {
                                    num9 = 0;
                                }
                                if (num9 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(slvl) ?? "");
                                }
                                int num10;
                                if (te != null)
                                {
                                    if (te is SANA)
                                    {
                                        sana = (SANA)te;
                                        num10 = 1;
                                    }
                                    else
                                    {
                                        num10 = 0;
                                    }
                                }
                                else
                                {
                                    num10 = 0;
                                }
                                if (num10 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(sana) ?? "");
                                }
                                int num11;
                                if (te != null)
                                {
                                    if (te is SHIS)
                                    {
                                        shis = (SHIS)te;
                                        num11 = 1;
                                    }
                                    else
                                    {
                                        num11 = 0;
                                    }
                                }
                                else
                                {
                                    num11 = 0;
                                }
                                if (num11 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(shis) ?? "");
                                }
                                int num12;
                                if (te != null)
                                {
                                    if (te is SHFT)
                                    {
                                        shft = (SHFT)te;
                                        num12 = 1;
                                    }
                                    else
                                    {
                                        num12 = 0;
                                    }
                                }
                                else
                                {
                                    num12 = 0;
                                }
                                if (num12 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(shft) ?? "");
                                }
                                int num13;
                                if (te != null)
                                {
                                    if (te is BRKT)
                                    {
                                        brkt = (BRKT)te;
                                        num13 = 1;
                                    }
                                    else
                                    {
                                        num13 = 0;
                                    }
                                }
                                else
                                {
                                    num13 = 0;
                                }
                                if (num13 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(brkt) ?? "");
                                }
                                int num14;
                                if (te != null)
                                {
                                    if (te is STAT)
                                    {
                                        stat = (STAT)te;
                                        num14 = 1;
                                    }
                                    else
                                    {
                                        num14 = 0;
                                    }
                                }
                                else
                                {
                                    num14 = 0;
                                }
                                if (num14 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(stat) ?? "");
                                }
                                int num15;
                                if (te != null)
                                {
                                    if (te is SP02)
                                    {
                                        p02 = (SP02)te;
                                        num15 = 1;
                                    }
                                    else
                                    {
                                        num15 = 0;
                                    }
                                }
                                else
                                {
                                    num15 = 0;
                                }
                                if (num15 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(p02) ?? "");
                                }
                                int num16;
                                if (te != null)
                                {
                                    if (te is WASP)
                                    {
                                        wASP = (WASP)te;
                                        num16 = 1;
                                    }
                                    else
                                    {
                                        num16 = 0;
                                    }
                                }
                                else
                                {
                                    num16 = 0;
                                }
                                if (num16 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(wASP) ?? "");
                                }
                                int num17;
                                if (te != null)
                                {
                                    if (te is SHFD_P18)
                                    {
                                        sHFD = (SHFD_P18)te;
                                        num17 = 1;
                                    }
                                    else
                                    {
                                        num17 = 0;
                                    }
                                }
                                else
                                {
                                    num17 = 0;
                                }
                                if (num17 != 0)
                                {
                                    LogController.Instance.Log(VisuHelper.ToJson(sHFD, isShowFildName: true) ?? "");
                                }
                                acfa = default(ACFA);
                                slvl = default(SLVL);
                                sana = default(SANA);
                                shis = default(SHIS);
                                shft = default(SHFT);
                                brkt = default(BRKT);
                                p02 = default(SP02);
                                sHFD = default(SHFD_P18);
                            }
                            break;
                        }
                    case "machine":
                        {
                            byte NODE3 = Convert.ToByte(NodeAddress);
                            byte FUNC2 = ConvertToByte(FuncAddress);
                            byte[] command3 = HuaniCommand.PackCommand(NODE3, FUNC2, VisuCommandNameEnum.ReadMachineParameter, _protocolType, "", 0).Item1;
                            byte[] response3 = await communication.SendCommandAsync(command3, "ReadData");
                            List<Pvalue> res3 = HuaniCommand.PasrePvalue(response3, _protocolType);
                            LogController.Instance.Log($"read Machine,node:{NODE3},Func:{FUNC2},res:{JsonConvert.SerializeObject(res3)},response:{BitConverter.ToString(response3)},send:{BitConverter.ToString(command3)}");
                            break;
                        }
                    case "brand":
                        {
                            byte NODE2 = Convert.ToByte(NodeAddress);
                            byte FUNC = ConvertToByte(FuncAddress);
                            byte[] command2 = HuaniCommand.PackCommand(NODE2, FUNC, VisuCommandNameEnum.ReadBrandParameter, _protocolType, "", 0, "", ConvertToInt(ParamNumber)).Item1;
                            byte[] response2 = await communication.SendCommandAsync(command2, "ReadData");
                            List<Pvalue> res2 = HuaniCommand.ParseBrandPvalue(response2, _protocolType);
                            LogController.Instance.Log($"read brand,node:{NODE2},Func:{FUNC},res:{JsonConvert.SerializeObject(res2)},response:{BitConverter.ToString(response2)},send:{BitConverter.ToString(command2)}");
                            break;
                        }
                    case "totalShift":
                        {
                            byte[] totalShiftData3 = HuaniCommand.PackCommandBySymbArray(1, 2, M5Const.TotalShiftDataArray, _protocolType);
                            byte[] totalShiftResponse3 = await communication.SendCommandAsync(totalShiftData3, "ReadData");
                            if (HuaniCommand.CheckIfResponsIsValid(totalShiftResponse3, "TotalShiftDataArray", "ReadData"))
                            {
                                LogController.Instance.Log("M5 read TotalShiftData success , res:" + BitConverter.ToString(totalShiftResponse3));
                                try
                                {
                                    M5ShiftDataParser.M5ParseTotalShiftData(totalShiftResponse3);
                                }
                                catch (Exception ex)
                                {
                                    Exception ex2 = ex;
                                    LogController.Instance.Error($"M5 read TotalShiftData M5ShiftDataParser , exception:{ex2},{ex2?.StackTrace}");
                                }
                            }
                            else
                            {
                                LogController.Instance.Log("M5 read TotalShiftData fail ");
                            }
                            break;
                        }
                    case "bobme12":
                        {
                            byte[] totalShiftData2 = HuaniCommand.PackCommandBySymbArray(1, 2, BoBMeConst.BobProductData, _protocolType);
                            byte[] totalShiftResponse2 = await communication.SendCommandAsync(totalShiftData2, "ReadData");
                            if (HuaniCommand.CheckIfResponsIsValid(totalShiftResponse2, "TotalShiftDataArray", "ReadData"))
                            {
                                LogController.Instance.Log("BOBME read BobProductData success ,send:" + BitConverter.ToString(totalShiftData2) + ", res:" + BitConverter.ToString(totalShiftResponse2));
                                BOBMeShiftData shift = BOBMeDataParser.BOBProductDataParse(totalShiftResponse2);
                                LogController.Instance.Log("BobProductData parse:" + JsonConvert.SerializeObject(shift));
                            }
                            break;
                        }
                    case "bobme21":
                        {
                            byte[] totalShiftData = HuaniCommand.PackCommandBySymbArray(2, 1, BoBMeConst.BobHMIData, _protocolType);
                            byte[] totalShiftResponse = await communication.SendCommandAsync(totalShiftData, "ReadData");
                            if (HuaniCommand.CheckIfResponsIsValid(totalShiftResponse, "TotalShiftDataArray", "ReadData"))
                            {
                                LogController.Instance.Log("BOBME read BobHMIData success  ,send:" + BitConverter.ToString(totalShiftData) + ",res:" + BitConverter.ToString(totalShiftResponse));
                                BOBMeCurrentData currentData = BOBMeDataParser.BOBCurrentDataParse(totalShiftResponse);
                                LogController.Instance.Log("BobHMIData parse:" + JsonConvert.SerializeObject(currentData));
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Exception ex3 = ex;
                LogController.Instance.Error("ReadData error:" + ex3.Message + ",StackTrace:" + ex3.StackTrace);
                MessageBox.Show("读取数据失败，请检查连接或参数设置。错误信息：" + ex3.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        [RelayCommand]
        private async Task WriteData(string prop)
        {
            try
            {
                if (string.IsNullOrEmpty(writeValue?.ToString()) || string.IsNullOrEmpty(FuncAddress) || string.IsNullOrEmpty(valueTypeItem))
                {
                    MessageBox.Show("请先设置FuncAddress,valueTypeItem,writeValue", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (MessageBox.Show("请确保机器在未生产的状态下进行测试写入，如在生产中写入可能会导致严重后果！", "提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) != MessageBoxResult.No && communication.IsConnected())
                {
                    byte FUNC = ConvertToByte(FuncAddress);
                    byte NUM = ConvertToByte(ParamNumber);
                    Pvalue pvalue = new Pvalue
                    {
                        nodeAddress = NUM,
                        functionNumber = FUNC,
                        ParameterValue = writeValue,
                        format = Convert.ToByte(Enum.Parse(typeof(PvalueFormatEnum), valueTypeItem))
                    };
                    byte[] command = new byte[0];
                    if (prop == "machine")
                    {
                        command = HuaniCommand.PackCommand(0, FUNC, VisuCommandNameEnum.WriteMachineParameter, _protocolType, "", 0, "", 0, new List<Pvalue> { pvalue }).Item1;
                    }
                    else if (prop == "brand")
                    {
                        command = HuaniCommand.PackCommand(0, FUNC, VisuCommandNameEnum.WriteBrandParameter, _protocolType, "", 0, BrandName, 0, new List<Pvalue> { pvalue }).Item1;
                    }
                    byte[] array = command;
                    if (array != null && array.Length < 4)
                    {
                        MessageBox.Show("写入数据失败，命令格式不正确，请检查参数设置。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                    else if (!HuaniCommand.CheckIfResponsIsValid(await communication.SendCommandAsync(command, "WriteData"), "SHFD", "WriteData"))
                    {
                        MessageBox.Show("写入数据失败。", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                    else
                    {
                        MessageBox.Show("写入数据成功。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error("WriteData error:" + ex2.Message + ",StackTrace:" + ex2.StackTrace);
                MessageBox.Show("写入数据失败，请检查连接或参数设置。错误信息：" + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        [RelayCommand]
        private async Task Connect(string prop)
        {
            if (!bool.TryParse(prop, out var res))
            {
                return;
            }
            if (res)
            {
                Console.WriteLine("connect");
                if (communication != null)
                {
                    await communication.DisconnectAsync();
                    Isdisconnect = true;
                    Isconnect = false;
                }
                communication = new VisuCommunication(IpAddress, Port, DLMachineTypeEnum.Huani);
                if (await communication.ConnectAsync())
                {
                    Isconnect = true;
                    Isdisconnect = false;
                    LogController.Instance.Log($"{IpAddress},port:{port},{selectItem}，连接成功");
                }
            }
            else
            {
                await communication.DisconnectAsync();
                Isdisconnect = true;
                Isconnect = false;
            }
        }

        [RelayCommand]
        private async void ReadZB48AData(string prop)
        {
            switch (prop)
            {
                case "MachineStatus":
                    {
                        string performanceRes12 = await _apiClient.GetAsync("/runningStatus/getCurrentMachineStatus", new Dictionary<string, string>(), null, "ReadZB48AData");
                        MachineStatusResponse res12 = JsonConvert.DeserializeObject<MachineStatusResponse>(performanceRes12);
                        LogController.Instance.Log("ZB48A Test /runningStatus/getCurrentMachineStatus ,response :" + performanceRes12);
                        LogController.Instance.Log("设备单机状态接口 : " + JsonConvert.SerializeObject(res12));
                        break;
                    }
                case "MachinePerformance":
                    {
                        string performanceRes7 = await _apiClient.GetAsync("/runningStatus/getCurrentShiftRunningStatus", new Dictionary<string, string>(), null, "ReadZB48AData");
                        MachinePerformanceResponse res7 = JsonConvert.DeserializeObject<MachinePerformanceResponse>(performanceRes7);
                        LogController.Instance.Log("ZB48A Test /runningStatus/getCurrentShiftRunningStatus ,response :" + performanceRes7);
                        LogController.Instance.Log("机器性能接口 : " + JsonConvert.SerializeObject(res7));
                        break;
                    }
                case "DeviceFault":
                    {
                        string performanceRes6 = await _apiClient.GetAsync("/api/haltEquipment", new Dictionary<string, string>(), null, "ReadZB48AData");
                        DeviceFaultResponse res6 = JsonConvert.DeserializeObject<DeviceFaultResponse>(performanceRes6);
                        LogController.Instance.Log("ZB48A Test /api/haltEquipment ,response :" + performanceRes6);
                        LogController.Instance.Log("设备停机信息接口 : " + JsonConvert.SerializeObject(res6));
                        break;
                    }
                case "DeviceParameter":
                    {
                        string performanceRes9 = await _apiClient.GetAsync("/api/parameterEquipment", new Dictionary<string, string>(), null, "ReadZB48AData");
                        DeviceParameterResponse res9 = JsonConvert.DeserializeObject<DeviceParameterResponse>(performanceRes9);
                        LogController.Instance.Log("ZB48A Test /api/parameterEquipment ,response :" + performanceRes9);
                        LogController.Instance.Log("设备参数当前值及单位接口 : " + JsonConvert.SerializeObject(res9));
                        break;
                    }
                case "HaltAnalysis":
                    {
                        string performanceRes2 = await _apiClient.GetAsync("/api/haltAnalysis", new Dictionary<string, string>(), null, "ReadZB48AData");
                        HaltAnalysisResponse res2 = JsonConvert.DeserializeObject<HaltAnalysisResponse>(performanceRes2);
                        LogController.Instance.Log("ZB48A Test /api/haltAnalysis ,response :" + performanceRes2);
                        LogController.Instance.Log("设备班次停机分析接口 : " + JsonConvert.SerializeObject(res2));
                        break;
                    }
                case "HourConsumption":
                    {
                        string performanceRes11 = await _apiClient.GetAsync("/sampleElectric/hourConsumption", new Dictionary<string, string> { { "equipmentCode", "nqe718p2js" } }, 8189, "ReadZB48AData");
                        EnergyConsumptionResponse res11 = null;
                        try
                        {
                            res11 = JsonConvert.DeserializeObject<EnergyConsumptionResponse>(performanceRes11);
                        }
                        catch (Exception ex)
                        {
                            Exception ex4 = ex;
                            LogController.Instance.Log($"ZB48A Test {"/sampleElectric/hourConsumption"} ,response :{performanceRes11},exception:{ex4}");
                        }
                        LogController.Instance.Log("ZB48A Test /sampleElectric/hourConsumption ,response :" + performanceRes11);
                        LogController.Instance.Log("单机每小时能耗使用情况接口 : " + JsonConvert.SerializeObject(res11));
                        break;
                    }
                case "MalfunctionData":
                    {
                        string performanceRes10 = await _apiClient.GetAsync("/api/malfunctionEquipment", new Dictionary<string, string>(), null, "ReadZB48AData");
                        MalfunctionDataResponse res10 = JsonConvert.DeserializeObject<MalfunctionDataResponse>(performanceRes10);
                        LogController.Instance.Log("ZB48A Test /api/malfunctionEquipment ,response :" + performanceRes10);
                        LogController.Instance.Log("设备报警信息（不导致停机）接口  : " + JsonConvert.SerializeObject(res10));
                        break;
                    }
                case "ParameterModification":
                    {
                        Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                {
                    "pageNum",
                    0.ToString()
                },
                {
                    "pageSize",
                    10.ToString()
                },
                { "orderBy", "id" },
                { "direction", "DESC" }
            };
                        string performanceRes8 = await _apiClient.PostAsync("/api/parameterModifyRecord", parameters);
                        LogController.Instance.Log("参数修改记录接口  Response:" + performanceRes8);
                        ParameterModificationResponse res8 = JsonConvert.DeserializeObject<ParameterModificationResponse>(performanceRes8);
                        LogController.Instance.Log("ZB48A Test /api/parameterModifyRecord ,response :" + performanceRes8);
                        LogController.Instance.Log("参数修改记录接口  : " + JsonConvert.SerializeObject(res8));
                        break;
                    }
                case "ProcessConsumption":
                    {
                        string performanceRes5 = await _apiClient.GetAsync("/api/senior/shiftCounts", new Dictionary<string, string>(), null, "ReadZB48AData");
                        ProcessConsumptionResponse res5 = JsonConvert.DeserializeObject<ProcessConsumptionResponse>(performanceRes5);
                        LogController.Instance.Log("ZB48A Test /api/senior/shiftCounts ,response :" + performanceRes5);
                        LogController.Instance.Log("全流程消耗接口 : " + JsonConvert.SerializeObject(res5));
                        int num;
                        if (res5 != null && res5?.Data != null)
                        {
                            num = ((res5 != null && res5.Data.Count > 0) ? 1 : 0);
                        }
                        else
                        {
                            num = 0;
                        }
                        if (num != 0)
                        {
                            if (res5.Data.ContainsKey("xx_label_paper_total_expend"))
                            {
                                float? count = res5.Data["xx_label_paper_total_expend"].Count;
                                LogController.Instance.Log($"{"xx_label_paper_total_expend"}:{"小盒商标纸总耗"} count:{count}");
                            }
                            if (res5.Data.ContainsKey("ct_label_paper_total_expend"))
                            {
                                float? count2 = res5.Data["ct_label_paper_total_expend"].Count;
                                LogController.Instance.Log($"{"ct_label_paper_total_expend"}:{"条盒商标纸总耗"} count:{count2}");
                            }
                        }
                        break;
                    }
                case "ProductionData":
                    {
                        string performanceRes4 = await _apiClient.GetAsync("/api/standard/shiftCounts", new Dictionary<string, string>(), null, "ReadZB48AData");
                        try
                        {
                            ProductionDataResponse res4 = JsonConvert.DeserializeObject<ProductionDataResponse>(performanceRes4);
                            LogController.Instance.Log("ZB48A Test /api/standard/shiftCounts ,response :" + performanceRes4);
                            LogController.Instance.Log("生产数据接口 : " + JsonConvert.SerializeObject(res4));
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    }
                case "RejectData":
                    {
                        string performanceRes3 = await _apiClient.GetAsync("/api/rejectShiftCounts", new Dictionary<string, string>(), null, "ReadZB48AData");
                        RejectDataResponse res3 = JsonConvert.DeserializeObject<RejectDataResponse>(performanceRes3);
                        LogController.Instance.Log("ZB48A Test /api/rejectShiftCounts ,response :" + performanceRes3);
                        LogController.Instance.Log("剔除数据接口 : " + JsonConvert.SerializeObject(res3));
                        break;
                    }
                case "SpeedData":
                    {
                        string str = "{\"sendTime\":1736750109000,\"speedMap\":{\"750\":0,\"700\":0,\"778\":0},\"equipmentCode\":\"nqe718p2js\"}";
                        JsonConvert.DeserializeObject<SpeedDataResponseAn>(str);
                        string performanceRes = await _apiClient.GetAsync("/state/speed", new Dictionary<string, string>(), 8189, "ReadZB48AData");
                        SpeedDataResponseAn res = null;
                        try
                        {
                            res = JsonConvert.DeserializeObject<SpeedDataResponseAn>(performanceRes);
                        }
                        catch (Exception ex)
                        {
                            Exception ex2 = ex;
                            LogController.Instance.Error($"ZB48A Test {"/state/speed"} ,response :{performanceRes},exception:{ex2}");
                        }
                        LogController.Instance.Log("ZB48A Test /state/speed ,response :" + performanceRes);
                        LogController.Instance.Log("实际车速接口 : " + JsonConvert.SerializeObject(res));
                        break;
                    }
            }
        }

        [RelayCommand]
        private void CopyMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Clipboard.SetText(message);
            }
        }

        [RelayCommand]
        private async Task Caiji()
        {
            ConfigHelper.SetTerminalCodeInConfig("TerminalCode", DeviceCode);
            main?.Stop();
            main = new ServiceMain();
            main.Start();
            LogController.Instance.Log("采集服务已启动，请在任务栏中查看服务状态。");
        }

        [RelayCommand]
        private async Task Monitor(string para)
        {
            try
            {
                bool tt = false;
                DataViewWindow show = new DataViewWindow(_parameterData);
                show.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                MqttController mqtt = new MqttController(isautoconnect: false);
                try
                {
                    mqtt.LoadConfig(conifg.MqttUserName, conifg.MqttUserPassword);
                    if (para == "test")
                    {
                        Task.Run(async delegate
                        {
                            while (!tt)
                            {
                                await Task.Delay(1000);
                                List<DataItemDetailDto> dd = (from x in Enumerable.Range(1, 100)
                                                              select new DataItemDetailDto
                                                              {
                                                                  Code = "Code" + x,
                                                                  Value = new Random().Next(10000),
                                                                  IsGood = true
                                                              }).ToList();
                                RealTimeDataDto dto = new RealTimeDataDto
                                {
                                    MachineId = "TEST",
                                    Ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                                    Details = dd
                                };
                                if (await mqtt.ConnectAsync(conifg.MqttIPAddress.ToString(), conifg.MqttPort, "Monitor"))
                                {
                                    await mqtt.PublishMessageAsync(topic, JsonConvert.SerializeObject(dto), isSaveMessage: true, "Monitor");
                                }
                            }
                        });
                    }
                    Task.Run(async delegate
                    {
                        await mqtt.ConnectAsync(conifg.MqttIPAddress.ToString(), conifg.MqttPort, "Monitor");
                        await mqtt.Subscribe(topic);
                        Action<string, string> ShowData = delegate (string topic, string msg)
                        {
                            try
                            {
                                string value = msg.ToString();
                                if (!string.IsNullOrEmpty(value))
                                {
                                    RealTimeDataDto realTimeDataDto = JsonConvert.DeserializeObject<RealTimeDataDto>(value);
                                    if (realTimeDataDto != null && realTimeDataDto?.Details != null && (realTimeDataDto == null || realTimeDataDto.Details?.Count != 0))
                                    {
                                        DateTimeOffset DateTimeUnix = DateTimeOffset.FromUnixTimeMilliseconds(realTimeDataDto.Ts);
                                        List<ParameterData> newData = realTimeDataDto.Details.Select((DataItemDetailDto x) => new ParameterData
                                        {
                                            Code = x.Code,
                                            Value = x.Value,
                                            IsGood = x.IsGood,
                                            Desc = DateTimeUnix.DateTime.ToLocalTime().ToString()
                                        }).ToList();
                                        show.UpdateData(newData);
                                    }
                                }
                            }
                            catch (Exception ex3)
                            {
                                LogController.Instance.Error("Monitor error:" + ex3.Message + ",StackTrace:" + ex3.StackTrace);
                            }
                        };
                        mqtt.RecvieData(ShowData);
                    });
                    show.ShowDialog();
                    tt = true;
                }
                finally
                {
                    if (mqtt != null)
                    {
                        ((IDisposable)mqtt).Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                LogController.Instance.Error("Monitor error:" + ex2.Message + ",StackTrace:" + ex2.StackTrace);
                MessageBox.Show("数据监控异常: " + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        [RelayCommand]
        private async Task RunStartBat()
        {
            try
            {
                string batPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "start.bat");
                if (!File.Exists(batPath))
                {
                    MessageBox.Show("未找到 start.bat 文件。", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = batPath,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                };
                Process.Start(psi);
                main?.Stop();
                MessageBox.Show("已启动 start.bat。", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                MessageBox.Show("启动 start.bat 失败：" + ex2.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }
    }
}
