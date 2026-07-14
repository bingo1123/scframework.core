using System.Collections.ObjectModel;
using System.Windows.Input;

public class LogController 
    {
        public delegate void OnLog(string msg);

        public ICommand ClearCommand { get; }

        public Func<string> LogInsertCallBack { get; }

        private static readonly object LOCKER_LOG = new object();

        public OnLog OnLogCallBack;

        private ObservableCollection<string> _logMessages;


        public void RegisterLogCallBackFunc(OnLog Func)
        {
            OnLogCallBack += Func;
        }

        public void MqttLog(string msg)
        {
            LogHelper.MqttLogger.Info(msg);
        }

        public void DebugfLog(string msg)
        {
            LogHelper.DebugLogger.Info(msg);
        }

        public void MqttError(string msg)
        {
            LogHelper.MqttLogger.Error(msg);
        }


        public void Log(string msg)
        {
            LogHelper.CommLogger.Info(msg);
            OnLogCallBack?.Invoke(msg);

        }

        public void Warning(string msg)
        {
            LogHelper.CommLogger.Error(msg);
            OnLogCallBack?.Invoke(msg);

        }

        public void Error(string msg)
        {
            LogHelper.CommLogger.Error(msg);
            OnLogCallBack?.Invoke(msg);

        }

        //public void ClearLog()
        //{
        //    lock (LOCKER_LOG)
        //        LogMessages = new ObservableCollection<string>();
        //}

        #region Singleton
        private static LogController _instance = null;
        private static readonly object LOCK = new object();

        private LogController()
        {

         // ClearCommand = new RelayCommand(ClearLog);
        }

        public static LogController Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LOCK)
                    {
                        _instance ??= new LogController();
                    }
                }
                return _instance;
            }
        }
        #endregion
    }

