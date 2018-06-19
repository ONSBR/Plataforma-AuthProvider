using System;

namespace ONS.AuthProvider.Api.Exception
{
    public class AuthException: System.Exception 
    {
        public int? StatusCode {get;set;}

        public AuthException() {}

        public AuthException(int statusCode) {
            StatusCode = statusCode;
        }

        public AuthException(string msg): base(msg) {}

        public AuthException(string msg, int statusCode): base(msg) {
            StatusCode = statusCode;
        }

        public AuthException(string msg, System.Exception ex): base(msg, ex) {}

        public AuthException(string msg, System.Exception ex, int statusCode): base(msg, ex) {
            StatusCode = statusCode;
        }
    }
}