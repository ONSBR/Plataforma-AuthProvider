using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuth.AspNet.AuthServer;
using System.Security.Claims;
using System.Security.Principal;
using System.Collections.Concurrent;

using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

using ONS.AuthProvider.OAuth.Util;
using Microsoft.AspNetCore.Authentication;


namespace ONS.AuthProvider.OAuth.Providers.Fake
{
    /// <summary>Classe que define o formato de geração de token para o 
    /// provedor de autenticação fake.</summary>
    public class FakeJwtFormat : ISecureDataFormat<AuthenticationTicket> 
    {
        private const string KeyConfigFakeJwtSecret = "Auth:Server:Adapters:Fake:Jwt.Token:Key";
        private const string KeyConfigFakeJwtIssuer = "Auth:Server:Adapters:Fake:Jwt.Token:Issuer";
        
        private readonly ILogger<FakeJwtFormat> _logger;

        private readonly string _issuer;
        private readonly string _configKey;

        public FakeJwtFormat() {
            _logger = AuthLoggerFactory.Get<FakeJwtFormat>();
            _issuer = AuthConfiguration.Get(KeyConfigFakeJwtIssuer);
            _configKey = AuthConfiguration.Get(KeyConfigFakeJwtSecret);
        }

        ///<summary>Método responsável por gerar o token no formato Jwt para 
        /// o ticket com dados da solicitação de autenticação.</summary>
        ///<param name="data">Dados de ticket da solicitação.</param>
        ///<returns>Token de autenticação.</returns>
        public string Protect(AuthenticationTicket data)
        {
            if (data == null) {
                throw new ArgumentNullException("data");
            }

            var audience = data.Properties.Items[Constants.Parameters.ClientId];
            
            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");
            
            var identity = (ClaimsIdentity) data.Principal.Identity;
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            if (_logger.IsEnabled(LogLevel.Trace)) {
                _logger.LogTrace(string.Format("Generating token with issued: {0}, expires:{1}.", issued, expires));
            }
            
            var key = new SymmetricSecurityKey(Convert.FromBase64String(_configKey));
            var signingKey = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _issuer,
                Audience = audience,
                SigningCredentials = signingKey,
                Subject = identity,
                NotBefore = issued.Value.UtcDateTime,
                Expires = expires.Value.UtcDateTime
            });
            
            var jwt = handler.WriteToken(securityToken);

            return jwt;
        }

        public string Protect(AuthenticationTicket data, string purpose) 
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket Unprotect(string protectedText, string value)
        {
            throw new NotImplementedException();
        }
    }
}