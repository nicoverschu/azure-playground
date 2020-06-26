using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: FunctionsStartup(typeof(GracefulShutdown.MyFunction.Http.Startup))]
namespace GracefulShutdown.MyFunction.Http
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
