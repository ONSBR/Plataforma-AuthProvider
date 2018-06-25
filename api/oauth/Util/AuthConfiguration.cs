using System.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace ONS.AuthProvider.OAuth.Util
{   
    ///<summary>Classe de configurações do sistema de autenticação.</summary>
    public class AuthConfiguration {

        public static ILogger<AuthConfiguration> Logger {get;set;}

        public static IConfiguration Configuration {get;set;}
            
        ///<summary>Método utilizado para obter um determinado parâmetro de configuração.</summary>
        ///<param name="chave">Chave de identificação da configuração.</param>
        ///<param name="required">Indica se o parâmetro é obrigatório.</param>
        ///<returns>Valor do parâmetro configurado no sistema.</returns>
        public static string Get(string chave, bool required = true) 
        {
            var retorno = Configuration[chave];

            if (required && string.IsNullOrEmpty(retorno)) {
                var msg = string.Format("Configuration Not found. Chave={0}", chave);
                Logger.LogDebug(msg);
                throw new System.Exception(msg);
            }
            
            return retorno;
        }

    }
}