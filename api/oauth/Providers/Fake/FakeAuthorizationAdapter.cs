using System;
using System.Collections.Generic;
using System.Linq;
using OAuth.AspNet.AuthServer;
using Microsoft.AspNetCore.Http;

using OAuth.AspNet.AuthServer;
using ONS.AuthProvider.OAuth.Providers;

namespace ONS.AuthProvider.OAuth.Providers.Fake
{

    public class FakeAuthorizationAdapter: IAuthorizationAdapter {
        
        public void SetConfiguration(OAuthAuthorizationServerOptions options) {
            
            options.AuthorizeEndpointPath = new PathString("");
            options.TokenEndpointPath = new PathString("");
            options.ApplicationCanDisplayErrors = true;                       
            options.AllowInsecureHttp = true;
            /*options.Provider = new OAuthAuthorizationServerProvider
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