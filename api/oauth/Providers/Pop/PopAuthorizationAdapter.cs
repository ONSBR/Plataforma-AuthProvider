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

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    ///<summary>Classe que representa o adaptador de configuração do provider OAuth do POP.
    ///O adaptador do Pop permite fazer a configuração remota o servidor de autenticação padrão do ONS.</summary>
    public class PopAuthorizationAdapter: IAuthorizationAdapter 
    {   
        private const string KeyConfigAdapterPop = "Auth:Server:Adapters:Pop";

        private readonly ILogger _logger;

        public PopAuthorizationAdapter() {
            _logger = AuthLoggerFactory.Get<PopAuthorizationAdapter>();
        }

        ///<summary>Método responsável pela configuração do adaptador de providers.</summary>
        ///<param name="app">Aplicação de autenticação.</param>
        public void ConfigureApp(IApplicationBuilder app)  
        {
            var config = _getConfiguration();
            config.Validate();

            var configUrlJwtOAuth = System.Environment.GetEnvironmentVariable("PopUrlJwtOAuth");
            if (!string.IsNullOrEmpty(configUrlJwtOAuth)) {
                config.JwtToken.UrlJwtOAuth = configUrlJwtOAuth;
            }
            
            var provider = new PopAuthorizationProvider(config.JwtToken);
            var tokenProvider = new PopAuthenticationTokenProvider(config.JwtToken);
            var refreshProvider = new PopRefreshTokenProvider();

            app.UseOAuthAuthorizationServer(options => {

                options.AuthorizeEndpointPath = new PathString(config.AuthorizeEndpointPath);
                options.TokenEndpointPath = new PathString(config.TokenEndpointPath);
                options.ApplicationCanDisplayErrors = true;                       
                options.AllowInsecureHttp = config.AllowInsecureHttp.Value;

                options.AutomaticAuthenticate = config.AutomaticAuthenticate.Value;
                
                options.Provider = provider;
                options.AccessTokenProvider = tokenProvider;
                options.RefreshTokenProvider = refreshProvider;
            });
        }

        private PopConfiguration _getConfiguration() {
            var section = AuthConfiguration.Configuration.GetSection(KeyConfigAdapterPop);
            var configuration = section.Get<PopConfiguration>();
            return configuration;
        }
    }

}