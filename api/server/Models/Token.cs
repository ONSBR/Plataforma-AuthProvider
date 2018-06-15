using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONS.AuthProvider.Api.Models
{
    public class Token 
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
 
        public override string ToString() {
            return string.Format(
                "{0} {{ access_token={1}, token_type={2}, expire_in={3}, refresh_token={4} }}", 
                this.GetType().Name, AccessToken, TokenType, ExpiresIn, RefreshToken
            );
        }
    }
}