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

        public PopAuthenticationTokenProvider(JwtToken configToken): base() {
            _logger = AuthLoggerFactory.Get<PopAuthenticationTokenProvider>();
            _configToken = configToken;
            OnCreate = CreateAuthenticationCode;
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
                var securityTokenPop = _getValidateSecurityTokenPop(handler, accessToken);

                accessToken = _createTokenRsa(handler, securityTokenPop);
            } 
            
            context.SetToken(accessToken);
            
            var utcNow = DateTime.UtcNow;
            context.Ticket.Properties.IssuedUtc = utcNow;
            context.Ticket.Properties.ExpiresUtc = utcNow.AddSeconds(token.ExpiresIn);
        }

        private string _createTokenRsa(JwtSecurityTokenHandler handler, JwtSecurityToken securityTokenPop) 
        {    
            SigningCredentials signingKey;
                    
            using(RSA privateRsa = RSA.Create())
            {
                var privateKeyXml = File.ReadAllText(_configToken.RsaPrivateKeyXml);
                RsaExtension.FromXmlString(privateRsa, privateKeyXml);
                var privateKey = new RsaSecurityKey(privateRsa);
                
                signingKey = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
            }

            JwtHeader header = new JwtHeader(signingKey);

            var payload = securityTokenPop.Payload;
            string rawHeader = header.Base64UrlEncode();
            string rawPayload = payload.Base64UrlEncode();
            string rawSignature = _createEncodedSignature(string.Concat(rawHeader, ".", rawPayload), signingKey);

            var securityToken = new JwtSecurityToken(header, payload, rawHeader, rawPayload, rawSignature);
            
            return handler.WriteToken(securityToken);
        }

        private JwtSecurityToken _getValidateSecurityTokenPop(JwtSecurityTokenHandler handler, string accessToken) {
            
            var tokenValidationParameters = new TokenValidationParameters
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
            
            SecurityToken retorno;
            handler.ValidateToken(accessToken, tokenValidationParameters, out retorno);
            
            return (JwtSecurityToken) retorno;
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