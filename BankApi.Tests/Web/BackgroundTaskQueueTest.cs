using BankApi.BackgroundTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Xunit.Sdk;
using Moq;

namespace BankApi.Tests.Web
{
    public class BackgroundTaskQueueTest : WebTestBase
    {
        [Fact]
        public async Task BackgroundTaskQueue_WhenTaskSubmitted_ExecutesTheTask()
        {
            var sync = new SemaphoreSlim(0);

            var client = this.BuildWebApplicationFactory(
                configServices: services =>
                {
                    services.AddBackgroundQueue();
                },
                configBuilder: app => 
                {
                    app.Use(async (ctx, next) =>
                    {
                        var queue = ctx.RequestServices.GetRequiredService<IBackgroundTaskQueue>();

                        await queue.QueueBackgroundWorkItemAsync((sp, token) =>
                        {
                            sync.Release();

                            return ValueTask.CompletedTask;
                        });

                        ctx.Response.StatusCode = StatusCodes.Status200OK;
                    });
                })
                .CreateClient();

            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            var syncAwaiter = sync.WaitAsync();
            if (syncAwaiter != await Task.WhenAny(syncAwaiter, Task.Delay(10000)))
            {
                throw new XunitException("task not completed");

            }
        }

        [Fact]
        public async Task BackgroundTaskQueue_WhenTaskCompleted_DisposesServices()
        {
            var firstTask = new SemaphoreSlim(0);
            var secondTask = new SemaphoreSlim(0);

            var disposableService = new Mock<IDisposable>();

            var client = this.BuildWebApplicationFactory(
                configServices: services =>
                {
                    services.AddBackgroundQueue();

                    services.AddTransient<IDisposable>(sp => disposableService.Object);
                },
                configBuilder: app =>
                {
                    app.Use(async (ctx, next) =>
                    {
                        var queue = ctx.RequestServices.GetRequiredService<IBackgroundTaskQueue>();

                        await queue.QueueBackgroundWorkItemAsync(async (sp, token) =>
                        {
                            sp.GetRequiredService<IDisposable>();
                            await firstTask.WaitAsync();
                        });

                        // let's queue a second task for sync purposes
                        await queue.QueueBackgroundWorkItemAsync((sp, token) =>
                        {
                            secondTask.Release();

                            return ValueTask.CompletedTask;
                        });

                        ctx.Response.StatusCode = StatusCodes.Status200OK;
                    });
                })
                .CreateClient();

            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();

            disposableService.Verify(x => x.Dispose(), Times.Never, "Dispose should not have been called before the task has completed");

            firstTask.Release(); // first task completed

            // sometime at this point (the threads are concurrent, Dispose will be called)

            await secondTask.WaitAsync(); // second task completed

            // the dependencies on the first task should've been disposed now
            disposableService.Verify(x => x.Dispose(), Times.Once, "Dispose has not been called on the dependency");
        }
    }
}
