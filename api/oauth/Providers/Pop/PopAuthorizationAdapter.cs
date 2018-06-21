using System;
using System.Collections.Generic;
using System.Linq;
using OAuth.AspNet.AuthServer;

using Microsoft.AspNetCore.Authentication;
using OAuth.AspNet.AuthServer;
using ONS.AuthProvider.OAuth.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.OAuth.Providers.Pop
{

    public class PopAuthorizationAdapter: IAuthorizationAdapter 
    {    
        

        public PopAuthorizationAdapter(IConfiguration configuration, ILogger logger) {

        }

        public void SetConfiguration(OAuthAuthorizationServerOptions options) 
        {
            // TODO
            
            

            
            /*options.ApplicationCanDisplayErrors = true;
            options.AllowInsecureHttp = configAllowInsecureHttp;
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
            };*/                
        }
    }

}