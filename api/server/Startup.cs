using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using ONS.AuthProvider.Api.Services.Impl.Pop;
using ONS.AuthProvider.Api.Services.Impl;
using ONS.AuthProvider.Api.Services;

using ONS.AuthProvider.Api.Exception;

namespace ONS.AuthProvider.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var servicesProvider = services.BuildServiceProvider();
            var loggerError = servicesProvider.GetRequiredService<ILoggerFactory>().CreateLogger<ErrorHandlingFilter>();

            services.AddMvc(options =>
            {
                options.Filters.Add(new ErrorHandlingFilter(loggerError));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Declara todos os serviços injetados.
            services.AddSingleton<IAuthServiceFactory, AuthServiceFactoryImpl>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
