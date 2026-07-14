using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Yuanji.Drive
{
    public interface ITransport:IDisposable
    {
        string Name { get; }

        Task<bool> InitializationAsync(TransportConfig config);
        Task<bool> ConnectAsync();

        Task<bool> DisconnectAsync();

        Task<bool> SendAsync(object connent, params string[] agrs);

        Task RecvieData(Action<string,string> revice);

    }
}
