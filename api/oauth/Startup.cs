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

using Microsoft.AspNetCore.Http;
using ONS.AuthProvider.OAuth.Providers;
using ONS.AuthProvider.OAuth.Providers.Pop;
using ONS.AuthProvider.OAuth.Providers.Fake;
using ONS.AuthProvider.OAuth.Util;

namespace ONS.AuthProvider.OAuth
{
    public class Startup
    {
        private const string ConfigAuthAdapterUse = "Auth:Adapter:Use";

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
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, 
            ILoggerFactory loggerFactory, ILogger<Startup> logger)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(AuthConfiguration.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            AuthConfiguration.Logger = loggerFactory.CreateLogger<AuthConfiguration>();
            
            AuthorizationAdapterFactory.Add("pop", new PopAuthorizationAdapter(
                loggerFactory.CreateLogger<PopAuthorizationAdapter>()));
            AuthorizationAdapterFactory.Add("fake", new FakeAuthorizationAdapter(
                loggerFactory.CreateLogger<FakeAuthorizationAdapter>()));
            
            // Setup Authorization Server
            var configAdapterUse = AuthConfiguration.Get(ConfigAuthAdapterUse);

            app.UseOAuthAuthorizationServer(options => {
                AuthorizationAdapterFactory.Get(configAdapterUse).SetConfiguration(options);
            });

            app.UseMvc();

            logger.LogDebug("Sistema inicializado com sucesso.");
        }
    }
}
