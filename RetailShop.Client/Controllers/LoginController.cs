using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using RetailShop.Client.Services.IServices;
using System.Security.Claims;
using System.Text;

namespace RetailShop.Client.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        private readonly IUserPOSService _userService;

        public LoginController(IUserPOSService userService)
        {
            _userService = userService;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("signin")]
        public async Task<IActionResult> SignIn(string Username, string Password)
        {
            var user = await _userService.GetUserByCredentials(Username, Password);

            if (user == null)
            {
                TempData["err"] = "Thông tin đăng nhập không chính xác";
                return RedirectToAction("Index");
            }

            // Kiểm tra role phải là staff hoặc admin
            if (user.Role?.ToLower() != "staff" && user.Role?.ToLower() != "admin")
            {
                TempData["err"] = "Bạn không có quyền sử dụng hệ thống";
                return RedirectToAction("Index");
            }

            // Kiểm tra tài khoản có active không
            if (!user.Active)
            {
                TempData["err"] = "Tài khoản đã bị vô hiệu hóa. Vui lòng liên hệ quản lý.";
                return RedirectToAction("Index");
            }

            // Tạo Claims cho Authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("FullName", user.FullName ?? user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "staff"),
                new Claim("DisplayRole", GetDisplayRole(user.Role))
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), authProperties);

            // Mã hóa UserId sang Base64 cho localStorage theo requirement
            string userIdBase64 = EncodeBase64(user.UserId.ToString());
            TempData["LoggedInUserIdBase64"] = userIdBase64;
            TempData["success"] = "Đăng nhập thành công";

            return RedirectToAction("Index");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất khỏi Cookie Authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Xóa userId khỏi localStorage
            return RedirectToAction("Index", "Login");
        }

        // Hàm mã hóa Base64
        private string EncodeBase64(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        // Hàm chuyển đổi role sang tiếng Việt
        private string GetDisplayRole(string role)
        {
            return role?.ToLower() switch
            {
                "admin" => "Quản lý",
                "staff" => "Nhân viên bán hàng",
                _ => "(Không xác định)"
            };
        }
    }
}
