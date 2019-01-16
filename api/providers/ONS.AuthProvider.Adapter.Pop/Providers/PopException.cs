using System;

namespace ONS.AuthProvider.Adapter.Pop.Providers
{
    ///<summary>Exceção relacionada a autenticação da plataforma no pop.</summary>
    public class PopException : Exception
    {
        public PopException(string error, string errorDescription)
        {
            Error = error;
            ErrorDescription = errorDescription;
        }

        public string Error { get; }

        public string ErrorDescription { get; }
    }
}