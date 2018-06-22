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


namespace ONS.AuthProvider.OAuth.Providers.Fake
{
    public static class FakeJwtTokenGenerator 
    {
        private const string ConfigFakeJwtUsername = "Auth:Adapter:Providers:fake:Jwt.Username";
        private const string ConfigFakeJwtPassword = "Auth:Adapter:Providers:fake:Jwt.Password";
        private const string ConfigFakeJwtKey = "Auth:Adapter:Providers:fake:Jwt.Token:Key";
        private const string ConfigFakeJwtAudience = "Auth:Adapter:Providers:fake:Jwt.Token:Audience";
        private const string ConfigFakeJwtIssuer = "Auth:Adapter:Providers:fake:Jwt.Token:Issuer";
        private const string ConfigFakeJwtExpiration = "Auth:Adapter:Providers:fake:Jwt.Token:Expiration.Minutes";
        private const string ConfigFakeJwtRole = "Auth:Adapter:Providers:fake:Jwt.Token:Role";

        public static string GenerateToken(string username) 
        {
            var issuer = AuthConfiguration.Get(ConfigFakeJwtIssuer);
            var configKey = AuthConfiguration.Get(ConfigFakeJwtKey);
            var audience = AuthConfiguration.Get(ConfigFakeJwtAudience);
            var expiration = Convert.ToDouble(AuthConfiguration.Get(ConfigFakeJwtExpiration));
            var role = AuthConfiguration.Get(ConfigFakeJwtRole);

            //if (_logger.IsEnabled(LogLevel.Debug)) {
            //    _logger.LogDebug(string.Format("Geração de Token: {0}, {1}, {2}, {3}",
            //        issuer, audience, configKey, expiration
            //    ));
            //}

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(username, "Login"),
                new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                    new Claim(JwtRegisteredClaimNames.UniqueName, username),
                    new Claim("role", role)
                }
            );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao +
                TimeSpan.FromMinutes( expiration );

            var key = new SymmetricSecurityKey(Convert.FromBase64String(configKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = creds,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            return token;

        }
    }
}