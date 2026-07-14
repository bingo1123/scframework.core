
using System.Runtime.CompilerServices;
using YS.Yuanji.Commom;

namespace YS.Yuanji.Drive
{
    public interface IChanlel
    {
        CancellationToken CancellationToken { get; set; }
        Task<bool> LoadAsync(ChanleConfig chanleConfig);

        Task<bool> ConnectAsync();

        bool IsConnected();

        Task<bool> DisconnectAsync();

        Task<byte[]> SendCommandAsync(byte[] data, [CallerMemberName] string? name = null);

        Task<(bool,String)> WriteAsync(Item item, byte[] data, [CallerMemberName] string? name = null);

        Task<(bool, String)> WriteAsync(Item item, object value, [CallerMemberName] string? name = null);

        Task<(bool, String,Dictionary<Item,object>)> ReadAsync(List<Item> items, [CallerMemberName] string? name = null);

        Task<bool> OnExit();

    }
}
