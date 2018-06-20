using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;

namespace ONS.AuthProvider.Validator
{
    /// <summary>Classe com as opções de validação da autenticação.</summary>
    public class AuthValidateOptions
    {
        public bool ValidateIssuer {get;set;} = true;
        public bool ValidateAudience {get;set;} = true;
        public bool ValidateLifetime {get;set;} = true;
        public bool ValidateIssuerSigningKey {get;set;} = true;

        public bool RequireExpirationTime {get;set;} = true;
        public bool RequireSignedTokens {get;set;} = true;

        public string ValidIssuer {get;set;}
        public string ValidAudience {get;set;}
        public string ValidKey {get;set;}

        public ILogger Logger {get;set;}

        public override string ToString() {
            return string.Format(
                "{0} {{ ValidIssuer={1}, ValidAudience={2}, ValidKey={3} }}", 
                this.GetType().Name, ValidIssuer, ValidAudience, ValidKey
            );
        }
    }

}
