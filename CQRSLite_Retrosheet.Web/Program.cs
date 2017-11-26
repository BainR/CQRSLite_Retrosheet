using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Web;
using Microsoft.Extensions.Logging;

namespace CQRSLite_Retrosheet.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BuildWebHost(args).Run(); // this is from the dotnet core 2 template

            // The following manually sets up the configuration to get the port number from the hosting.json
            //     and to use Kestrel directly without going through an IIS proxy.

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                // .UseIISIntegration()
                .ConfigureLogging(b => b.SetMinimumLevel(LogLevel.Trace))
                .UseStartup<Startup>()
                .UseNLog()
                .Build();

            host.Run();
        }

        // this is from the dotnet core 2 template
        //public static IWebHost BuildWebHost(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>()
        //        .Build();
    }
}
