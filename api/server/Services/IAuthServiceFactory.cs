using ONS.AuthProvider.Api.Models;

namespace ONS.AuthProvider.Api.Services
{
    public interface IAuthServiceFactory
    {
        IAuthService Get(string clientId);
    }
}