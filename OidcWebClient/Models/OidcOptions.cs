using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OidcWebClient.Models
{
    public class OidcOptions
    {
        public string Authority { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public bool UsePkce { get; set; } = true;
        public string ResponseType { get; set; } = OpenIdConnectResponseType.Code;
        public bool SaveTokens { get; set; } = true;
        public List<string> Scopes { get; set; } = new() { "openid", "profile", "email" };
    }
}
