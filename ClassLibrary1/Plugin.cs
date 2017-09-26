using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public abstract class Plugin : IDisposable
    {
        public virtual Task OnStartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnPauseAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnContinueAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnStopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual void Dispose()
        {
        }
    }
}
