using System.IO.Pipes;
using System.Text;
using YS.Yuanji.Log;

namespace Yuanji.core
{
    public class Service
    {

        private NamedPipeServerStream _server;
        private CancellationTokenSource _cts;

        public void Main(object args)
        {
            try
            {
                //Task.Delay(5000);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                _cts = new CancellationTokenSource();
                //Task.Run(() => RegesitLog(_cts.Token), _cts.Token);
                LogController.Instance.Log($"YSDL.Start.....");

                if (args != null && args is Dictionary<string, string> context)
                {
                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                       // LogController.Instance.Error($"Load config: {JsonConvert.SerializeObject(DataCollectContoller.Instance.config)},exception:{ex}");
                    }
                }
                else
                {
                   
                }
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"Main: error:{ex}");
            }

        }

        public void Exit()
        {
            try
            {
                LogController.Instance.Log($"YSDL.stop.....");
                _cts?.Cancel();
                _server?.Close();
                _server?.Dispose();
                LogController.Instance.Log($"YSDL.stop finish!");
            }
            catch (Exception ex)
            {
                LogController.Instance.Error($"Exit: error:{ex}");
            }
        }
    }
}

