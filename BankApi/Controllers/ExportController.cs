using BankApi.BackgroundTasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BankApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private IBackgroundTaskQueue _tasks;

        public ExportController(IBackgroundTaskQueue tasks)
        {
            _tasks = tasks;
        }

        public async Task<IActionResult> SubmitExportRequest([FromQuery] string accountNumber)
        {
            await _tasks.QueueBackgroundWorkItemAsync(async (sp, token) => 
            {
                // var export = sp.GetService<IExportService>();
                // var url = await export.GeneratePdfStatementAsync(accountNumber);

                // var notifier = sp.GetService<INotifier>();
                // await notifier.SendNotificationAsync(this.User, $"The export is ready at {url}");

                await Task.Delay(2000);
            });

            return this.Ok();
        }
    }
}
