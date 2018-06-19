using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ONS.AuthProvider.Api.Exception;

using Microsoft.AspNetCore.Http;

namespace ONS.AuthProvider.Api.Models
{
    public class User 
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }

        public string HostOrigin { get; set; }

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
                throw new AuthException(msgInvalid.ToString(), StatusCodes.Status400BadRequest);
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