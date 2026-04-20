using Microsoft.IdentityModel.Tokens;

namespace OidcServer.Helper
{
    public class JwkLoader
    {
        private static readonly string DefaultFile = Path.Combine("OidcDiscovery", "default-jwk.json");
        private static JsonWebKey? defaultJwt = null;//jsonweb lay tu identity model .token
       private static SpinLock spinLock = new();
        public static JsonWebKey LoadFromFile(string file) 
        {
            var fi = new FileInfo(file);//FileInfo là một lớp trong .NET cung cấp các phương thức và thuộc tính để làm việc với các tệp tin và thư mục trên hệ thống tệp. Nó cho phép bạn truy cập thông tin về tệp tin, như tên, kích thước, ngày tạo, ngày sửa đổi, v.v., cũng như thực hiện các thao tác như đọc, ghi, xóa tệp tin.
            if (fi.Exists)
            {
                using var reader = fi.OpenText();//dòng này mở tệp tin để đọc nội dung của nó. Nếu tệp tin tồn tại, nó sẽ trả về một đối tượng StreamReader để đọc dữ liệu từ tệp tin đó. Nếu tệp tin không tồn tại, nó sẽ trả về null.
                var  json =  reader.ReadToEnd();//dòng này đọc toàn bộ nội dung của tệp tin và lưu nó vào biến json dưới dạng một chuỗi. Phương thức ReadToEnd() sẽ đọc từ vị trí hiện tại của con trỏ đến cuối tệp tin, do đó nó sẽ trả về toàn bộ nội dung của tệp tin.
                return new JsonWebKey(json);//dòng này tạo một đối tượng JsonWebKey mới bằng cách sử dụng chuỗi json đã đọc từ tệp tin. JsonWebKey là một lớp trong thư viện Microsoft.IdentityModel.Tokens đại diện cho một khóa JSON Web Key (JWK), thường được sử dụng để xác thực và mã hóa trong các ứng dụng bảo mật.
            }
            else
            {
                throw new FileNotFoundException($"File {file} not found");
            }
        }
        public static JsonWebKey LoadFromDefault()
        {
            bool lockTaken = false;
            spinLock.Enter(ref lockTaken);//dòng này cố gắng lấy một khóa (lock) để đảm bảo rằng chỉ một luồng (thread) có thể truy cập vào phần mã được bảo vệ bởi khóa tại một thời điểm. Biến lockTaken sẽ được đặt thành true nếu khóa đã được lấy thành công, và false nếu không thể lấy được khóa.
            if (lockTaken)
            {
                defaultJwt ??= LoadFromFile(DefaultFile);//dòng này sử dụng toán tử null-coalescing assignment (??=) để gán giá trị cho defaultJwt nếu nó hiện tại là null. Nếu defaultJwt đã có giá trị, nó sẽ giữ nguyên giá trị đó. Nếu defaultJwt là null, nó sẽ gọi phương thức LoadFromFile với DefaultFile để tải một JsonWebKey từ tệp tin và gán nó cho defaultJwt.
                spinLock.Exit();//dòng này giải phóng khóa (lock) đã được lấy trước đó bằng cách gọi phương thức Exit() trên đối tượng SpinLock. Điều này cho phép các luồng khác có thể lấy khóa và truy cập vào phần mã được bảo vệ bởi khóa.
            }
            else
            {
                throw new InvalidOperationException();//lỗi này sẽ ko bao giờ xảy ra vì nếu lockTaken là false, nghĩa là không thể lấy được khóa, thì sẽ không có luồng nào có thể truy cập vào phần mã được bảo vệ bởi khóa, do đó sẽ không có tình huống nào mà InvalidOperationException được ném ra trong trường hợp này.
            }
            return defaultJwt;
        }
    }
}
