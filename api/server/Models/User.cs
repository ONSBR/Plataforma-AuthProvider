using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ONS.AuthProvider.Api.Exception;

namespace ONS.AuthProvider.Api.Models
{
    ///<summary>Dados de autenticação do usuário.</summary>
    public class User 
    {
        /// <summary>Nome do usuário de autenticação.</summary>
        public string Username { get; set; }

        /// <summary>Senha do usuário de autenticação.</summary>
        public string Password { get; set; }
        
        /// <summary>Identificador do client que está fazendo a autenticação.</summary>
        public string ClientId { get; set; }

        /// <summary>Indica a origem da solicitação de autenticação.</summary>
        public string HostOrigin { get; set; }

        /// <summary>Valida os dados da entidade.</summary>
        public void Validate() {
            var msgInvalid = new StringBuilder();
            if (string.IsNullOrEmpty(Username)) {
                msgInvalid.Append("Username not informed."); 
            } 
            else if (string.IsNullOrEmpty(Password)) {
                msgInvalid.Append("Password not informed."); 
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
                "{0} {{ username={1}, client_id={2} }}", 
                this.GetType().Name, Username, ClientId
            );
        }
    }
}