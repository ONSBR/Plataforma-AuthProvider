using System;
using System.Collections.Generic;
using System.Linq;

namespace ONS.AuthProvider.OAuth.Providers
{
    public static class AuthorizationAdapterFactory 
    {
        private static IDictionary<string,IAuthorizationAdapter> _adapters = new Dictionary<string,IAuthorizationAdapter>();
        
        public static void Add(string key, IAuthorizationAdapter value) {
            _adapters[key] = value;
        }

        public static IAuthorizationAdapter Get(string adapterName) {

            // TODO validar se contém a chave
            return _adapters[adapterName];
        }
    }

}