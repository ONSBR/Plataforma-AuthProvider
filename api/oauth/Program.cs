﻿using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ONS.AuthProvider.OAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var envport = Environment.GetEnvironmentVariable("PORT");

            if (args.Length > 0) envport = args[0];

            var webHost = CreateWebHostBuilder(args);

            if (!string.IsNullOrEmpty(envport)) webHost.UseUrls("http://*:" + envport);

            webHost.Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();
        }
    }
}