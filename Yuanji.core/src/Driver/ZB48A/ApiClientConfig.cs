using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  Yuanji.core.src.Driver.ZB48A
{
    public static class ApiClientFactory
    {
        public static ApiClient CreateClient(ApiClientConfig config)
        {
            return new ApiClient(config);
        }
    }


    public class ApiClientConfig
    {
        public string IpAddress { get; set; }
        public string ApiId { get; set; } = "test";
        public string ApiSecret { get; set; } = "123456789";

        public int Port {  get; set; }

        public string baseUrl { get => $"http://{IpAddress}:{Port}"; }

        public ApiClientConfig(string ipAddress, string apiId, string apiSecret,int port=9910)
        {
            IpAddress = ipAddress;
            Port = port;
            ApiId = apiId;
            ApiSecret = apiSecret;
        }
    }
}
