using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OidcWebClient.Models;

namespace OidcWebClient.Extensions
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddOidcAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var oidcOptions = new OidcOptions();
            configuration.GetSection("Oidc").Bind(oidcOptions);

            return services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = oidcOptions.Authority;
                    options.ClientId = oidcOptions.ClientId;
                    options.ClientSecret = oidcOptions.ClientSecret;
                    //options.UsePkce = oidcOptions.UsePkce;
                    options.ResponseType = oidcOptions.ResponseType;
                    options.SaveTokens = oidcOptions.SaveTokens;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                    options.TokenValidationParameters.SignatureValidator = delegate (string token, TokenValidationParameters validationParameters)
                    {
                        var jwt = new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
                        return jwt;
                    };
                    //options.GetClaimsFromUserInfoEndpoint = true;

                    //foreach (var scope in oidcOptions.Scopes)
                    //{
                    //    options.Scope.Add(scope);
                    //}
                });
        }
    }
}