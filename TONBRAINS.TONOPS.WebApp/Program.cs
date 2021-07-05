using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TONBRAINS.TONOPS.Core.Handlers;
using TONBRAINS.TONOPS.WebApp.Middleware;

namespace TONBRAINS.TONOPS.WebApp
{
    public class Program
    {

        public static Dictionary<string, List<string>> History = new Dictionary<string, List<string>>();

        public static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            KataHelpers.ConnectionString = GlobalAppConfHandler.TonOpsDbConnectionString;
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            await UserSessionMiddleware.FillTokens();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args);
            host.UseSystemd();
            return host.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseUrls("https://0.0.0.0:7001", "http://0.0.0.0:5001");
            });
        }
    }
}
