using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using ONS.AuthProvider.Common.Extensions;
using ONS.AuthProvider.Common.Providers;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.Adapter.Fake.Providers
{
    /// <summary>
    ///     Classe que define o formato de geração de token para o
    ///     provedor de autenticação fake.
    /// </summary>
    public class FakeJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private readonly JwtToken _configToken;
        private readonly ILogger<FakeJwtFormat> _logger;

        private readonly SigningCredentials _signingCredentials;

        public FakeJwtFormat(JwtToken configToken)
        {
            _logger = AuthLoggerFactory.Get<FakeJwtFormat>();
            _configToken = configToken;

            if (_configToken.UseRsa)
            {
                var privateRsa = RSA.Create();
                var privateKeyXml = File.ReadAllText(_configToken.RsaPrivateKeyXml);
                privateRsa.FromXmlContent(privateKeyXml);
                var privateKey = new RsaSecurityKey(privateRsa);

                _signingCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
            }
            else
            {
                var key = new SymmetricSecurityKey(Convert.FromBase64String(_configToken.SecretKey));
                _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            }
        }

        /// <summary>
        ///     Método responsável por gerar o token no formato Jwt para
        ///     o ticket com dados da solicitação de autenticação.
        /// </summary>
        /// <param name="data">Dados de ticket da solicitação.</param>
        /// <returns>Token de autenticação.</returns>
        public string Protect(AuthenticationTicket data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var audience = data.Properties.Items[Constants.Parameters.ClientId];

            if (string.IsNullOrWhiteSpace(audience))
                throw new InvalidOperationException("AuthenticationTicket.Properties does not include audience");

            var identity = (ClaimsIdentity) data.Principal.Identity;
            var issued = data.Properties.IssuedUtc;
            var expires = data.Properties.ExpiresUtc;

            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace(string.Format("Generating token with issued: {0}, expires:{1}.", issued, expires));

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