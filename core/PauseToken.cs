using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace deploytool.core
{
    /// <summary>
    ///  暂停功能实现
    /// </summary>
    public class PauseTokenSource
    {
        private volatile TaskCompletionSource<bool> _paused;
        public PauseToken Token => new PauseToken(this);

        public bool IsPaused => _paused != null;

        public void Pause()
        {
            if (_paused == null)
                Interlocked.CompareExchange(ref _paused, new TaskCompletionSource<bool>(), null);
        }

        public void Resume()
        {
            if (_paused != null)
            {
                Interlocked.Exchange(ref _paused, null)?.SetResult(true);
            }
        }

        public Task WaitWhilePausedAsync()
        {
            return _paused?.Task ?? Task.CompletedTask;
        }
    }

    public struct PauseToken
    {
        private readonly PauseTokenSource _source;
        public PauseToken(PauseTokenSource source) => _source = source;
        public Task WaitWhilePausedAsync() => _source?.WaitWhilePausedAsync() ?? Task.CompletedTask;
    }
}
