using System;
using System.Collections.Generic;
using System.Linq;
using OAuth.AspNet.AuthServer;
using Microsoft.AspNetCore.Builder;

//using Microsoft.AspNetCore.Authentication;

using ONS.AuthProvider.OAuth.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    public class PopAuthorizationAdapter: IAuthorizationAdapter 
    {    
        private readonly ILogger _logger;

        public PopAuthorizationAdapter(ILogger logger) {
            _logger = logger;
        }

        public void SetConfiguration(OAuthAuthorizationServerOptions options) 
        {
            // TODO
            /* 
            options.Provider = new OAuthAuthorizationServerProvider
            {
                OnValidateClientRedirectUri = ValidateClientRedirectUri,
                OnValidateClientAuthentication = ValidateClientAuthentication,
                OnGrantResourceOwnerCredentials = GrantResourceOwnerCredentials,
                OnGrantClientCredentials = GrantClientCredetails
            };
            options.AuthorizationCodeProvider = new AuthenticationTokenProvider
            {
                OnCreate = CreateAuthenticationCode,
                OnReceive = ReceiveAuthenticationCode,
            };
            options.RefreshTokenProvider = new AuthenticationTokenProvider
            {
                OnCreate = CreateRefreshToken,
                OnReceive = ReceiveRefreshToken,
            };  */              
        }
    }

}