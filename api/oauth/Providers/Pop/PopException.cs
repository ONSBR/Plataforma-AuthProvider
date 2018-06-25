using System;

namespace ONS.AuthProvider.OAuth.Providers.Pop
{
    ///<summary>Exceção relacionada a autenticação da plataforma no pop.</summary>
    public class PopException: System.Exception 
    {
        private string _error;
        private string _errorDescription;

        public string Error { get {return _error;}}

        public string ErrorDescription { get {return _errorDescription;}}

        public PopException(string error, string errorDescription) {
            _error = error;
            _errorDescription = errorDescription;
        }
    }
}