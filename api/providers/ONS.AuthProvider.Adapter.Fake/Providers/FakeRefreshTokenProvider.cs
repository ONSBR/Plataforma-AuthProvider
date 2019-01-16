using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OAuth.AspNet.AuthServer;
using ONS.AuthProvider.Common.Providers;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.Adapter.Fake.Providers
{
    ///<summary>Provedor de geração e obtenção do token de atualização de autenticação.</summary>
    public class FakeRefreshTokenProvider : IAuthenticationTokenProvider
    {
        private readonly ILogger _logger;

        public FakeRefreshTokenProvider()
        {
            _logger = AuthLoggerFactory.Get<FakeRefreshTokenProvider>();
        }

        /// <summary>Método para criar o token de atualização de autenticação.</summary>
        /// <param name="contexto">Dados da solicitação para geração do token.</param>
        /// <returns>Tarefa para ativar a execução assíncrona </returns>
        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            _logger.LogDebug("Executando CreateAsync...");

            var retorno = Task.FromResult<object>(null);

            var refreshToken = Crypto.Sha256(Guid.NewGuid().ToString("n"));

            var serializedTicket = context.SerializeTicket();

            CacheManager.Cache.Set(refreshToken, serializedTicket);

            context.SetToken(refreshToken);

            if (_logger.IsEnabled(LogLevel.Debug))
                _logger.LogDebug(string.Format("Generated Refresh token. toke: {0}", refreshToken));

            return retorno;
        }

        /// <summary>Método para recuperar o token de atualização de autenticação.</summary>
        /// <param name="contexto">Dados da solicitação para geração do token.</param>
        /// <returns>Tarefa para ativar a execução assíncrona </returns>
        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            _logger.LogDebug("Executando ReceiveAsync...");

            var clientId = (string) context.HttpContext.Items[Constants.Parameters.ClientId];

            var retorno = Task.FromResult<object>(null);

            var refreshToken = context.Token;

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var contentTicket = CacheManager.Cache.Get<string>(refreshToken);

                if (!string.IsNullOrEmpty(contentTicket))
                {
                    context.DeserializeTicket(contentTicket);
                    CacheManager.Cache.Remove(refreshToken);
                }
            }

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