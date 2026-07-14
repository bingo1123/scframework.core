using Microsoft.Extensions.Hosting;
using YS.Yuanji.Log;

namespace Yuanji.Manage
{
    public class HostSevice : IHostedService
    {
        private readonly StarupSevice starupSevice;
        public HostSevice(StarupSevice starup)
        {
            starupSevice = starup;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                LogController.Instance.Log("Server is StartAsync……");
                await starupSevice.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogController.Instance.Log($"读取设备参数出错...");
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            LogController.Instance.Log("Server is StopAsync……");
            await starupSevice.StopAsync(cancellationToken);

        }

    }
}
