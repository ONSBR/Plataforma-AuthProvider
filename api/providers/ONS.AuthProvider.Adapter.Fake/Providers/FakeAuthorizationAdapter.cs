using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ONS.AuthProvider.Common.Providers;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.Adapter.Fake.Providers
{
    /// <summary>
    ///     Classe que representa o adaptador de configuração do provider OAuth de modo Fake.
    ///     O Adatador Fake valida os dados de autenticação conforme as configurações fake, e gera o token com formato JWT.
    ///     A geração do JWT segue as configurações do adaptador.
    /// </summary>
    public class FakeAuthorizationAdapter : IAuthorizationAdapter
    {
        private const string KeyConfigAdapterFake = "Auth:Server:Adapters:Fake";

        private readonly ILogger _logger;

        public FakeAuthorizationAdapter()
        {
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
                .RegisterPostEvictionCallback(
                    (key, value, reason, state) =>
                    {
                        _logger.LogDebug(string.Format("Refresh token exprired. token:{0}", key));
                    }, this);

            CacheManager.Cache.SetOptions(cacheEntryOptions);

            var provider = new FakeAuthorizationProvider(config);
            var refreshProvider = new FakeRefreshTokenProvider();
            var accessTokenFormat = new FakeJwtFormat(config.JwtToken);

            app.UseOAuthAuthorizationServer(options =>
            {
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

        private FakeConfiguration _getConfiguration()
        {
            var section = AuthConfiguration.Configuration.GetSection(KeyConfigAdapterFake);
            var configuration = section.Get<FakeConfiguration>();
            return configuration;
        }
    }
}