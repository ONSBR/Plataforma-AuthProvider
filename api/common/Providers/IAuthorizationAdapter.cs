using Microsoft.AspNetCore.Builder;

namespace ONS.AuthProvider.Common.Providers
{
    ///<summary>Interface que define o adaptador de configuração de providers do OAuth.</summary>
    public interface IAuthorizationAdapter
    {
        ///<summary>Método responsável pela configuração do adaptador de providers.</summary>
        ///<param name="app">Aplicação de autenticação.</param>
        void ConfigureApp(IApplicationBuilder app);
    }
}