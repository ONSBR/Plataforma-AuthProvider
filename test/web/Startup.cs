using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ONS.AuthProvider.Validator;

namespace ONS.AuthProvider.Test.Web
{
    public class Startup
    {
        private const string KeyConfigIssuer = "Auth:Validate:Issuer";
        private const string KeyConfigAudience = "Auth:Validate:Audience";
        private const string KeyConfigSecretKey = "Auth:Validate:SecretKey";
        private const string KeyConfigUseRsa = "Auth:Validate:UseRsa";
        private const string KeyConfigRsaPublicKeyXml = "Auth:Validate:RsaPublicKeyXml";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var configIssuer = Configuration[KeyConfigIssuer];
            var configAudience = Configuration[KeyConfigAudience];
            var configSecretKey = Configuration[KeyConfigSecretKey];
            var configUseRsa = "true".Equals(Configuration[KeyConfigUseRsa], StringComparison.InvariantCultureIgnoreCase);
            var configRsaPublicKeyXml = Configuration[KeyConfigRsaPublicKeyXml];

            var authOptions = new AuthValidateOptions{
                ValidIssuer = configIssuer,
                ValidAudience = configAudience,
                ValidSecretKey = configSecretKey,
                UseRsa = configUseRsa,
                FileRsaPublicKeyXml = configRsaPublicKeyXml
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
