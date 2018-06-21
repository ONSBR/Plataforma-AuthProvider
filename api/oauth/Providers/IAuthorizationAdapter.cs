using OAuth.AspNet.AuthServer;

namespace ONS.AuthProvider.OAuth.Providers
{
    public interface IAuthorizationAdapter {
        
        void SetConfiguration(OAuthAuthorizationServerOptions options);
    }

}