using System;
using ONS.AuthProvider.Api.Services.Impl.Pop;
using ONS.AuthProvider.Api.Services.Impl.Fake;
using ONS.AuthProvider.Api.Exception;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.Api.Services.Impl
{
    /// <summary>Fábrica de serviço de autenticação na plataforma</summary>
    public class AuthServiceFactoryImpl : IAuthServiceFactory
    {
        private const string ConfigAuthFactoryServiceAlias = "Auth:Factory:Service.Alias";

        private readonly IConfiguration _configuration;
        
        private readonly ILogger _logger;

        private readonly IAuthService _popAuthJwtService;
        private readonly IAuthService _fakeAuthJwtService;

        public AuthServiceFactoryImpl(IConfiguration configuration, 
            ILogger<AuthServiceFactoryImpl> logger, 
            ILogger<PopAuthJwtService> loggerPop, 
            ILogger<FakeAuthJwtService> loggerFake) 
        {
            _configuration = configuration;

            _logger = logger;
            
            _popAuthJwtService = new PopAuthJwtService(_configuration, loggerPop);
            _fakeAuthJwtService = new FakeAuthJwtService(_configuration, loggerFake);
        }

        /// <summary>Obtém o tipo de serviço de autenticação para o clientId.</summary>
        /// <param name="clientId">Indica o clientId para o tipo de serviço.</param>
        /// <returns>Tipo do serviço de autenticação para o tipo do clientId.</returns>
        public IAuthService Get(string clientId) 
        {    
            IAuthService retorno = null;

            var configTypeService = _getConfigTypeService(clientId);

            if ("pop".Equals(configTypeService)) {
                retorno = _popAuthJwtService;
            }
            else if ("fake".Equals(configTypeService)) {
                retorno = _fakeAuthJwtService;
            } else {
                _logger.LogError(string.Format(
                    "Erro de configuração de serviço para o clientId: {0}, configTypeService: {1}.", 
                    clientId, configTypeService
                ));
                throw new AuthException("Unexpected error has occurred.");
            }

            return retorno;
        }

        private string _getConfigTypeService(string clientId) 
        {
            var retorno = _configuration[ConfigAuthFactoryServiceAlias];
            
            if (!string.IsNullOrEmpty(clientId)) {
              var url = _configuration[string.Format("{0}.{1}", ConfigAuthFactoryServiceAlias, clientId)];
              if (!string.IsNullOrEmpty(url)) { 
                  retorno = url;
              }
            } 
            
            return retorno;
        }
    }
}
