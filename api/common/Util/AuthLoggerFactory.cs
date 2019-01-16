using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.Common.Util
{
    ///<summary>Classe de fabricação de logger para registros de logs para o sistema.</summary>
    public class AuthLoggerFactory
    {
        public static ILoggerFactory LoggerFactory { get; set; }

        ///<summary>Método para obter o Logger para um tipo informado, para registro de logs.</summary>
        ///<returns>Logger para o tipo informado.</returns>
        public static ILogger<T> Get<T>()
        {
            return LoggerFactory.CreateLogger<T>();
        }
    }
}