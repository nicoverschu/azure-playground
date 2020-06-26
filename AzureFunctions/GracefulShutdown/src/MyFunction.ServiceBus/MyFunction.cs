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

#error Please, setup a valid Service Bus connection string

        [FunctionName(nameof(RunAsync))]
        public async Task RunAsync(
            [ServiceBusTrigger("#queueName#", Connection = "ServiceBusConnectionString")]
            Message message,
            [ServiceBus("#queueName#", Connection = "ServiceBusConnectionString")]
            MessageSender queue,
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
                log.LogError($"Waiting for {msg.nSecondsShutdownDuration} seconds before scheduling message...");
                await Task.Delay(msg.nSecondsShutdownDuration * 1000); // observation: shutdown occurs immediately.

                // Schedule message back in the queue, so that it is processed one minute later
                var retryMsg = new Message(message.Body);
                await queue.ScheduleMessageAsync(retryMsg, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(1));
            }
        }
    }
}