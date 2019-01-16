using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OAuth.AspNet.AuthServer;
using ONS.AuthProvider.Common.Extensions;
using ONS.AuthProvider.Common.Providers;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.Adapter.Pop.Providers
{
    /// <summary>
    ///     Provedor do servidor para se comunicar com o aplicativo da Web durante o
    ///     processamento de solicitações, com autenticação no POP.
    /// </summary>
    public class PopAuthenticationTokenProvider : AuthenticationTokenProvider
    {
        private readonly JwtToken _configToken;
        private readonly ILogger _logger;

        private TokenValidationParameters _popTokenValidationParameters;

        // Configuração de RSA
        private JwtHeader _rsaJwtHeader;
        private string _rsaRawHeader;
        private SigningCredentials _rsaSigningCredentials;

        public PopAuthenticationTokenProvider(JwtToken configToken)
        {
            _logger = AuthLoggerFactory.Get<PopAuthenticationTokenProvider>();
            _configToken = configToken;
            OnCreate = CreateAuthenticationCode;

            if (_configToken.UseRsa)
            {
                _loadRsaConfiguration();
                if (_configToken.ValidatePopAccessToken) _loadPopTokenValidation();
            }
        }

        private void _loadRsaConfiguration()
        {
            var privateRsa = RSA.Create();

            var privateKeyXml = File.ReadAllText(_configToken.RsaPrivateKeyXml);
            privateRsa.FromXmlContent(privateKeyXml);
            var privateKey = new RsaSecurityKey(privateRsa);

            _rsaSigningCredentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);

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
            var token = (PopToken) context.HttpContext.Items[Constants.ResponseTypes.Token];

            var accessToken = token.AccessToken;

            if (_configToken.UseRsa)
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    var msg = string.Format("CreateAuthenticationCode POP - AccessToken: {0}", accessToken);
                    _logger.LogTrace(msg);
                }

                var handler = new JwtSecurityTokenHandler();

                SecurityToken securityTokenPop;
                if (_configToken.ValidatePopAccessToken)
                    handler.ValidateToken(accessToken, _popTokenValidationParameters, out securityTokenPop);
                else
                    securityTokenPop = handler.ReadJwtToken(accessToken);

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

            var rawPayload = payload.Base64UrlEncode();
            var rawSignature =
                _createEncodedSignature(string.Concat(_rsaRawHeader, ".", rawPayload), _rsaSigningCredentials);

            var securityToken = new JwtSecurityToken(_rsaJwtHeader, payload, _rsaRawHeader, rawPayload, rawSignature);

            return handler.WriteToken(securityToken);
        }

        private string _createEncodedSignature(string input, SigningCredentials signingCredentials)
        {
            var cryptoProviderFactory =
                signingCredentials.CryptoProviderFactory ?? signingCredentials.Key.CryptoProviderFactory;
            var signatureProvider =
                cryptoProviderFactory.CreateForSigning(signingCredentials.Key, signingCredentials.Algorithm);

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