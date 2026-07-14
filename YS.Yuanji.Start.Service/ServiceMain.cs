
using Microsoft.Extensions.Hosting;
using Topshelf;

namespace YS.Yuanji.Start.Service
{

    public class ServiceMain 
    {
        private IHost host;
        public void Start()
        {
            host = Yuanji.Start.Program.CreateHostBuilder(new string[] { }).Build();
            host.RunAsync();
        }

        public void Stop()
        {
            try
            {
                host.StopAsync();
            }
            catch (Exception ex)
            {
               
            }

        }


    }
}