using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuth.AspNet.AuthServer;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authentication;
using ONS.AuthProvider.OAuth.Util;

using Newtonsoft.Json;

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    /// <summary>Provedor do servidor para se comunicar com o aplicativo da Web durante o 
    /// processamento de solicitações, com autenticação no POP.</summary>
    public class PopAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        private readonly ILogger _logger;

        public PopAuthenticationTokenProvider(): base() {
            _logger = AuthLoggerFactory.Get<PopAuthenticationTokenProvider>();
            OnCreate = CreateAuthenticationCode;
        }

        /// <summary>Cria o o token do parâmetro access_token. No caso obtém da resposta do POP.</summary>
        public void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            PopToken token = (PopToken) context.HttpContext.Items[Constants.ResponseTypes.Token];
            
            string accessToken = token.AccessToken;
            context.SetToken(accessToken);
            
            var utcNow = DateTime.UtcNow;
            context.Ticket.Properties.IssuedUtc = utcNow;
            context.Ticket.Properties.ExpiresUtc = utcNow.AddSeconds(token.ExpiresIn);
        }

    }
}