using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OAuth.AspNet.AuthServer;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Authentication;
using ONS.AuthProvider.OAuth.Util;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ONS.AuthProvider.OAuth.Extensions;

using Newtonsoft.Json;

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    /// <summary>Provedor do servidor para se comunicar com o aplicativo da Web durante o 
    /// processamento de solicitações, com autenticação no POP.</summary>
    public class PopAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        private readonly ILogger _logger;
        private readonly JwtToken _configToken;

        // Configuração de RSA
        private JwtHeader _rsaJwtHeader;
        private string _rsaRawHeader;
        private SigningCredentials _rsaSigningCredentials;
        
        private TokenValidationParameters _popTokenValidationParameters;

        public PopAuthenticationTokenProvider(JwtToken configToken): base() {
            _logger = AuthLoggerFactory.Get<PopAuthenticationTokenProvider>();
            _configToken = configToken;
            OnCreate = CreateAuthenticationCode;

            if (_configToken.UseRsa) {
                
                _loadRsaConfiguration();
                _loadPopTokenValidation();
            }
        }

        private void _loadRsaConfiguration() 
        {
            using(RSA privateRsa = RSA.Create())
            {
                var privateKeyXml = File.ReadAllText(_configToken.RsaPrivateKeyXml);
                RsaExtension.FromXmlString(privateRsa, privateKeyXml);
                var privateKey = new RsaSecurityKey(privateRsa);
                
                _rsaSigningCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
            }
            _rsaJwtHeader = new JwtHeader(_rsaSigningCredentials);
            _rsaRawHeader = _rsaJwtHeader.Base64UrlEncode();
        }

        private void _loadPopTokenValidation() 
        {
            _popTokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,

                RequireExpirationTime = true, 
                RequireSignedTokens = true, 
                ClockSkew = TimeSpan.Zero,

                ValidIssuer = _configToken.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_configToken.PopSecretKey))
            };
        }

        /// <summary>Cria o o token do parâmetro access_token. No caso obtém da resposta do POP.</summary>
        public void CreateAuthenticationCode(AuthenticationTokenCreateContext context)
        {
            PopToken token = (PopToken) context.HttpContext.Items[Constants.ResponseTypes.Token];
            
            string accessToken = token.AccessToken;

            if (_configToken.UseRsa) {

                if (_logger.IsEnabled(LogLevel.Trace)) {
                    var msg = string.Format("CreateAuthenticationCode POP - AccessToken: {0}", accessToken);
                    _logger.LogTrace(msg);
                }

                var handler = new JwtSecurityTokenHandler();

                SecurityToken securityTokenPop;
                handler.ValidateToken(accessToken, _popTokenValidationParameters, out securityTokenPop);

                accessToken = _createTokenRsa(handler, (JwtSecurityToken) securityTokenPop);
            } 
            
            context.SetToken(accessToken);
            
            var utcNow = DateTime.UtcNow;
            context.Ticket.Properties.IssuedUtc = utcNow;
            context.Ticket.Properties.ExpiresUtc = utcNow.AddSeconds(token.ExpiresIn);
        }

        private string _createTokenRsa(JwtSecurityTokenHandler handler, JwtSecurityToken securityTokenPop) 
        {    
            var payload = securityTokenPop.Payload;
            
            string rawPayload = payload.Base64UrlEncode();
            string rawSignature = _createEncodedSignature(string.Concat(_rsaRawHeader, ".", rawPayload), _rsaSigningCredentials);

            var securityToken = new JwtSecurityToken(_rsaJwtHeader, payload, _rsaRawHeader, rawPayload, rawSignature);
            
            return handler.WriteToken(securityToken);
        }

        private string _createEncodedSignature(string input, SigningCredentials signingCredentials)
        {
            var cryptoProviderFactory = signingCredentials.CryptoProviderFactory ?? signingCredentials.Key.CryptoProviderFactory;
            var signatureProvider = cryptoProviderFactory.CreateForSigning(signingCredentials.Key, signingCredentials.Algorithm);
            
            try
            {
                return Base64UrlEncoder.Encode(signatureProvider.Sign(Encoding.UTF8.GetBytes(input)));
            }
            finally
            {
                cryptoProviderFactory.ReleaseSignatureProvider(signatureProvider);
            }
        }

    }
}