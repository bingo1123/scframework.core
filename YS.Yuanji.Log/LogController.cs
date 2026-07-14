
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace YS.Yuanji.Log
{
    public class LogController 
    {
        public delegate void OnLog(string msg);

        public ICommand ClearCommand { get; }

        public Func<string> LogInsertCallBack { get; }

        private static readonly object LOCKER_LOG = new object();

        public OnLog OnLogCallBack;

        private ObservableCollection<string> _logMessages;

        private LogHelper logHelper;

        private HttpClientService httpClientService;
		public void RegisterLogCallBackFunc(OnLog Func)
        {
            OnLogCallBack += Func;
        }

        public void MqttLog(string msg)
        {
            logHelper.MqttLogger.Info(msg);
        }

        public void DebugfLog(string msg)
        {
            logHelper.DebugLogger.Info(msg);
        }

        public void MqttError(string msg)
        {
            logHelper.MqttLogger.Error(msg);
        }


        public void Log(string msg)
        {
            logHelper.CommLogger.Info(msg);
            OnLogCallBack?.Invoke(msg);

        }

        public void Warning(string msg)
        {
            logHelper.CommLogger.Warn(msg);
            OnLogCallBack?.Invoke(msg);

        }

        public void Error(string msg)
        {
            logHelper.CommLogger.Error(msg);
            OnLogCallBack?.Invoke(msg);

        }

		public void PostLog(string key,string value)
		{
            if(httpClientService == null) 
            {
                lock(LOCKER_LOG)
                {
                    if(httpClientService == null)
                    {
                        httpClientService = new HttpClientService();
                    }
                }
            }
            httpClientService.Post(key, value);
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
            logHelper = new LogHelper();
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

}
