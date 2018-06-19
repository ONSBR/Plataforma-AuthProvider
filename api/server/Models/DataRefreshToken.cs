using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ONS.AuthProvider.Api.Exception;

using Microsoft.AspNetCore.Http;

namespace ONS.AuthProvider.Api.Models
{
    public class DataRefreshToken 
    {
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }

        public string HostOrigin { get; set; }
 
        public void Validate() {

            var msgInvalid = new StringBuilder();
            if (string.IsNullOrEmpty(RefreshToken)) {
                msgInvalid.Append("RefreshToken not informed."); 
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
                "{0} {{ refresh_token={1}, client_id={2} }}", 
                this.GetType().Name, RefreshToken, ClientId
            );
        }
    }
}