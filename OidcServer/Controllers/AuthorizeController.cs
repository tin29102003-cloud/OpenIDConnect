using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OidcServer.Helper;
using OidcServer.Models;
using OidcServer.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OidcServer.Controllers
{
    public class AuthorizeController : Controller
    {
        public readonly  IUserRepository userRepository;
        public readonly ICodeItemRepository codeItemRepository;
        private readonly TokenIssuingOptions tokenIssuingOptions;
        public AuthorizeController(IUserRepository userRepository, ICodeItemRepository codeItemRepository, TokenIssuingOptions tokenIssuingOptions) {
            this.userRepository = userRepository;
            this.codeItemRepository = codeItemRepository;
            this.tokenIssuingOptions = tokenIssuingOptions; 
        }
        public IActionResult Index(AuthenticationRequestModel authenticationRequest)
        {
            ValidateAuthenticateRequestModel(authenticationRequest);
            return View(authenticationRequest);
        }
        [HttpPost]
        public IActionResult authorize(AuthenticationRequestModel authenticationRequest, string username, string password, string[] scopes)
        {
            if(userRepository.GetUserByUsername(username) == null)
            {
                return View("UserNotFound");
            }
            if (!userRepository.ValidateUserCredentials(username, password)) 
            {
                return View("PasswordNotCorrect");
            }
            string code = GeneratedCode();
            var model = new CodeFlowResponseViewModel()
            {
                Code = code,
                State = authenticationRequest.State,
                RedirectUri = authenticationRequest.RedirectUri
            };
            codeItemRepository.Add(code, new CodeItem()
            {
                AuthenticationRequest = authenticationRequest,
                User = username,
                Scopes = scopes

            });
            return  View("SubmitForm",model);
        }
        //sau khi author xong no sẽ nhảy qua oauth/token để lấy access token, cái này nằm trong phần token endpoint, nó sẽ nhận yêu cầu từ client để đổi mã xác thực (authorization code) lấy access token. Tuy nhiên, trong ví dụ này, chúng ta chưa triển khai logic để xử lý yêu cầu này, nên chúng ta sẽ trả về NotFound() để biểu thị rằng endpoint này chưa được triển khai hoặc không tồn tại. trong josn file
        [Route("oauth/token")]//cái này nằm trong phần token endpoint, nó sẽ nhận yêu cầu từ client để đổi mã xác thực (authorization code) lấy access token. Tuy nhiên, trong ví dụ này, chúng ta chưa triển khai logic để xử lý yêu cầu này, nên chúng ta sẽ trả về NotFound() để biểu thị rằng endpoint này chưa được triển khai hoặc không tồn tại. trong josn file
        [HttpPost]
        public IActionResult ReturnTokens(string grant_type, string code, string redirect_uri)
        {
            if(grant_type != "authorization_code")
            {
                return BadRequest();
            }
            var codeItem = codeItemRepository.FindByCode(code);  
            if(codeItem == null)
            {
                return BadRequest();
            }
            codeItemRepository.Delete(code);
            if(codeItem.AuthenticationRequest.RedirectUri != redirect_uri)
            {
                return BadRequest();
            }
            var jwk = JwkLoader.LoadFromDefault();
            var model = new AuthenticationResponseModel()
            {
                AccessToken = GeneratedAccessToken(codeItem.User,string.Join(' ',codeItem.Scopes), codeItem.AuthenticationRequest.ClientId, codeItem.AuthenticationRequest.Nonce, jwk),//cái acccess token dùng để lấy dữ liệu
                TokenType = "Bearer",
                ExpiresIn = 3600,
                IdToken = GenerateIdToken(codeItem.User, string.Join(' ', codeItem.Scopes), codeItem.AuthenticationRequest.ClientId, codeItem.AuthenticationRequest.Nonce, jwk),//idtoken dùng để lấy thông tin người dùng , nó sẽ được gửi về client sau khi người dùng đã xác thực thành công. Client có thể sử dụng id token để hiển thị thông tin người dùng hoặc để xác thực người dùng trong các yêu cầu tiếp theo đến resource server. Id token thường chứa các claim như sub (subject), name, email, và các claim khác liên quan đến người dùng đã xác thực.
                RefreshToken = GeneratedRefreshToken()
            };
            return Json(model);
           
        }

        private string GenerateIdToken(string userId, string scope, string audience, string nonce, JsonWebKey jsonWebKey)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId),
                new("scope", scope),
                new(JwtRegisteredClaimNames.Nonce, nonce)
            };
            var idToken = JwtGenerator.GenerateJwtToken(
                tokenIssuingOptions.IdTokenExpirySeconds,
                tokenIssuingOptions.Issuer,
                audience,
                nonce,
                claims,
                jsonWebKey
            );

            return idToken;
        }

        private string GeneratedRefreshToken()
        {
            return GeneratedCode();
        }

        private string GeneratedAccessToken(string userId, string scope, string audience, string nonce, JsonWebKey jsonWebKey)
        {
            var claims = new List<Claim>
           {
               new(JwtRegisteredClaimNames.Sub, userId),//claim sub (subject) được sử dụng để định danh người dùng trong access token. Nó thường chứa một giá trị duy nhất đại diện cho người dùng đã xác thực, chẳng hạn như ID người dùng hoặc tên đăng nhập. Claim này giúp resource server xác định danh tính của người dùng khi họ gửi access token để truy cập vào các tài nguyên bảo vệ.
               new("scope", scope)//dòng này là thêm một claim tùy chỉnh có tên "scope" vào access token. Claim này chứa thông tin về phạm vi (scope) mà người dùng đã cấp quyền truy cập. Phạm vi xác định các quyền và quyền hạn mà người dùng đã đồng ý khi cấp quyền cho ứng dụng client. Resource server có thể sử dụng claim này để kiểm tra xem người dùng có đủ quyền để truy cập vào tài nguyên cụ thể hay không.
               ,new(JwtRegisteredClaimNames.Nonce, nonce)//bạn có thể thêm claim nonce vào access token để đảm bảo tính bảo mật và chống lại các cuộc tấn công replay. Claim nonce chứa một giá trị ngẫu nhiên duy nhất được tạo ra trong quá trình xác thực và được gửi từ client đến authorization server. Khi authorization server tạo access token, nó sẽ bao gồm claim nonce trong token. Khi client gửi access token đến resource server, resource server có thể kiểm tra claim nonce để đảm bảo rằng token không bị tái sử dụng hoặc bị đánh cắp bởi kẻ tấn công.
           };
            var AccessToken = JwtGenerator.GenerateJwtToken(
                    tokenIssuingOptions.AccessTokenExpirySeconds,
                    tokenIssuingOptions.Issuer,
                    audience,
                    nonce,
                    claims,
                    jsonWebKey
                );
            return AccessToken;
        }

        static Random random = new();
        private string GeneratedCode()
        {
            
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 32)//dòng này là tạo ra một chuỗi ngẫu nhiên có độ dài 32 ký tự bằng cách lặp lại chuỗi chars nhiều lần và chọn ngẫu nhiên một ký tự từ chuỗi đó để tạo thành mã xác thực (authorization code). Chuỗi chars bao gồm tất cả các chữ cái hoa, chữ cái thường và số, giúp đảm bảo rằng mã xác thực có đủ độ phức tạp và khó đoán.
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //hàm này kiểm tra xem du lieuj vào có rỗng hay ko, nếu không hợp lệ sẽ ném ra lỗi ArgumentNullException
        private void ValidateAuthenticateRequestModel(AuthenticationRequestModel authenticationRequest)
        {
            ArgumentNullException.ThrowIfNull(authenticationRequest.ClientId, nameof(authenticationRequest.ClientId));
            if (string.IsNullOrEmpty(authenticationRequest.ClientId))
            {
                throw new Exception("ClientId is required");
            }
            if (string.IsNullOrEmpty(authenticationRequest.ResponseType))
            {
                throw new Exception("Response_type_required");
            }
            if (string.IsNullOrEmpty(authenticationRequest.Scope))
            {
                throw new Exception("Scope is required");
            }
            if (string.IsNullOrEmpty(authenticationRequest.RedirectUri))
            {
                throw new Exception("Redirect_uri is required");
            }
        }
    }
}
