using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyFunction.ServiceBus
{
    public class MyFunction
    {
        private readonly ILogger log;
        public MyFunction(ILogger log)
        {
            this.log = log;
        }

        [FunctionName(nameof(RunAsync))]
        public async Task RunAsync(
            [ServiceBusTrigger("queue", Connection = "ServiceBusConnectionString")]
            Message message,
            [ServiceBus("retryHandling", Connection = "ServiceBusConnectionString")]
            MessageSender retryQueue,
            CancellationToken cancellationToken)
        {
            MyServiceBusMessage msg;
            try
            {
                msg = Newtonsoft.Json.JsonConvert.DeserializeObject<MyServiceBusMessage>(Encoding.UTF8.GetString(message.Body));
            }
            catch (Exception)
            {
                msg = new MyServiceBusMessage();
            }

            try
            {
                for (int i = 0; i < msg.nSecondsWait; i++)
                {
                    var remainingWait = msg.nSecondsWait - i;
                    log.LogInformation($"Waiting for shutdown for {remainingWait} second(s) - IsCancellationRequested={cancellationToken.IsCancellationRequested}");
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (Exception)
            {
                log.LogError($"HOST IS BEING SHUT DOWN - IsCancellationRequested={cancellationToken.IsCancellationRequested}");
                log.LogError($"Waiting for {msg.nSecondsShutdownDuration} seconds before sending message to retry queue...");
                await Task.Delay(msg.nSecondsShutdownDuration * 1000); // observation: ?

                // Send the message in a retry queue
                var retryMsg = new Message(message.Body);
                await retryQueue.SendAsync(retryMsg);
            }
        }
    }
}