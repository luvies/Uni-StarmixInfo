using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StarmixInfo
{
    public class Program
    {
        const string LogFolder = "Logs";
        const string LogFile = "webserver-{Date}.log";

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .ConfigureAppConfiguration(
                       (context, config) =>
                       {
                           config.AddJsonFile("Secrets/appsecrets.json",
                                              optional: false);
                           config.AddJsonFile($"Secrets/appsecrets.{context.HostingEnvironment.EnvironmentName}.json",
                                              optional: true);
                       })
                   .ConfigureLogging((context, loggerFactory) =>
                                     loggerFactory.AddFile(Path.Combine(LogFolder, LogFile),
                                                           isJson: true))
                   .Build();
    }
}
