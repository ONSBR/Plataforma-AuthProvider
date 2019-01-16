using System;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.Adapter.Fake.Providers
{
    /// <summary>Configuração do adaptador fake.</summary>
    public class FakeConfiguration
    {
        /// <summary>Configuração de Path do Endpoint de autorização OAuth.</summary>
        public string AuthorizeEndpointPath { get; set; }

        /// <summary>Configuração de Path do Endpoint de geração de token OAuth.</summary>
        public string TokenEndpointPath { get; set; }

        /// <summary>Configuração de permissão de acesso por http, sem https.</summary>
        public bool? AllowInsecureHttp { get; set; }

        /// <summary>Configuração de autenticação automática.</summary>
        public bool? AutomaticAuthenticate { get; set; }

        /// <summary>Configuração de audiences válidos de clientId para autenticação.</summary>
        public string[] Audiences { get; set; }

        /// <summary>Configuração de roles válidos de autenticação.</summary>
        public string[] Roles { get; set; }

        /// <summary>Configuração de credenciais válidas para autenticação.</summary>
        public Credentials Credentials { get; set; }

        /// <summary>Configuração para geração de token com JWT.</summary>
        public JwtToken JwtToken { get; set; }

        /// <summary>Validação das configurações de autenticação.</summary>
        public void Validate()
        {
            var msg = "Configuration Not found. KeyConfig={0}";

            if (string.IsNullOrEmpty(AuthorizeEndpointPath))
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => AuthorizeEndpointPath)));
            if (string.IsNullOrEmpty(TokenEndpointPath))
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => TokenEndpointPath)));
            if (!AllowInsecureHttp.HasValue)
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => AllowInsecureHttp)));
            if (!AutomaticAuthenticate.HasValue)
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => AutomaticAuthenticate)));
            if (Audiences == null || Audiences.Length == 0)
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => Audiences)));
            if (Roles == null || Roles.Length == 0)
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => Roles)));
            if (Credentials == null)
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => Credentials)));
            Credentials.Validate();
            if (JwtToken == null)
                throw new Exception(string.Format(msg, PropertyUtil.GetName(() => JwtToken)));
            JwtToken.Validate();
        }
    }

    /// <summary>Configuração das credenciais para autenticação do adaptador fake.</summary>
    public class Credentials
    {
        /// <summary>Configuração do usuário para autenticação.</summary>
        public string Username { get; set; }

        /// <summary>Configuração de senha para autenticação. Esse valor tem criptografia SHA256.</summary>
        public string Password { get; set; }

        /// <summary>Valida as configurações de credenciais.</summary>
        public void Validate()
        {
            var msg = "Configuration Not found. KeyConfig={0}";

            if (string.IsNullOrEmpty(Username))
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => Username)));
            if (string.IsNullOrEmpty(Password))
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => Password)));
        }
    }

    /// <summary>Configuração para geração do token no formato JWT.</summary>
    public class JwtToken
    {
        /// <summary>Configuração de chave para assinatura do token.</summary>
        public string SecretKey { get; set; }

        /// <summary>Configuração de issuer para assinatura do token.</summary>
        public string Issuer { get; set; }

        /// <summary>Configuração de tempo de expiração de validação do token.</summary>
        public double? Expiration { get; set; }

        /// <summary>Configuração de tempo de expiração de validação do refresh token.</summary>
        public double? RefreshExpiration { get; set; }

        /// <summary>Configuração que indica se deve ser usado RSA no token.</summary>
        public bool UseRsa { get; set; }

        /// <summary>Configuração da chave privada para assinar RSA no token.</summary>
        public string RsaPrivateKeyXml { get; set; }

        /// <summary>Valida as configurações de geração de token.</summary>
        public void Validate()
        {
            var msg = "Configuration Not found. KeyConfig={0}";

            if (string.IsNullOrEmpty(Issuer))
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => Issuer)));
            if (!Expiration.HasValue)
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => Expiration)));
            if (!RefreshExpiration.HasValue)
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => RefreshExpiration)));
            if (UseRsa)
            {
                if (string.IsNullOrEmpty(RsaPrivateKeyXml))
                    throw new Exception(string.Format(msg, PropertyUtil.GetName(() => RsaPrivateKeyXml)));
            }
            else
            {
                if (string.IsNullOrEmpty(SecretKey))
                    throw new Exception(string.Format(msg, PropertyUtil.GetName(() => SecretKey)));
            }
        }
    }
}