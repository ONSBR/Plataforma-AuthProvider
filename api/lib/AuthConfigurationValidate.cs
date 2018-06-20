using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace ONS.AuthProvider.Validator
{
    /// <summary>Classe de validação dos dados de autenticação nas requisições ao sistema.</summary>
    public static class AuthConfigurationValidate
    {
        private const string ConfigPathAuthIssuer = "Auth:Validate:Issuer";
        private const string ConfigPathAuthAudience = "Auth:Validate:Audience";
        private const string ConfigPathAuthKey = "Auth:Validate:Key";

        /// <summary>Realiza as configurações necessárias para realizar validação de 
        /// autenticação e permissão das requisições de acesso aos recursos do sistema.</summary>
        /// <param name="services">Provedor de serviços para configuração.</param>
        /// <param name="configuration">Provedor de configurações para a validação.</param>
        public static void Configure(IServiceCollection services, IConfiguration configuration) {

            var issuer = configuration[ConfigPathAuthIssuer];
            var audience = configuration[ConfigPathAuthAudience];
            var key = configuration[ConfigPathAuthKey];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(key))
                };
            });

            /* TODO rever validação
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Member",
                    policy => policy.RequireClaim("MembershipId"));
            });*/

        }
    }
}
