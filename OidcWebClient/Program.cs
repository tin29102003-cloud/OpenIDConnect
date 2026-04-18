using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace OidcWebClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            if(builder.Environment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;//hiện thị thông tin cá nhân trong lỗi để dễ dàng gỡ lỗi trong môi trường phát triển. Tuy nhiên, bạn nên tắt nó trong môi trường sản xuất để bảo vệ thông tin cá nhân của người dùng.
            }
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            
            // Add OIDC Authentication
            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;//dắng ký scheme mặc định là cookie
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = builder.Configuration["Oidc:Authority"];//này là URL của Identity Provider (IdP) mà bạn muốn kết nối đến
                    options.ClientId = builder.Configuration["Oidc:ClientId"];//này là ID của ứng dụng của bạn đã đăng ký với IdP
                    options.ClientSecret = builder.Configuration["Oidc:ClientSecret"];//key bí mật của ứng dụng của bạn đã đăng ký với IdP
                    options.UsePkce = true;
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.SaveTokens = true;
                    //options.GetClaimsFromUserInfoEndpoint = true;//dòng này là để lấy thông tin người dùng từ UserInfo Endpoint của IdP sau khi đã nhận được mã xác thực (authorization code)
                    //options.Scope.Add("openid");
                    //options.Scope.Add("profile");
                    //options.Scope.Add("email");
                });
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            //app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
