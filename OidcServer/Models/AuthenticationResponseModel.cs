using System.Text.Json.Serialization;

namespace OidcServer.Models
{
    public class AuthenticationResponseModel : RefreshResponseModel
    {
        [JsonPropertyName("id_token")]
        public required string IdToken { get; set; }//id token đc định nghĩa bởi openid connect và at và rt  thi đc định nghĩa bởi auth2.0, id token chứa thông tin về người dùng đã xác thực, được mã hóa dưới dạng JWT (JSON Web Token). Id token thường được sử dụng trong OpenID Connect để cung cấp thông tin về người dùng cho client sau khi họ đã đăng nhập thành công. Id token có thể bao gồm các claim như sub (subject - định danh người dùng), name (tên người dùng), email (địa chỉ email của người dùng), và các thông tin khác tùy thuộc vào phạm vi (scope) mà client yêu cầu trong quá trình xác thực. Id token giúp client xác định danh tính của người dùng và có thể được sử dụng để tạo phiên làm việc hoặc cung cấp quyền truy cập vào các tài nguyên bảo vệ.
    }
}
