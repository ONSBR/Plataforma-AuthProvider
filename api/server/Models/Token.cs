using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONS.AuthProvider.Api.Models
{
    /// <summary>Representa os dados do Token gerado para a autenticação válida.</summary>
    public class Token 
    {
        /// <summary>Indica o token gerado para autenticação.</summary>
        public string AccessToken { get; set; }

        /// <summary>Indica o tipo do token.</summary>
        public string TokenType { get; set; }

        /// <summary>Indica o tempo de expiração para o token de autenticação.</summary>
        public long ExpiresIn { get; set; }

        /// <summary>Identificador para atualização do token, gerando novo token de expiração.</summary>
        public string RefreshToken { get; set; }
 
        public override string ToString() {
            return string.Format(
                "{0} {{ access_token={1}, token_type={2}, expire_in={3}, refresh_token={4} }}", 
                this.GetType().Name, AccessToken, TokenType, ExpiresIn, RefreshToken
            );
        }
    }
}