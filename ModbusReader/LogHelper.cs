using log4net;
using log4net.Config;
using System.IO;

public static class LogHelper
    {
        static LogHelper()
        {
            if (!Init())
            {
               
                throw new Exception("Can't init logger");
            }
        }

        public static ILog CommLogger = LogManager.GetLogger("CommunicationLabLogger");

        public static ILog MqttLogger = LogManager.GetLogger("MqttLabLogger");

        public static ILog DebugLogger = LogManager.GetLogger("MsgLogger");

        public static bool Init()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "log4net.config";
            if (!File.Exists(path))
            {
                Console.WriteLine($"{AppDomain.CurrentDomain.BaseDirectory + "log4net.config"} Can not init logger");
                return false;
            }
            FileInfo logCfg = new FileInfo(path);

            try
            {
                XmlConfigurator.ConfigureAndWatch(logCfg);
            }
            catch (Exception)
            {
                Console.WriteLine($"{AppDomain.CurrentDomain.BaseDirectory + "log4net.config"} ConfigureAndWatch");
                return false;
            }
            return true;
        }
    }
