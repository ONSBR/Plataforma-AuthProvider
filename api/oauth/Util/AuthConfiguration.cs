using System.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

using ONS.AuthProvider.OAuth.Exception;

namespace ONS.AuthProvider.OAuth.Util
{   
    public class AuthConfiguration {

        public static ILogger<AuthConfiguration> Logger {get;set;}

        public static IConfiguration Configuration {get;set;}
            

        public static string Get(string chave, bool required = true) {
            var retorno = Configuration[chave];
            if (required && string.IsNullOrEmpty(retorno)) {
                var msg = string.Format("Configuration Not found. Chave={0}", chave);
                Logger.LogDebug(msg);
                throw new AuthException(msg);
            }
            return retorno;
        }

    }
}