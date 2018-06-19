using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using ONS.AuthProvider.Api.Models;
using ONS.AuthProvider.Api.Services;
using ONS.AuthProvider.Api.Exception;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Principal;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

using ONS.AuthProvider.Api.Utils.Http;

using Microsoft.AspNetCore.Http;

namespace ONS.AuthProvider.Api.Services.Impl.Fake
{
    /// <summary>Implementação do serviço de autenticação de forma simulada com tecnologia JWT.</summary>
    public class FakeAuthJwtService : IAuthService
    {
        private const string ConfigFakeJwtUsername = "Auth:Fake:Jwt.Username";
        private const string ConfigFakeJwtPassword = "Auth:Fake:Jwt.Password";
        private const string ConfigFakeJwtKey = "Auth:Fake:Jwt.Token:Key";
        private const string ConfigFakeJwtAudience = "Auth:Fake:Jwt.Token:Audience";
        private const string ConfigFakeJwtIssuer = "Auth:Fake:Jwt.Token:Issuer";
        private const string ConfigFakeJwtExpirationSeconds = "Auth:Fake:Jwt.Token:Expiration.Seconds";

        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        private readonly Dictionary<string,string> _cache = new Dictionary<string,string>();
        
        public FakeAuthJwtService(IConfiguration configuration, ILogger<FakeAuthJwtService> logger) {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>Autenticação do usuário e geração do token de validade, com expiração.</summary>
        /// <param name="user">Dados do usuário para autenticação.</param>
        /// <returns>Dados do token de autenticação.</returns>
        public Token Auth(User user)
        {
            var username = _getConfigByClientId(ConfigFakeJwtUsername, user.ClientId);
            var password = _getConfigByClientId(ConfigFakeJwtPassword, user.ClientId);

            var pwdEncrypt = sha256(user.Password);

            var authenticated = string.Equals(user.Username, username) && string.Equals(pwdEncrypt, password);
            _logger.LogDebug("pwdEncrypt:"+pwdEncrypt);
            if (authenticated) {

                return _generateToken(username);

            } else {
                throw new AuthException("Incorrect data user.", StatusCodes.Status401Unauthorized);
            }
        }

        /// <summary>Token com expiração atualizada, de validade de autenticação.</summary>
        /// <param name="dataRefresh">Dados para atualização do token.</param>
        /// <returns>Dados do token atualizado para nova expiração.</returns>
        public Token Refresh(DataRefreshToken dataRefresh) 
        {
            if (_cache.ContainsKey(dataRefresh.RefreshToken)) {

                var username = _cache[dataRefresh.RefreshToken];
                var retorno = _generateToken(username);
                _cache.Remove(dataRefresh.RefreshToken);
                return retorno;

            } else {
                
                throw new AuthException("Invalid refresh token", StatusCodes.Status401Unauthorized);
            }
        }

        private Token _generateToken(string username) {
            
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(username, "Login"),
                new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.UniqueName, username)
                }
            );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao +
                TimeSpan.FromSeconds( Convert.ToDouble(_getConfig(ConfigFakeJwtExpirationSeconds) ));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_getConfig(ConfigFakeJwtKey)));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _getConfig(ConfigFakeJwtIssuer),
                Audience = _getConfig(ConfigFakeJwtAudience),
                SigningCredentials = creds,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            var retorno = new Token();
            retorno.AccessToken = token;
            retorno.TokenType = "bearer";
            // TODO tempo de expiração
            retorno.ExpiresIn = 1199;
            retorno.RefreshToken = Guid.NewGuid().ToString().Replace("-", String.Empty);

            _cache[retorno.RefreshToken] = username;

            return retorno;
        }

        private string _getConfig(string configPath) 
        {
            var retorno = _configuration[configPath];
            _validateConfigValue(configPath, retorno);
            return retorno;
        }

        private string _getConfigByClientId(string configPath, string clientId) 
        {
            var retorno = _configuration[configPath];
            
            if (!string.IsNullOrEmpty(clientId)) {
              var value = _configuration[string.Format("{0}.{1}", configPath, clientId)];
              if (!string.IsNullOrEmpty(value)) { 
                  retorno = value;
              }
            } 
            
            _validateConfigValue(configPath, retorno);

            return retorno;
        }

        private void _validateConfigValue(string configPath, string value) 
        {
            if (string.IsNullOrEmpty(value)) {
                
                var msg = string.Format(
                    "Configuração não encontrada. Vide appsettings<.[env]>.json, path: {0}.<clientId> ", 
                    configPath
                );

                _logger.LogError(msg);

                throw new AuthException("Configuration internal error.", StatusCodes.Status500InternalServerError);
            }
        }

        static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

    }
}
