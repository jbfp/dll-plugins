using ClassLibrary1;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyPlugin
{
    [Plugin("My Plugin")]
    public sealed class MyPlugin : Plugin
    {
        private static readonly TimeSpan period = TimeSpan.FromSeconds(1);

        private readonly CancellationTokenSource cts;
        private readonly Timer timer;

        public MyPlugin()
        {
            Console.WriteLine("Constructing timer...");

            cts = new CancellationTokenSource();
            timer = new Timer(OnTimerCallback);
        }

        public override void Dispose()
        {
            Console.WriteLine("Disposing timer...");

            cts.Cancel();
            cts.Dispose();
            timer.Dispose();
        }

        public override Task OnStartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting timer...");

            timer.Change(TimeSpan.Zero, period);

            return base.OnStartAsync(cancellationToken);
        }

        public override Task OnPauseAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Pausing timer...");

            timer.Change(Timeout.InfiniteTimeSpan, period);

            return base.OnPauseAsync(cancellationToken);
        }

        public override Task OnContinueAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Continuing timer...");

            timer.Change(TimeSpan.Zero, period);

            return base.OnContinueAsync(cancellationToken);
        }

        public override Task OnStopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping timer...");

            timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

            return base.OnStopAsync(cancellationToken);
        }

        private static void OnTimerCallback(object _)
        {
            Console.WriteLine("Callback!");
        }
    }
}
