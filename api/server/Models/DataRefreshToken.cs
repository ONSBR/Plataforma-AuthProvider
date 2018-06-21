using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using ONS.AuthProvider.Api.Exception;

namespace ONS.AuthProvider.Api.Models
{
    ///<summary>Dados para geração de um novo token atualizado, com nova expiração.</summary>
    public class DataRefreshToken 
    {
        /// <summary>Identificador para atualização do token, gerando novo token de expiração.</summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>Identificador do client que está fazendo a autenticação.</summary>
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        /// <summary>Indica a origem da solicitação de autenticação.</summary>
        public string HostOrigin { get; set; }
 
        /// <summary>Valida os dados da entidade.</summary>
        public void Validate() {

            var msgInvalid = new StringBuilder();
            if (string.IsNullOrEmpty(RefreshToken)) {
                msgInvalid.Append("RefreshToken not informed."); 
            } 
            else if (string.IsNullOrEmpty(ClientId)) {
                msgInvalid.Append("ClientId not informed."); 
            }
            
            if (msgInvalid.Length > 0) {
                throw new AuthException(msgInvalid.ToString());
            }
        }

        public override string ToString() {
            return string.Format(
                "{0} {{ refresh_token={1}, client_id={2} }}", 
                this.GetType().Name, RefreshToken, ClientId
            );
        }
    }
}