using Topshelf;

namespace YS.Yuanji.Start.Service
{
    internal class Program
    {
        // 服务名称需与Topshelf配置的名称一致
        const string serviceName = "QJYS_Serivce";
        static void Main(string[] args)
        {
			var rc = HostFactory.Run(x =>
            {
                x.Service<ServiceMain>(s =>
                {
                    s.ConstructUsing(name => new ServiceMain());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });  // 关键配置
                x.SetServiceName(serviceName);
                x.SetDescription("终端数据采集服务");
                x.StartAutomaticallyDelayed();

                x.RunAsLocalSystem();
            });
            Environment.ExitCode = (int)rc;
            //new ServiceMain().Start();
            //Thread.Sleep(-1);

        }
    }
}
