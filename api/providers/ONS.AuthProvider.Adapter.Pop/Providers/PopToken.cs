using Newtonsoft.Json;
using ONS.AuthProvider.Common.Providers;

namespace ONS.AuthProvider.Adapter.Pop.Providers
{
    /// <summary>Representa os dados do Token gerado para a autenticação válida.</summary>
    public class PopToken
    {
        /// <summary>Indica o token gerado para autenticação.</summary>
        [JsonProperty(Constants.Parameters.AccessToken)]
        public string AccessToken { get; set; }

        /// <summary>Indica o tipo do token.</summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>Indica o tempo de expiração para o token de autenticação.</summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>Identificador para atualização do token, gerando novo token de expiração.</summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0} {{ access_token={1}, token_type={2}, expire_in={3}, refresh_token={4} }}",
                GetType().Name, AccessToken, TokenType, ExpiresIn, RefreshToken
            );
        }
    }
}