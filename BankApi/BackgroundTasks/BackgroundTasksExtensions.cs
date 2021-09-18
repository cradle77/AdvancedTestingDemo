using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BankApi.BackgroundTasks
{
    internal static class BackgroundTasksExtensions
    {
        public static IServiceCollection AddBackgroundQueue(this IServiceCollection services)
        {
            services.TryAddSingleton<IBackgroundTaskQueue>(sp => new BackgroundTaskQueue(10));

            services.AddHostedService<QueuedHostedService>();

            return services;
        }
    }
}
