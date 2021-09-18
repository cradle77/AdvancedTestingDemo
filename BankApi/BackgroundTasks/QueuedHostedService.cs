using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BankApi.BackgroundTasks
{
    // Copied from here
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-5.0&tabs=visual-studio#queued-background-tasks-1

    public class QueuedHostedService : BackgroundService
    {
        public IBackgroundTaskQueue TaskQueue { get; }

        private IServiceProvider _serviceProvider;


        public QueuedHostedService(IBackgroundTaskQueue taskQueue, IServiceProvider serviceProvider)
        {
            TaskQueue = taskQueue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        await workItem(scope.ServiceProvider, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    // we log the exception here
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await base.StopAsync(stoppingToken);
        }
    }
}
