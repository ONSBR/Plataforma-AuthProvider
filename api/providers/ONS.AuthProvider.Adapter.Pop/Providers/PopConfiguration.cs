using System;
using ONS.AuthProvider.Common.Util;

namespace ONS.AuthProvider.Adapter.Pop.Providers
{
    /// <summary>Configuração do adaptador do POP.</summary>
    public class PopConfiguration
    {
        /// <summary>Configuração de Path do Endpoint de autorização OAuth.</summary>
        public string AuthorizeEndpointPath { get; set; }

        /// <summary>Configuração de Path do Endpoint de geração de token OAuth.</summary>
        public string TokenEndpointPath { get; set; }

        /// <summary>Configuração de permissão de acesso por http, sem https.</summary>
        public bool? AllowInsecureHttp { get; set; }

        /// <summary>Configuração de autenticação automática.</summary>
        public bool? AutomaticAuthenticate { get; set; }

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
            if (JwtToken == null)
                throw new Exception(string.Format(msg, PropertyUtil.GetName(() => JwtToken)));
            JwtToken.Validate();
        }
    }

    /// <summary>Configuração para geração do token no formato JWT.</summary>
    public class JwtToken
    {
        /// <summary>Configuração de issuer para assinatura do token.</summary>
        public string Issuer { get; set; }

        /// <summary>Configuração da Url de autenticação do POP.</summary>
        public string UrlJwtOAuth { get; set; }

        /// <summary>Configuração de referer default do header esperado pelo POP.</summary>
        public string HeaderRefererDefault { get; set; }

        /// <summary>Configuração que indica se deve validar o token recebido do pop, com a chave configurada em PopSecretKey.</summary>
        public bool ValidatePopAccessToken { get; set; }

        /// <summary>Configuração de chave para validação da assinatura do token do POP.</summary>
        public string PopSecretKey { get; set; }

        /// <summary>Configuração que indica se deve ser usado RSA no token.</summary>
        public bool UseRsa { get; set; }

        /// <summary>Configuração da chave privada para assinar RSA no token.</summary>
        public string RsaPrivateKeyXml { get; set; }

        /// <summary>Valida as configurações de geração de token.</summary>
        public void Validate()
        {
            var msg = "Configuration Not found. KeyConfig={0}";

            if (string.IsNullOrEmpty(UrlJwtOAuth))
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => UrlJwtOAuth)));
            if (string.IsNullOrEmpty(HeaderRefererDefault))
                throw new Exception(
                    string.Format(msg, PropertyUtil.GetName(() => HeaderRefererDefault)));
            if (UseRsa)
            {
                if (ValidatePopAccessToken && string.IsNullOrEmpty(PopSecretKey))
                    throw new Exception(string.Format(msg, PropertyUtil.GetName(() => PopSecretKey)));
                if (string.IsNullOrEmpty(Issuer))
                    throw new Exception(string.Format(msg, PropertyUtil.GetName(() => Issuer)));
                if (string.IsNullOrEmpty(RsaPrivateKeyXml))
                    throw new Exception(string.Format(msg, PropertyUtil.GetName(() => RsaPrivateKeyXml)));
            }
        }
    }
}