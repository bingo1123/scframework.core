using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanji.core.srcDriver.Huani.Controller
{
    public class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore;
        public AsyncLock()
        {
            _semaphore = new SemaphoreSlim(1, 1);
        }
        public async Task<IDisposable> LockAsync()
        {
            await _semaphore.WaitAsync();
            return new LockReleaser(_semaphore);
        }
        private class LockReleaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            public LockReleaser(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }
            public void Dispose()
            {
                _semaphore.Release();
            }
        }
    }
}
