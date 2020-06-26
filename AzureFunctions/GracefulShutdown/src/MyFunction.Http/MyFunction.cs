using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace GracefulShutdown.MyFunction.Http
{
    public class MyFunction
    {
        private readonly ILogger log;
        public MyFunction(ILogger log)
        {
            this.log = log;
        }

        [FunctionName("WaitForShutdown")]
        public async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "wait-for-shutdown/{nSecondsWait:int}/{nSecondsShutdownDuration:int}")]
            HttpRequest req,
            int nSecondsWait,
            int nSecondsShutdownDuration,
            CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i < nSecondsWait; i++)
                {
                    var remainingWait = nSecondsWait - i;
                    log.LogInformation($"Waiting for shutdown for {remainingWait} second(s) - IsCancellationRequested={cancellationToken.IsCancellationRequested}");
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception)
            {
                log.LogError($"HOST IS BEING SHUT DOWN - IsCancellationRequested={cancellationToken.IsCancellationRequested}");
                log.LogError($"Waiting for {nSecondsShutdownDuration} seconds before responding 503...");
                await Task.Delay(nSecondsShutdownDuration * 1000); // observation: around 5 seconds before host forces shutdown
                var result = new JsonResult(new { message = "HOST IS BEING SHUT DOWN" });
                result.StatusCode = 503;
                return result;
            }

            return new JsonResult(new { message = "Host did not shut down" });
        }
    }
}
