using Microsoft.Extensions.Logging;

namespace ONS.AuthProvider.Validator
{
    /// <summary>Classe com as opções de validação da autenticação.</summary>
    public class AuthValidateOptions
    {
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateAudience { get; set; } = true;
        public bool ValidateLifetime { get; set; } = true;
        public bool ValidateIssuerSigningKey { get; set; } = true;

        public bool RequireExpirationTime { get; set; } = true;
        public bool RequireSignedTokens { get; set; } = true;

        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
        public string ValidSecretKey { get; set; }

        public bool UseRsa { get; set; }
        public string FileRsaPublicKeyXml { get; set; }

        public ILogger Logger { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0} {{ ValidIssuer={1}, ValidAudience={2}, ValidSecretKey={3}, UseRsa={4}, FileRsaPublicKeyXml={5} }}",
                GetType().Name, ValidIssuer, ValidAudience, ValidSecretKey, UseRsa, FileRsaPublicKeyXml
            );
        }
    }
}