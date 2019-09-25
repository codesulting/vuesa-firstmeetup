using demo.functions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(FunctionStartup))]

namespace demo.functions
{
    public class FunctionStartup : FunctionsStartup
    {
        private readonly ILoggerFactory _loggerFactory;
        private IConfigurationRoot Configuration = null;


        public override void Configure(IFunctionsHostBuilder builder)
        {
            
            var context = builder.Services.BuildServiceProvider().GetService<IOptions<ExecutionContextOptions>>().Value;
            
            Configuration = new ConfigurationBuilder()
                .SetBasePath(context.AppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

         
            // builder.Services.AddScoped(typeof(IContainer),(x) => ApplicationContainer);
               }
    }
}