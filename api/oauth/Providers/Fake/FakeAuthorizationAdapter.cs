using System;
using System.Collections.Generic;
using System.Linq;
using OAuth.AspNet.AuthServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;

using ONS.AuthProvider.OAuth.Providers;
using ONS.AuthProvider.OAuth.Util;
using Microsoft.Extensions.Caching.Memory;

namespace ONS.AuthProvider.OAuth.Providers.Fake
{
    ///<summary>Classe que representa o adaptador de configuração do provider OAuth de modo Fake.
    ///O Adatador Fake valida os dados de autenticação conforme as configurações fake, e gera o token com formato JWT.
    ///A geração do JWT segue as configurações do adaptador.</summary>
    public class FakeAuthorizationAdapter: IAuthorizationAdapter 
    {   
        private const string KeyConfigAuthorizePath = "Auth:Server:Adapters:Fake:AuthorizeEndpointPath";
        private const string KeyConfigTokenPath = "Auth:Server:Adapters:Fake:TokenEndpointPath";
        private const string KeyConfigAllowInsecureHttp = "Auth:Server:Adapters:Fake:AllowInsecureHttp";
        private const string KeyConfigAutomaticAuthenticate = "Auth:Server:Adapters:Fake:AutomaticAuthenticate";
        private const string KeyConfigFakeJwtRefreshExpiration = "Auth:Server:Adapters:Fake:Jwt.Token:Refresh.Expiration.Minutes";
        private const string KeyConfigFakeJwtExpiration = "Auth:Server:Adapters:Fake:Jwt.Token:Expiration.Minutes";

        private readonly ILogger _logger;

        public FakeAuthorizationAdapter() {
            _logger = AuthLoggerFactory.Get<FakeAuthorizationAdapter>();
        }

        ///<summary>Método responsável pela configuração do adaptador de providers.</summary>
        ///<param name="app">Aplicação de autenticação.</param>
        public void ConfigureApp(IApplicationBuilder app)  
        {
            var authorizeEndpointPath = AuthConfiguration.Get(KeyConfigAuthorizePath);
            var tokenEndpointPath = AuthConfiguration.Get(KeyConfigTokenPath);
            var allowInsecureHttp = "true".Equals(AuthConfiguration.Get(KeyConfigAllowInsecureHttp));
            var automaticAuthenticate = "true".Equals(AuthConfiguration.Get(KeyConfigAutomaticAuthenticate));

            var expiration = Convert.ToDouble(AuthConfiguration.Get(KeyConfigFakeJwtExpiration));
            var expirationRefreshToken = Convert.ToDouble(AuthConfiguration.Get(KeyConfigFakeJwtRefreshExpiration));

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(expirationRefreshToken))
                .RegisterPostEvictionCallback(callback: (object key, object value, EvictionReason reason, object state) => {
                    _logger.LogDebug(string.Format("Refresh token exprired. token:{0}", key));
                }, state: this);
                
            CacheManager.Cache.SetOptions(cacheEntryOptions);

            var provider = new FakeAuthorizationProvider();
            var refreshProvider = new FakeRefreshTokenProvider();
            var accessTokenFormat = new FakeJwtFormat();

            app.UseOAuthAuthorizationServer(options => {

                options.AuthorizeEndpointPath = new PathString(authorizeEndpointPath);
                options.TokenEndpointPath = new PathString(tokenEndpointPath);
                options.ApplicationCanDisplayErrors = true;                       
                options.AllowInsecureHttp = allowInsecureHttp;

                options.AutomaticAuthenticate = automaticAuthenticate;
                options.AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(expiration);
                
                options.Provider = provider;
                options.AccessTokenFormat = accessTokenFormat;
                options.RefreshTokenProvider = refreshProvider;
            });
        }

    }

}