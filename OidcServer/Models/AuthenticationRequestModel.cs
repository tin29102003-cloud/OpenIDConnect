using Microsoft.AspNetCore.Mvc;
namespace OidcServer.Models
{
    public class AuthenticationRequestModel
    {
        [BindProperty(Name = "client_id", SupportsGet = true)]
        public string ClientId { get; set; } = string.Empty;
        [BindProperty(Name = "redirect_uri", SupportsGet = true)]
        public string RedirectUri { get; set; } = string.Empty;
        [BindProperty(Name = "response_type", SupportsGet = true)]
        public string ResponseType { get; set; } = string.Empty;
        [BindProperty(Name = "scope", SupportsGet = true)]
        public string Scope { get; set; } = string.Empty;//dòng này là để chỉ định phạm vi (scope) của quyền truy cập mà ứng dụng yêu cầu từ người dùng. Phạm vi xác định những gì ứng dụng có thể làm với thông tin của người dùng sau khi được cấp quyền. Ví dụ, nếu bạn yêu cầu scope là "openid profile email", ứng dụng sẽ có quyền truy cập vào thông tin cơ bản của người dùng (openid), thông tin hồ sơ (profile) và địa chỉ email (email) của họ.
        [BindProperty(Name = "code_challenge", SupportsGet = true)]//nàu là một phần của PKCE (Proof Key for Code Exchange) để tăng cường bảo mật cho quá trình xác thực, đặc biệt là trong các ứng dụng di động hoặc ứng dụng một trang (SPA) nơi mà client secret không thể được bảo vệ.
        public string CodeChallenge { get; set; } = string.Empty;//dong này là một giá trị được tạo ra từ client bằng cách sử dụng một hàm băm (hash function) trên một chuỗi ngẫu nhiên (code verifier). Giá trị này sau đó được gửi đến authorization server trong yêu cầu xác thực. Khi authorization server trả về mã xác thực (authorization code), client sẽ sử dụng code verifier để chứng minh rằng họ là người đã bắt đầu quá trình xác thực, giúp ngăn chặn các cuộc tấn công giả mạo.
        [BindProperty(Name = "code_challenge_method", SupportsGet = true)]
        public string CodeChallengeMethod { get; set; } = string.Empty;//dòng nầy là để chỉ định phương thức băm được sử dụng để tạo code challenge. Phương thức phổ biến nhất là "S256", có nghĩa là sử dụng SHA-256 để tạo ra code challenge từ code verifier. Nếu không chỉ định, mặc định sẽ là "plain", có nghĩa là code challenge sẽ giống với code verifier, nhưng phương thức này không được khuyến khích vì ít an toàn hơn.
        [BindProperty(Name = "response_mode", SupportsGet = true)]
        public string ResponseMode { get; set; } = string.Empty;//đây là một tham số tùy chọn trong yêu cầu xác thực OpenID Connect (OIDC) để chỉ định cách mà authorization server sẽ trả về mã xác thực (authorization code) hoặc token cho client. Các giá trị phổ biến cho response_mode bao gồm "query" (trả về qua chuỗi truy vấn URL), "fragment" (trả về qua phần fragment của URL), và "form_post" (trả về qua một POST request). Việc sử dụng response_mode giúp tăng cường bảo mật và kiểm soát cách thức truyền dữ liệu giữa authorization server và client.
        [BindProperty(Name = "nonce", SupportsGet = true)]
        public string Nonce { get; set; } = string.Empty;//dòng này là một giá trị ngẫu nhiên được tạo ra bởi client và gửi đến authorization server trong yêu cầu xác thực. Mục đích của nonce là để bảo vệ chống lại các cuộc tấn công phát lại (replay attacks). Khi authorization server trả về mã xác thực (authorization code) hoặc token, nó sẽ bao gồm giá trị nonce mà client đã gửi. Client sau đó sẽ kiểm tra xem giá trị nonce trong phản hồi có khớp với giá trị mà nó đã gửi hay không. Nếu không khớp, client sẽ từ chối phản hồi vì có thể đó là một cuộc tấn công phát lại.
        [BindProperty(Name = "state", SupportsGet = true)]
        public string State { get; set; } = string.Empty;//dòng này là một giá trị ngẫu nhiên được tạo ra bởi client và gửi đến authorization server trong yêu cầu xác thực. Mục đích của state là để bảo vệ chống lại các cuộc tấn công CSRF (Cross-Site Request Forgery). Khi authorization server trả về mã xác thực (authorization code) hoặc token, nó sẽ bao gồm giá trị state mà client đã gửi. Client sau đó sẽ kiểm tra xem giá trị state trong phản hồi có khớp với giá trị mà nó đã gửi hay không. Nếu không khớp, client sẽ từ chối phản hồi vì có thể đó là một cuộc tấn công giả mạo.

    }
}
