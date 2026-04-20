namespace OidcServer.Models
{
    public class TokenIssuingOptions
    {
        public string Issuer { get; set; } = "https://localhost:7057"; // this server's Issuer ID, must be an HTTPS URL
        public int IdTokenExpirySeconds { get; set; } = 60 * 20;
        public int AccessTokenExpirySeconds { get; set; } = 60 * 5;
        public int RefreshTokenExpirySeconds { get; set; } = 60 * 24 * 7;
    }
}
