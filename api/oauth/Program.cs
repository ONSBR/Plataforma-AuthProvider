using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.OAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envport = System.Environment.GetEnvironmentVariable("PORT");
			
            if (args.Length > 0) {
                envport = args[0];
            }

            var webHost = CreateWebHostBuilder(args);
            
            if (!string.IsNullOrEmpty(envport)) {
                webHost.UseUrls("http://*:" + envport);
            }
            
            webHost.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();
    }
}
