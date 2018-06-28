using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ONS.AuthProvider.Validator
{
    /// <summary>Classe de validação dos dados de autenticação nas requisições ao sistema.</summary>
    public static class AuthConfigurationValidate
    {

        /// <summary>Realiza as configurações necessárias para realizar validação de 
        /// autenticação e permissão das requisições de acesso aos recursos do sistema.</summary>
        /// <param name="services">Provedor de serviços para configuração.</param>
        /// <param name="options">Opções de validação.</param>
        public static void Configure(IServiceCollection services, AuthValidateOptions options) 
        {
            var optionsEvents = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (options.Logger != null && options.Logger.IsEnabled(LogLevel.Debug)) {
                        options.Logger.LogDebug("OnAuthenticationFailed: " + context.Exception.Message);
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    if (options.Logger != null && options.Logger.IsEnabled(LogLevel.Debug)) {
                        options.Logger.LogDebug("OnTokenValidated: " + context.SecurityToken);
                    }
                    return Task.CompletedTask;
                }
            };

            SecurityKey issuerSigningKey;
            if (options.UseRsa) {
                RSA publicRsa = RSA.Create();
                var publicKeyXml = File.ReadAllText(options.FileRsaPublicKeyXml);
                RsaExtension.FromXmlString(publicRsa, publicKeyXml);
                issuerSigningKey = new RsaSecurityKey(publicRsa);
            } else {
                issuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(options.ValidSecretKey));
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = options.ValidateIssuer,
                ValidateAudience = options.ValidateAudience,
                ValidateLifetime = options.ValidateLifetime,
                ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,

                RequireExpirationTime = options.RequireExpirationTime, 
                RequireSignedTokens = options.RequireSignedTokens, 
                ClockSkew = TimeSpan.Zero,

                ValidIssuer = options.ValidIssuer,
                ValidAudience = options.ValidAudience,
                IssuerSigningKey = issuerSigningKey
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(optionsToken => {
                optionsToken.TokenValidationParameters = tokenValidationParameters;
                optionsToken.Events = optionsEvents;
            });
        }
    }
}
