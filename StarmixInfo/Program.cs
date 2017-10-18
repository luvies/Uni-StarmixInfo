using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
                                              false);
                           config.AddJsonFile($"Secrets/appsecrets.{context.HostingEnvironment.EnvironmentName}.json",
                                              false);
                       })
                   .ConfigureLogging(loggerFactory =>
                                     loggerFactory.AddFile(Path.Combine(LogFolder, LogFile), isJson: true))
                   .Build();
    }
}
