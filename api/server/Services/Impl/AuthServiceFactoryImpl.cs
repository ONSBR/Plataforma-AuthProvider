using System;
using ONS.AuthProvider.Api.Services.Impl.Pop;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.Api.Services.Impl
{
    public class AuthServiceFactoryImpl : IAuthServiceFactory
    {
        private const string ConfigAuthFactoryServiceAlias = "Auth:Factory:Service.Alias";

        private readonly IAuthService _popAuthJwtService;
        private readonly IConfiguration _configuration;

        public AuthServiceFactoryImpl(IConfiguration configuration, ILogger<PopAuthJwtService> logger) 
        {
            _configuration = configuration;
            _popAuthJwtService = new PopAuthJwtService(_configuration, logger);
            // Implementar opção sem ir no pop
        }

        public IAuthService Get(string clientId) 
        {    
            IAuthService retorno = null;

            // TODO falta o defualt
            if ("pop".Equals(_getConfigTypeService(clientId))) {
                retorno = _popAuthJwtService;
            }

            // TODO colocar validação quando não encontrado e se utilizou o deault logar aqui
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
