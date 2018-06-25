using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuth.AspNet.AuthServer;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Logging;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using ONS.AuthProvider.OAuth.Util;

using Newtonsoft.Json;

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    ///<summary>Provedor de geração e obtenção do token de atualização de autenticação.</summary>
    public class PopRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly ILogger _logger;
        
        public PopRefreshTokenProvider(): base() {
            _logger = AuthLoggerFactory.Get<PopAuthenticationTokenProvider>();
        }

        ///<summary>Método para criar o token de atualização de autenticação.</summary>
        ///<param name="contexto">Dados da solicitação para geração do token.</param>
        /// <returns>Tarefa para ativar a execução assíncrona </returns>
        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            _logger.LogDebug("Executando CreateAsync...");
            
            var retorno = Task.FromResult<object>(null);

            PopToken token = (PopToken) context.HttpContext.Items[Constants.ResponseTypes.Token];
            
            string refreshToken = token.RefreshToken;
            context.SetToken(refreshToken);
            
            if (_logger.IsEnabled(LogLevel.Debug)) {
                _logger.LogDebug(string.Format("Generated Refresh token. toke: {0}", refreshToken));
            }

            return retorno;
        }

        ///<summary>Método para recuperar o token de atualização de autenticação.</summary>
        ///<param name="contexto">Dados da solicitação para geração do token.</param>
        /// <returns>Tarefa para ativar a execução assíncrona </returns>
        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            _logger.LogDebug("Executando ReceiveAsync...");

            var retorno = Task.FromResult<object>(null);

            var allowedOrigin = (string) context.HttpContext.Items["as:clientAllowedOrigin"];
            if (!string.IsNullOrEmpty(allowedOrigin)) {
                context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            }

            var authProperties = new AuthenticationProperties();
            authProperties.IssuedUtc = DateTime.UtcNow;
            authProperties.ExpiresUtc = DateTime.UtcNow.AddMinutes(60);

            var refreshTicket = new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(
                    new GenericIdentity("", ""),
                    new Claim[0]
                )), 
                authProperties,
                OAuthDefaults.AuthenticationType
            );
            
            context.SetTicket(refreshTicket);

            return retorno;
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

    }

}