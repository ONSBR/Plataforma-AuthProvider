using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OAuth.AspNet.AuthServer;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using ONS.AuthProvider.OAuth.Util;
using Newtonsoft.Json;

using System.IdentityModel.Tokens.Jwt;

using ONS.AuthProvider.OAuth.Providers;
using Microsoft.AspNetCore.Authentication;

namespace ONS.AuthProvider.OAuth.Providers.Fake
{
    /// <summary>Provedor do servidor para se comunicar com o aplicativo da Web durante
    /// o processamento de solicitações.</summary>
    public class FakeAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        private const string KeyConfigFakeJwtUsername = "Auth:Server:Adapters:Fake:Jwt.Username";
        private const string KeyConfigFakeJwtPassword = "Auth:Server:Adapters:Fake:Jwt.Password";     
        private const string KeyConfigFakeJwtClientId = "Auth:Server:Adapters:Fake:Jwt.Token:Audience";   
        private const string KeyConfigFakeJwtRole = "Auth:Server:Adapters:Fake:Jwt.Token:Role";

        private const string GrantRole = "role";
        
        private readonly ILogger<FakeAuthorizationProvider> _logger;
        private readonly string[] _configClientIds;
        private readonly string[] _configRoles;

        public FakeAuthorizationProvider(): base() {
            
            _logger = AuthLoggerFactory.Get<FakeAuthorizationProvider>();
            var configStrClientIds = AuthConfiguration.Get(KeyConfigFakeJwtClientId);
            _configClientIds = configStrClientIds.Split(",");

            var configStrRoles = AuthConfiguration.Get(KeyConfigFakeJwtRole);
            _configRoles = !string.IsNullOrEmpty(configStrRoles) ? configStrRoles.Split(",") : new string[0];
        }

        /// <summary>
        /// Chamado para validar que a origem do pedido é um "client_id" registrado, e que as credenciais corretas para esse cliente são
        /// presente no pedido. Se o aplicativo da Web aceitar credenciais de autenticação básica,
        /// context.TryGetBasicCredentials (out clientId, out clientSecret) pode ser chamado para adquirir esses valores, se presentes no cabeçalho da solicitação. Se a web
        /// application aceita "client_id" e "client_secret" como parâmetros de POST codificados,
        /// context.TryGetFormCredentials (out clientId, out clientSecret) pode ser chamado para adquirir esses valores, se presentes no corpo da solicitação.
        /// Se o context.Validate não for chamado, o pedido não continuará.
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns>Tarefa para ativar a execução assíncrona </returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            _logger.LogDebug("Executando ValidateClientAuthentication...");
            if (_logger.IsEnabled(LogLevel.Trace)) {
                var msg = string.Format("ValidateClientAuthentication Detail - Parameters: {0}",
                    JsonConvert.SerializeObject(context.Parameters)
                );
                _logger.LogTrace(msg);
            }

