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
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ONS.AuthProvider.Validator;

namespace ONS.AuthProvider.AppTest
{
    public class Startup
    {
        private const string ConfigPathAuthIssuer = "Auth:Validate:Issuer";
        private const string ConfigPathAuthAudience = "Auth:Validate:Audience";
        private const string ConfigPathAuthKey = "Auth:Validate:Key";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authOptions = new AuthValidateOptions{
                ValidIssuer = Configuration[ConfigPathAuthIssuer],
                ValidAudience = Configuration[ConfigPathAuthAudience],
                ValidKey = Configuration[ConfigPathAuthKey]
            };
            AuthConfigurationValidate.Configure(services, authOptions);

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
            
            // OBS: importante que fique antes do UseMvc
            app.UseAuthentication();
            
            app.UseMvc();
            
        }

    }
}
