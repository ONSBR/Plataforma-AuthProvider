using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ONS.AuthProvider.Common.Providers;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.OAuth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
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
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

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

            _configureTempDir(logger);

            AuthorizationAdapterFactory.Use().ConfigureApp(app);
        }

        private void _configureTempDir(ILogger<Startup> logger)
        {
            var dirTemp = Environment.GetEnvironmentVariable("Temp");

            if (string.IsNullOrEmpty(dirTemp))
            {
                logger.LogWarning("Não encontrada variável de ambiente, para pasta temporária. ENV [Temp].");

                dirTemp = AuthConfiguration.Get("Auth:Server:TempDir");

                Environment.SetEnvironmentVariable("Temp", dirTemp);

                logger.LogWarning(string.Format("Será utilizada configuração" +
                                                " default: Auth:Server:TempDir={0}", dirTemp));
            }
        }
    }
}