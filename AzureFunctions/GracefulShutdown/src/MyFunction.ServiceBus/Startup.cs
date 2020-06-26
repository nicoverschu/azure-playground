using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyFunction.ServiceBus;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MyFunction.ServiceBus
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(
                svc => svc.GetService<ILoggerFactory>()
                .CreateLogger("default"));
        }
    }
}