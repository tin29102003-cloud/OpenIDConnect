using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;




namespace OidcServer.Helper
{
    public class JwtGenerator
    {//hàm tạo  token JWT (JSON Web Token) với các tham số như thời gian hết hạn, nhà phát hành, đối tượng nhận, nonce, các claim và khóa JSON Web Key. Hàm này sẽ sử dụng thông tin này để tạo ra một token JWT được ký bằng thuật toán RSA-SHA256 và trả về dưới dạng chuỗi.
        public static string GenerateJwtToken(int expirySeconds, string issuer, string audience, string nonce, IEnumerable<Claim> claims, JsonWebKey jsonWebKey)
        {
           var signingCredentials = new SigningCredentials(jsonWebKey, SecurityAlgorithms.RsaSha256);//dòng này là để tạo ra một đối tượng SigningCredentials sử dụng JsonWebKey và thuật toán ký RSA-SHA256. SigningCredentials là một phần quan trọng trong việc tạo JWT (JSON Web Token) vì nó xác định cách mà token sẽ được ký để đảm bảo tính toàn vẹn và xác thực của token. Trong trường hợp này, chúng ta đang sử dụng một JsonWebKey (thường là một khóa RSA) để ký token, và thuật toán RSA-SHA256 đảm bảo rằng token được ký một cách an toàn và có thể được xác minh bởi các bên nhận token bằng cách sử dụng khóa công khai tương ứng.
            var additionalClaims = new Dictionary<string, object>
            {
                {JwtRegisteredClaimNames.Nonce, nonce}
            };//dòng này là để tạo ra một từ điển (dictionary) chứa các claim bổ sung mà bạn muốn bao gồm trong JWT. Trong trường hợp này, chúng ta đang thêm một claim có tên "nonce" với giá trị được truyền vào từ tham số nonce. Claim "nonce" thường được sử dụng trong OpenID Connect để bảo vệ chống lại các cuộc tấn công phát lại (replay attacks) bằng cách đảm bảo rằng mỗi token được tạo ra có một giá trị nonce duy nhất.
            var jwtHeader = new JwtHeader(signingCredentials);//tạo ra header jwt dùng để biết cách mà token được ký và các thông tin liên quan đến việc xác thực token. Header này sẽ bao gồm thông tin về thuật toán ký (alg) và loại token (typ), cũng như bất kỳ thông tin bổ sung nào khác mà bạn muốn bao gồm trong header. Trong trường hợp này, chúng ta đang sử dụng SigningCredentials để xác định cách mà token sẽ được ký, và JwtHeader sẽ tự động thêm thông tin về thuật toán ký vào header của JWT.
            var jwtPayload = new JwtPayload(issuer, audience, claims, null, DateTime.UtcNow.AddSeconds(expirySeconds), DateTime.UtcNow);
            //tham số thứ 4 là thời diểm token có hiệu lực và tham số thư 6 là thời điểm token đc tạo
            
            var token = new JwtSecurityToken(jwtHeader, jwtPayload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
