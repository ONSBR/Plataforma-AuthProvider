using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONS.AuthProvider.Api.Models
{
    public class DataRefreshToken 
    {
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }

        public string HostOrigin { get; set; }
 
        public override string ToString() {
            return string.Format(
                "{0} {{ refresh_token={1}, client_id={2} }}", 
                this.GetType().Name, RefreshToken, ClientId
            );
        }
    }
}