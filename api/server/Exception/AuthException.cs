using System;

namespace ONS.AuthProvider.Api.Exception
{
    public class AuthException: System.Exception 
    {
        public AuthException() {}

        public AuthException(string msg): base(msg) {}

        public AuthException(string msg, System.Exception ex): base(msg, ex) {}
    }
}