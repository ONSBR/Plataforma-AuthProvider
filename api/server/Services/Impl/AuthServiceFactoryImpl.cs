using System;
using ONS.AuthProvider.Api.Services.Impl.Pop;
using ONS.AuthProvider.Api.Services.Impl.Fake;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.Api.Services.Impl
{
    public class AuthServiceFactoryImpl : IAuthServiceFactory
    {
        private const string ConfigAuthFactoryServiceAlias = "Auth:Factory:Service.Alias";

        private readonly IConfiguration _configuration;
        
        private readonly IAuthService _popAuthJwtService;
        private readonly IAuthService _fakeAuthJwtService;

        public AuthServiceFactoryImpl(IConfiguration configuration, 
            ILogger<PopAuthJwtService> loggerPop, 
            ILogger<FakeAuthJwtService> loggerFake) 
        {
            _configuration = configuration;
            
            _popAuthJwtService = new PopAuthJwtService(_configuration, loggerPop);
            _fakeAuthJwtService = new FakeAuthJwtService(_configuration, loggerFake);
        }

        public IAuthService Get(string clientId) 
        {    
            IAuthService retorno = null;

            var configTypeService = _getConfigTypeService(clientId);

            if ("pop".Equals(configTypeService)) {
                retorno = _popAuthJwtService;
            }
            else if ("fake".Equals(configTypeService)) {
                retorno = _fakeAuthJwtService;
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
