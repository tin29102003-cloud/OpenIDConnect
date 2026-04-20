using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OidcWebClient.Controllers
{
    public class ProfileController : Controller
    {
        [Authorize]//no sẽ chuyển qua trang controllẻ bên client server để xác thực nếu chưa đăng nhập, sau khi đăng nhập thành công nó sẽ quay lại trang này
        public IActionResult Index()
        {
            return View();
        }
    }
}
