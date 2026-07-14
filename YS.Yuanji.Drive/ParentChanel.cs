using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YS.Yuanji.Commom;

namespace YS.Yuanji.Drive
{
    public class ParentChanel : IChanlel
    {
        public CancellationToken CancellationToken { get; set; }
        public ChanleConfig Config { get; internal set; }

        public virtual async Task<bool> ConnectAsync()
        {
            return true;
        }

        public virtual async Task<bool> DisconnectAsync()
        {
            return true;
        }

        public virtual bool IsConnected()
        {
            return true;
        }

        public async Task<bool> LoadAsync(ChanleConfig chanleConfig)
        {
            Config = chanleConfig;
            return await InitalAsync();
        }

        public virtual async Task<bool> InitalAsync()
        {
            return true;
        }

        public virtual Task<(bool, string, Dictionary<Item, object>)> ReadAsync(List<Item> items, [CallerMemberName] string? name = null)
        {
            throw new NotImplementedException();
        }

        public virtual async  Task<byte[]> SendCommandAsync(byte[] data, [CallerMemberName] string? name = null)
        {
            return null;
        }

        public virtual Task<(bool, string)> WriteAsync(Item item, byte[] data, [CallerMemberName] string? name = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<(bool, string)> WriteAsync(Item item, object value, [CallerMemberName] string? name = null)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<bool> OnExit()
        {
            return true;
        }
    }
}
