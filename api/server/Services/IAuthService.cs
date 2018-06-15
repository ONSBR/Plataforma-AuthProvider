using ONS.AuthProvider.Api.Models;

namespace ONS.AuthProvider.Api.Services
{
    public interface IAuthService
    {

        Token Auth(User user);
        
        Token Refresh(DataRefreshToken dataRefresh);

    }
}