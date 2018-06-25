using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OAuth.AspNet.AuthServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

using Microsoft.AspNetCore.Http;
using ONS.AuthProvider.OAuth.Providers;
using ONS.AuthProvider.OAuth.Providers.Pop;
using ONS.AuthProvider.OAuth.Providers.Fake;
using ONS.AuthProvider.OAuth.Util;

namespace ONS.AuthProvider.OAuth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env) {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            AuthConfiguration.Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, ILogger<Startup> logger, IMemoryCache memoryCache)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(AuthConfiguration.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            _configureAuthProvider(app, loggerFactory, logger, memoryCache);
            
            app.UseMvc();

            logger.LogDebug("Sistema inicializado com sucesso.");
        }

        private void _configureAuthProvider(IApplicationBuilder app, ILoggerFactory loggerFactory, 
            ILogger<Startup> logger, IMemoryCache memoryCache) 
        { 
            AuthLoggerFactory.LoggerFactory = loggerFactory;

            AuthConfiguration.Logger = loggerFactory.CreateLogger<AuthConfiguration>();
    
            CacheManager.Init(memoryCache);

            AuthorizationAdapterFactory.Use().ConfigureApp(app);
        }

        private void _configureTempDir(ILogger<Startup> logger) {
            var dirTemp = System.Environment.GetEnvironmentVariable("Temp");

            if (string.IsNullOrEmpty(ditTemp)) {

                logger.LogWarning("Não encontrada variável de ambiente, para pasta temporária. ENV [Temp].");

                dirTemp = AuthConfiguration.Get("Auth:Server:TempDir");

                System.Environment.SetEnvironmentVariable("Temp", dirTemp);

                logger.LogWarning(string.Format("Será utilizada configuração" +
                 " default: Auth:Server:TempDir={0}", dirTemp);                
            }
        }
    }
}
