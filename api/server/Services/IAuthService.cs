using ONS.AuthProvider.Api.Models;

namespace ONS.AuthProvider.Api.Services
{
    /// <summary>Define a interface de serviços de autenticação na plataforma</summary>
    public interface IAuthService
    {
        /// <summary>Autenticação do usuário e geração do token de validade, com expiração.</summary>
        /// <param name="user">Dados do usuário para autenticação.</param>
        /// <returns>Dados do token de autenticação.</returns>
        Token Auth(User user);
        
        /// <summary>Token com expiração atualizada, de validade de autenticação.</summary>
        /// <param name="dataRefresh">Dados para atualização do token.</param>
        /// <returns>Dados do token atualizado para nova expiração.</returns>
        Token Refresh(DataRefreshToken dataRefresh);

    }
}