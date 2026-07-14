using log4net;
using log4net.Config;

namespace YS.Yuanji.Log
{
    public class LogHelper
    {
        public LogHelper()
        {
            if (!Init())
            {
               
                throw new Exception("Can't init logger");
            }
        }

        public  ILog CommLogger = LogManager.GetLogger("CommunicationLabLogger");

        public  ILog MqttLogger = LogManager.GetLogger("MqttLabLogger");

        public  ILog DebugLogger = LogManager.GetLogger("MsgLogger");

        public  bool Init()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            string path = AppDomain.CurrentDomain.BaseDirectory + "log4net.config";
            if (!File.Exists(path))
            {
                Console.WriteLine($"{AppDomain.CurrentDomain.BaseDirectory + "log4net.config"} is not exist");
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
}
