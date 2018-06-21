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
using OAuth.AspNet.AuthServer;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using ONS.AuthProvider.OAuth.Providers;
using ONS.AuthProvider.OAuth.Providers.Pop;
using ONS.AuthProvider.OAuth.Providers.Fake;

namespace ONS.AuthProvider.OAuth
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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

            var configAdapterUse = "pop";
            var configTokenEndpointPath = "/oauth2/token";
            var configAuthorizeEndpointPath = "/authi";
            var configAllowInsecureHttp = true;

            AuthorizationAdapterFactory.Add("pop", new PopAuthorizationAdapter(null, null));

            // Setup Authorization Server
            app.UseOAuthAuthorizationServer(
                options => {
                    options.AuthorizeEndpointPath = new PathString(configAuthorizeEndpointPath);
                    options.TokenEndpointPath = new PathString(configTokenEndpointPath);
                    options.AllowInsecureHttp = configAllowInsecureHttp;
                    options.ApplicationCanDisplayErrors = true;
                    //options.TokenEndpointPath = new PathString(configTokenEndpointPath);
                    //AuthorizationAdapterFactory.Get(configAdapterUse).SetConfiguration(options);       
                }
            );

            app.UseMvc();
        }
    }
}
