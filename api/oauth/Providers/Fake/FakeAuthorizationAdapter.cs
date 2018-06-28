using System;
using System.Collections.Generic;
using System.Linq;
using OAuth.AspNet.AuthServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;

using ONS.AuthProvider.OAuth.Providers;
using ONS.AuthProvider.OAuth.Util;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ONS.AuthProvider.OAuth.Providers.Fake
{
    ///<summary>Classe que representa o adaptador de configuração do provider OAuth de modo Fake.
    ///O Adatador Fake valida os dados de autenticação conforme as configurações fake, e gera o token com formato JWT.
    ///A geração do JWT segue as configurações do adaptador.</summary>
    public class FakeAuthorizationAdapter: IAuthorizationAdapter 
    {   
        private const string KeyConfigAdapterFake = "Auth:Server:Adapters:Fake";

        private readonly ILogger _logger;

        public FakeAuthorizationAdapter() {
            _logger = AuthLoggerFactory.Get<FakeAuthorizationAdapter>();
        }

        ///<summary>Método responsável pela configuração do adaptador de providers.</summary>
        ///<param name="app">Aplicação de autenticação.</param>
        public void ConfigureApp(IApplicationBuilder app)  
        {
            var config = _getConfiguration();
            config.Validate();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(config.JwtToken.RefreshExpiration.Value))
                .RegisterPostEvictionCallback(callback: (object key, object value, EvictionReason reason, object state) => {
                    _logger.LogDebug(string.Format("Refresh token exprired. token:{0}", key));
                }, state: this);
                
            CacheManager.Cache.SetOptions(cacheEntryOptions);

            var provider = new FakeAuthorizationProvider(config);
            var refreshProvider = new FakeRefreshTokenProvider();
            var accessTokenFormat = new FakeJwtFormat(config.JwtToken);

            app.UseOAuthAuthorizationServer(options => {

                options.AuthorizeEndpointPath = new PathString(config.AuthorizeEndpointPath);
                options.TokenEndpointPath = new PathString(config.TokenEndpointPath);
                options.ApplicationCanDisplayErrors = true;                       
                options.AllowInsecureHttp = config.AllowInsecureHttp.Value;

                options.AutomaticAuthenticate = config.AutomaticAuthenticate.Value;
                options.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(config.JwtToken.Expiration.Value);
                
                options.Provider = provider;
                options.AccessTokenFormat = accessTokenFormat;
                options.RefreshTokenProvider = refreshProvider;
            });
        }

        private FakeConfiguration _getConfiguration() {
            var section = AuthConfiguration.Configuration.GetSection(KeyConfigAdapterFake);
            var configuration = section.Get<FakeConfiguration>();
            return configuration;
        }

    }

}