using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuth.AspNet.AuthServer;
using System.Security.Claims;
using System.Security.Principal;
using System.Security.Cryptography;

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
using ONS.AuthProvider.OAuth.Extensions;

namespace ONS.AuthProvider.OAuth.Providers.Fake
{
    /// <summary>Classe que define o formato de geração de token para o 
    /// provedor de autenticação fake.</summary>
    public class FakeJwtFormat : ISecureDataFormat<AuthenticationTicket> 
    {
        private readonly ILogger<FakeJwtFormat> _logger;
        private readonly JwtToken _configToken;

        private readonly SigningCredentials _signingCredentials;

        public FakeJwtFormat(JwtToken configToken) {

            _logger = AuthLoggerFactory.Get<FakeJwtFormat>();
            _configToken = configToken;

            if (_configToken.UseRsa) {
                
                //using(RSA privateRsa = RSA.Create())
                {
                    RSA privateRsa = RSA.Create();
                    var privateKeyXml = File.ReadAllText(_configToken.RsaPrivateKeyXml);
                    RsaExtension.FromXmlString(privateRsa, privateKeyXml);
                    var privateKey = new RsaSecurityKey(privateRsa);
                    
                    _signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
                }
            } else {
                var key = new SymmetricSecurityKey(Convert.FromBase64String(_configToken.SecretKey));
                _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);    
            }
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

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _configToken.Issuer,
                Audience = audience,
                SigningCredentials = _signingCredentials,
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