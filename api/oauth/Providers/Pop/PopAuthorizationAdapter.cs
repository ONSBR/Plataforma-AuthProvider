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

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    ///<summary>Classe que representa o adaptador de configuração do provider OAuth do POP.
    ///O adaptador do Pop permite fazer a configuração remota o servidor de autenticação padrão do ONS.</summary>
    public class PopAuthorizationAdapter: IAuthorizationAdapter 
    {   
        private const string KeyConfigAuthorizePath = "Auth:Server:Adapters:Pop:AuthorizeEndpointPath";
        private const string KeyConfigTokenPath = "Auth:Server:Adapters:Pop:TokenEndpointPath";
        private const string KeyConfigAllowInsecureHttp = "Auth:Server:Adapters:Pop:AllowInsecureHttp";
        private const string KeyConfigAutomaticAuthenticate = "Auth:Server:Adapters:Pop:AutomaticAuthenticate";
        
        private readonly ILogger _logger;

        public PopAuthorizationAdapter() {
            _logger = AuthLoggerFactory.Get<PopAuthorizationAdapter>();
        }

        ///<summary>Método responsável pela configuração do adaptador de providers.</summary>
        ///<param name="app">Aplicação de autenticação.</param>
        public void ConfigureApp(IApplicationBuilder app)  
        {
            var authorizeEndpointPath = AuthConfiguration.Get(KeyConfigAuthorizePath);
            var tokenEndpointPath = AuthConfiguration.Get(KeyConfigTokenPath);
            var allowInsecureHttp = "true".Equals(AuthConfiguration.Get(KeyConfigAllowInsecureHttp));
            var automaticAuthenticate = "true".Equals(AuthConfiguration.Get(KeyConfigAutomaticAuthenticate));

            var provider = new PopAuthorizationProvider();
            var tokenProvider = new PopAuthenticationTokenProvider();
            var refreshProvider = new PopRefreshTokenProvider();

            app.UseOAuthAuthorizationServer(options => {

                options.AuthorizeEndpointPath = new PathString(authorizeEndpointPath);
                options.TokenEndpointPath = new PathString(tokenEndpointPath);
                options.ApplicationCanDisplayErrors = true;                       
                options.AllowInsecureHttp = allowInsecureHttp;

                options.AutomaticAuthenticate = automaticAuthenticate;
                
                options.Provider = provider;
                options.AccessTokenProvider = tokenProvider;
                options.RefreshTokenProvider = refreshProvider;
            });
        }

    }

}