using System;
using System.Security.Cryptography;
using System.Text;

namespace ONS.AuthProvider.Common.Util
{
    ///<summary>Classe utilitária com métodos de criptografia..</summary>
    public static class Crypto
    {
        ///<summary>Método para gerar um hash com criptografia do tipo Sha256, para um texto qualquer informado.</summary>
        ///<param name="input">Texto informado para ser criptografado.</param>
        ///<returns>Texto criptografado.</returns>
        public static string Sha256(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();

            var byteValue = Encoding.UTF8.GetBytes(input);

            var byteHash = hashAlgorithm.ComputeHash(byteValue);

            return Convert.ToBase64String(byteHash);
        }
    }
}