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
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                    .UseStartup<Startup>()
//                    .UseKestrel(options =>
//                    {
//#if DEBUG
//                        //options.Listen(IPAddress.Loopback, 5000);
//                        //options.Listen(IPAddress.Loopback, 5001, listenOpts =>
//                        //{
//                        //    listenOpts.UseHttps("devCert.pfx");
//                        //});
//#else
//                        options.Listen(IPAddress.Any, 80);
//                        options.Listen(IPAddress.Any, 443, listenOpts =>
//                        {
//                            listenOpts.UseHttps("devCert.pfx");
//                        });
//#endif
                    //})
                    .Build();
    }
}
