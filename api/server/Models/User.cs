using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ONS.AuthProvider.Api.Models
{
    public class User 
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }

        public string HostOrigin { get; set; }

        public void Validate() {

        }
         
        public override string ToString() {
            return string.Format(
                "{0} {{ username={1}, client_id={2} }}", 
                this.GetType().Name, Username, ClientId
            );
        }
    }
}