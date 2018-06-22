using System;
using System.Collections.Generic;
using System.Linq;
using OAuth.AspNet.AuthServer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using ONS.AuthProvider.OAuth.Providers;
using ONS.AuthProvider.OAuth.Util;

namespace ONS.AuthProvider.OAuth.Providers.Fake
{

    public class FakeAuthorizationAdapter: IAuthorizationAdapter 
    {   
        private const string KeyConfigAuthorizePath = "Auth:Adapter:Providers:fake:AuthorizeEndpointPath";
        private const string KeyConfigTokenPath = "Auth:Adapter:Providers:fake:TokenEndpointPath";
        private const string KeyConfigAllowInsecureHttp = "Auth:Adapter:Providers:fake:AllowInsecureHttp";

        private readonly ILogger _logger;

        public FakeAuthorizationAdapter(ILogger logger) {
            _logger = logger;
        }

        public void SetConfiguration(OAuthAuthorizationServerOptions options) {

            var tokenEndpointPath = AuthConfiguration.Get(KeyConfigTokenPath);
            var authorizeEndpointPath = AuthConfiguration.Get(KeyConfigAuthorizePath);
            var allowInsecureHttp = "true".Equals(AuthConfiguration.Get(KeyConfigAllowInsecureHttp));
Console.WriteLine("tokenEndpointPath: " + tokenEndpointPath + ", authorizeEndpointPath: " + authorizeEndpointPath);
            options.AuthorizeEndpointPath = new PathString(authorizeEndpointPath);
            options.TokenEndpointPath = new PathString(tokenEndpointPath);
            options.ApplicationCanDisplayErrors = true;                       
            options.AllowInsecureHttp = allowInsecureHttp;

            var provider = new FakeAuthorizationProvider();

            options.Provider = new OAuthAuthorizationServerProvider
            {
                OnValidateClientRedirectUri = provider.ValidateClientRedirectUri,
                OnValidateClientAuthentication = provider.ValidateClientAuthentication,
                OnGrantResourceOwnerCredentials = provider.GrantResourceOwnerCredentials,
                OnGrantClientCredentials = provider.GrantClientCredetails
            };
            options.AuthorizationCodeProvider = new AuthenticationTokenProvider
            {
                OnCreate = provider.CreateAuthenticationCode,
                OnReceive = provider.ReceiveAuthenticationCode,
            };
            options.RefreshTokenProvider = new AuthenticationTokenProvider
            {
                OnCreate = provider.CreateRefreshToken,
                OnReceive = provider.ReceiveRefreshToken,
            };     
        }
    }

}