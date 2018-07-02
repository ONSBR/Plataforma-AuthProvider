using System;
using System.Collections.Generic;
using System.Linq;

using ONS.AuthProvider.OAuth.Util;
using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.OAuth.Providers
{
    ///<summary>Fábrica de adaptadores de providers de autenticação OAuth.</summary>
    public class AuthorizationAdapterFactory 
    {
        private const string KeyConfigAuthServerAdapterUsage = "Auth:Server:AdapterUsage";
        private const string KeyConfigAuthServerAdapters = "Auth:Server:Adapters";
        private const string KeyConfigAuthServerAdapterSufix = "Type";

        private const string MsgNotFoundFormat = "Configuration not found. path: {0}";

        private static ILogger<AuthorizationAdapterFactory> _logger = AuthLoggerFactory.Get<AuthorizationAdapterFactory>(); 

        private static string _getAdapterType() 
        {
            var configAuthServerAdapterUsage = System.Environment.GetEnvironmentVariable("AdapterUsage");
            if (string.IsNullOrEmpty(configAuthServerAdapterUsage)) {
                configAuthServerAdapterUsage = AuthConfiguration.Get(KeyConfigAuthServerAdapterUsage);
            }
            
            var keyConfigAdapterType = string.Format(
                "{0}:{1}:{2}", 
                KeyConfigAuthServerAdapters, configAuthServerAdapterUsage, 
                KeyConfigAuthServerAdapterSufix
            );
            
            return AuthConfiguration.Get(keyConfigAdapterType);
        }

        ///<summary>Método para obter o adaptador configurado para o serviço de autenticação.</summary>
        ///<returns>Retonna o adaptador configurado..</returns>
        public static IAuthorizationAdapter Use() {
            
            var typeStr = _getAdapterType();
            
            try {
                var type = Type.GetType(typeStr);
                if (type != null) {
                    return (IAuthorizationAdapter) Activator.CreateInstance(type);
                } else {
                    var msg = string.Format("System can't load adapter type. type: {0}", typeStr);
                    _logger.LogError(msg);
                    throw new Exception(msg);
                }
            } catch(System.Exception ex) {
                var msg = string.Format("Error to instance adapter type. type: {0}", typeStr);
                _logger.LogError(msg, ex);
                throw new Exception(msg, ex);
            }
        }
    }

}