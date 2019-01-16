using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OAuth.AspNet.AuthServer;
using ONS.AuthProvider.Common.Providers;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.Adapter.Pop.Providers
{
    /// <summary>
    ///     Provedor do servidor para se comunicar com o aplicativo da Web durante
    ///     o processamento de solicitações.
    /// </summary>
    public class PopAuthorizationProvider : OAuthAuthorizationServerProvider
    {
        private readonly ILogger<PopAuthorizationProvider> _logger;
        private readonly PopAuthJwtService _popService;

        public PopAuthorizationProvider(JwtToken configToken)
        {
            _logger = AuthLoggerFactory.Get<PopAuthorizationProvider>();
            _popService = new PopAuthJwtService(configToken);
        }

        /// <summary>
        ///     Chamado para validar que a origem do pedido é um "client_id" registrado, e que as credenciais corretas para esse
        ///     cliente são
        ///     presente no pedido. Se o aplicativo da Web aceitar credenciais de autenticação básica,
        ///     context.TryGetBasicCredentials (out clientId, out clientSecret) pode ser chamado para adquirir esses valores, se
        ///     presentes no cabeçalho da solicitação. Se a web
        ///     application aceita "client_id" e "client_secret" como parâmetros de POST codificados,
        ///     context.TryGetFormCredentials (out clientId, out clientSecret) pode ser chamado para adquirir esses valores, se
        ///     presentes no corpo da solicitação.
        ///     Se o context.Validate não for chamado, o pedido não continuará.
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns>Tarefa para ativar a execução assíncrona </returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            _logger.LogDebug("Executando ValidateClientAuthentication...");
            if (_logger != null && _logger.IsEnabled(LogLevel.Trace))
            {
                var msg = string.Format("ValidateClientAuthentication Detail - Parameters: {0}",
                    JsonConvert.SerializeObject(context.Parameters)
                );
                _logger.LogTrace(msg);
            }

            var retorno = Task.FromResult(0);

            var parameters = new Dictionary<string, string>();
            foreach (var item in context.Parameters)
            {
                string value = item.Value;
                if (!string.IsNullOrEmpty(value)) parameters[item.Key] = value;
            }

            var hostOrigin = context.Request.Headers["Origin"];
            var cookieValue = context.Request.Headers["Cookie"];

            try
            {
                var popToken = _popService.RequestOAuth(parameters, hostOrigin, cookieValue);

                context.Validated();
                context.HttpContext.Items[Constants.ResponseTypes.Token] = popToken;
            }
            catch (PopException pex)
            {
                context.SetError(pex.Error, pex.ErrorDescription);
            }

            return retorno;
        }

        /// <summary>
        ///     Chamado para cada solicitação para o endpoint Token para determinar se a solicitação é válida e deve continuar.
        ///     O comportamento padrão ao usar o OAuthAuthorizationServerProvider é assumir solicitações bem formadas, com
        ///     validado credenciais do cliente, deve continuar o processamento. Um aplicativo pode adicionar restrições
        ///     adicionais.
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns>Tarefa para ativar a execução assíncrona</returns>
        public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            _logger.LogDebug("Executando ValidateTokenRequest...");
            if (_logger != null && _logger.IsEnabled(LogLevel.Trace))
            {
                var msg = string.Format("ValidateTokenRequest Detail - TokenRequest: {0}",
                    JsonConvert.SerializeObject(context.TokenRequest)
                );
                _logger.LogTrace(msg);
            }

            var retorno = Task.FromResult<object>(null);

            context.Validated();

            return retorno;
        }

        /// <summary>
        ///     Chamado quando um pedido para o terminal Token chega com um "grant_type" de "password". Isso ocorre quando o
        ///     usuário fornece nome e senha
        ///     credenciais diretamente na interface do usuário do aplicativo cliente, e o aplicativo cliente está usando aquelas
        ///     para adquirir um "access_token" e
        ///     opcional "refresh_token". Se o aplicativo da Web suportar o
        ///     tipo de concessão de credenciais do proprietário do recurso deve validar o contexto.Nome de usuário e
        ///     contexto.Palavra-chave, conforme apropriado. Para emitir um
        ///     token de acesso o contexto.Validado deve ser chamado com um novo ticket contendo as declarações sobre o
        ///     proprietário do recurso que devem ser associadas
        ///     com o token de acesso. O aplicativo deve tomar as medidas adequadas para garantir que o terminal não seja
        ///     maltratado por chamadores mal-intencionados.
        ///     O comportamento padrão é rejeitar este tipo de concessão.
        ///     Veja também http://tools.ietf.org/html/rfc6749#section-4.3.2
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns>Tarefa para ativar a execução assíncrona</returns>
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            _logger.LogDebug("Executando GrantResourceOwnerCredentials...");

            var username = context.UserName;

            if (_logger != null && _logger.IsEnabled(LogLevel.Trace))
            {
                var msg = string.Format("GrantResourceOwnerCredentials Detail - Username: {0}", username);
                _logger.LogTrace(msg);
            }

            var retorno = Task.FromResult(0);

            context.Validated(new ClaimsPrincipal());

            return retorno;
        }

        /// <summary>
        ///     Chamado quando um pedido para o terminal Token chega com um "grant_type" de "refresh_token". Isso ocorre se seu
        ///     aplicativo tiver emitido um "refresh_token"
        ///     junto com o "access_token", e o cliente está tentando usar o "refresh_token" para adquirir um novo "access_token",
        ///     e possivelmente um novo "refresh_token".
        ///     Para emitir um token de atualização, um Options.RefreshTokenProvider deve ser atribuído para criar o valor que é
        ///     retornado. As reivindicações e propriedades
        ///     associado ao token de atualização estão presentes no contexto.Ticket. O aplicativo deve chamar o contexto.Validado
        ///     para instruir o
        ///     Authorization Server middleware para emitir um token de acesso com base nessas declarações e propriedades. A
        ///     chamada para o contexto.Validado pode
        ///     receber um AuthenticationTicket ou ClaimsIdentity diferentes para controlar quais informações fluem do token de
        ///     atualização para
        ///     o token de acesso. O comportamento padrão ao usar o OAuthAuthorizationServerProvider é para fluir informações do
        ///     token de atualização para
        ///     o token de acesso não modificado.
        ///     Veja também http://tools.ietf.org/html/rfc6749#section-6
        /// </summary>
        /// <param name="context"> O contexto do evento traz informações e resultados.</param>
        /// <returns> Tarefa para ativar a execução assíncrona</returns>
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            _logger.LogDebug("Executando GrantRefreshToken...");

            context.Validated();

            return Task.FromResult<object>(null);
        }
    }
}