            string reqOrigin = context.Request.Headers["Origin"];
            context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { reqOrigin });

            string clientId;
            string clientSecret;
            if (context.TryGetBasicCredentials(out clientId, out clientSecret) ||
                context.TryGetFormCredentials(out clientId, out clientSecret))
            {
                if (Array.IndexOf(_configClientIds, clientId) >= 0) {
                    context.HttpContext.Items[Constants.Parameters.ClientId] = clientId;
                    context.Validated();
                } else {
                    context.SetError(Constants.Errors.InvalidClient, "Invalid client_id.");    
                }
            } 
            else {
                context.SetError(Constants.Errors.InvalidClient, "Parameter client_id not informed.");
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Chamado para cada solicitação para o endpoint Token para determinar se a solicitação é válida e deve continuar.
        /// O comportamento padrão ao usar o OAuthAuthorizationServerProvider é assumir solicitações bem formadas, com
        /// validado credenciais do cliente, deve continuar o processamento. Um aplicativo pode adicionar restrições adicionais.
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns>Tarefa para ativar a execução assíncrona</returns>
        public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            _logger.LogDebug("Executando ValidateTokenRequest...");
            if (_logger.IsEnabled(LogLevel.Trace)) {
                var msg = string.Format("ValidateTokenRequest Detail - TokenRequest: {0}",
                    JsonConvert.SerializeObject(context.TokenRequest)
                );
                _logger.LogTrace(msg);
            }
            
            var retorno = Task.FromResult<object>(null);

            if (context.TokenRequest.Parameters.ContainsKey(Constants.Parameters.Password)) {
                string reqPassword = context.TokenRequest.Parameters[Constants.Parameters.Password];
                context.HttpContext.Items[Constants.Parameters.Password] = reqPassword;
            }

            context.Validated();
            
            return retorno;
        }
        
        /// <summary>
        /// Chamado quando um pedido para o terminal Token chega com um "grant_type" de "password". Isso ocorre quando o usuário fornece nome e senha
        /// credenciais diretamente na interface do usuário do aplicativo cliente, e o aplicativo cliente está usando aquelas para adquirir um "access_token" e
        /// opcional "refresh_token". Se o aplicativo da Web suportar o
        /// tipo de concessão de credenciais do proprietário do recurso deve validar o contexto.Nome de usuário e contexto.Palavra-chave, conforme apropriado. Para emitir um
        /// token de acesso o contexto.Validado deve ser chamado com um novo ticket contendo as declarações sobre o proprietário do recurso que devem ser associadas
        /// com o token de acesso. O aplicativo deve tomar as medidas adequadas para garantir que o terminal não seja maltratado por chamadores mal-intencionados.
        /// O comportamento padrão é rejeitar este tipo de concessão.
        /// Veja também http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns>Tarefa para ativar a execução assíncrona</returns>
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            _logger.LogDebug("Executando GrantResourceOwnerCredentials...");

            string username = context.UserName;
            string password = (string) context.HttpContext.Items[Constants.Parameters.Password];

            if (_logger.IsEnabled(LogLevel.Trace)) {
                var msg = string.Format("GrantResourceOwnerCredentials Detail - Username: {0}", username);
                _logger.LogTrace(msg);
            }

            var retorno = Task.FromResult(0);

            if (string.IsNullOrEmpty(username)) {
                context.SetError(Constants.Errors.InvalidUsername, "Parameter username not informed.");
                return retorno;
            }

            if (string.IsNullOrEmpty(password)) {
                context.SetError(Constants.Errors.InvalidPassword, "Parameter password not informed.");
                return retorno;
            }

            var configUsername = AuthConfiguration.Get(KeyConfigFakeJwtUsername);
            var configPassword = AuthConfiguration.Get(KeyConfigFakeJwtPassword);

            var pwdEncrypt = Crypto.Sha256(password);
            if (_logger.IsEnabled(LogLevel.Trace)) {
                var msg = string.Format("Password received: {0}", pwdEncrypt);
                _logger.LogTrace(msg);
            }

            if (string.Equals(username, configUsername) && string.Equals(pwdEncrypt, configPassword)) {

                var clientId = (string) context.HttpContext.Items[Constants.Parameters.ClientId];
                context.Validated(new ClaimsPrincipal(CreateIdentity(username)));
                context.Ticket.Properties.Items[Constants.Parameters.ClientId] = clientId;

            } else {
                context.SetError(Constants.Errors.InvalidCredentials, "Incorrect data user, username or password.");
            }

            return retorno;
        }

        /// <summary>
        /// Chamado quando um pedido para o terminal Token chega com um "grant_type" de "refresh_token". Isso ocorre se seu aplicativo tiver emitido um "refresh_token"
        /// junto com o "access_token", e o cliente está tentando usar o "refresh_token" para adquirir um novo "access_token", e possivelmente um novo "refresh_token".
        /// Para emitir um token de atualização, um Options.RefreshTokenProvider deve ser atribuído para criar o valor que é retornado. As reivindicações e propriedades
        /// associado ao token de atualização estão presentes no contexto.Ticket. O aplicativo deve chamar o contexto.Validado para instruir o
        /// Authorization Server middleware para emitir um token de acesso com base nessas declarações e propriedades. A chamada para o contexto.Validado pode
        /// receber um AuthenticationTicket ou ClaimsIdentity diferentes para controlar quais informações fluem do token de atualização para
        /// o token de acesso. O comportamento padrão ao usar o OAuthAuthorizationServerProvider é para fluir informações do token de atualização para
        /// o token de acesso não modificado.
        /// Veja também http://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns> Tarefa para ativar a execução assíncrona</returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            _logger.LogDebug("Executando GrantRefreshToken...");

            var currentClient = (string) context.HttpContext.Items[Constants.Parameters.ClientId];
            var originalClient = context.Ticket.Properties.Items[Constants.Parameters.ClientId];
            
            var retorno = Task.FromResult<object>(null);

            if (!string.Equals(originalClient, currentClient))
            {
                context.SetError(Constants.Errors.InvalidClient, "Refresh token is issued to a different clientId.");
                return retorno;
            }

            // Change auth ticket for refresh token requests
            var schema = context.Ticket.AuthenticationScheme;
            var newIdentity = new ClaimsIdentity(context.Ticket.Principal.Identity);

            var newClaim = newIdentity.Claims.Where(c => c.Type == "newClaim").FirstOrDefault();
            if (newClaim != null)
            {
                newIdentity.RemoveClaim(newClaim);
            }
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));

            var newTicket = new AuthenticationTicket(
                new ClaimsPrincipal(newIdentity), 
                context.Ticket.Properties, schema
            ); 
            newTicket.Properties.Items[Constants.Parameters.ClientId] = originalClient;

            context.Validated(newTicket);

            return retorno;
        }

        private ClaimsIdentity CreateIdentity(string username) 
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, username));

            foreach (var role in _configRoles) {
                claims.Add(new Claim(GrantRole, role));
            }

            return new ClaimsIdentity(
                new GenericIdentity(username, "Login"),
                claims.ToArray()
            );
        }
        
    }
}