using ONS.AuthProvider.Api.Models;

namespace ONS.AuthProvider.Api.Services
{
    /// <summary>Define a interface de fábrica de serviço de autenticação na plataforma</summary>
    public interface IAuthServiceFactory
    {
        /// <summary>Obtém o tipo de serviço de autenticação para o clientId.</summary>
        /// <param name="clientId">Indica o clientId para o tipo de serviço.</param>
        /// <returns>Tipo do serviço de autenticação para o tipo do clientId.</returns>
        IAuthService Get(string clientId);
    }
}