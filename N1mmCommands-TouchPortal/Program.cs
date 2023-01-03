/* This project published at https://github.com/frazierjason/N1mmCommands-TouchPortal
 * under the MIT license.
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using N1MMCommandsPlugin.Properties;
using TouchPortalSDK;
using TouchPortalSDK.Interfaces;
using TouchPortalSDK.Messages.Events;
using TouchPortalSDK.Configuration;
using Serilog;

namespace N1mmCommands.Touchportal
{
    public class Program
    {
        private static void Main(string[] args)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("Program");
            const string mutexName = "n1mm.commands.tp";
            _ = new System.Threading.Mutex(true, mutexName, out var createdNew);

            if (!createdNew)
            {
                logger.LogError($"{mutexName} is already running. Exiting application.");
                return;
            }

            var configurationRoot = new ConfigurationBuilder()
#pragma warning disable CS8602 // Dereference of a possibly null reference.
              .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
              .AddJsonFile("appsettings.json", false, true)
              .Build();

            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(configure =>
            {
                //configure.AddSimpleConsole(options => options.TimestampFormat = "[yyyy.MM.dd HH:mm:ss] ");
                //configure.AddConfiguration(configurationRoot.GetSection("Logging"));
                configure.ClearProviders();
                configure.AddSerilog(logger: new LoggerConfiguration().ReadFrom.Configuration(configurationRoot)
                    .CreateLogger(), dispose: true);
            });

            //Registering the Plugin to the IoC container:
            serviceCollection.AddTouchPortalSdk(configurationRoot);
            serviceCollection.AddSingleton<Plugin>();

            var serviceProvider = serviceCollection.BuildServiceProvider(true);

            var plugin = serviceProvider.GetRequiredService<Plugin>(); // new N1mmCommands();
            plugin.Run();
        }
    }
}