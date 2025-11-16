using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RetailShop.Models;
using RetailShop.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies; 

namespace RetailShop.Controllers;

[Route("login")]
public class LoginController : Controller
{
    private readonly AppDbContext _context;
    //tạo constructor để inject Dbcontext
    public LoginController(AppDbContext context)
    {
        _context = context;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

    //Action xử lý form login
    [HttpPost]
    [Route("signin")]
    public async Task<IActionResult> SignIn(string Username, string Password)
    {
        
        // Tìm kiếm User theo Username và Password
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);

        if (user == null)
        {
            
            TempData["err"] = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return RedirectToAction("Index");
        }

        // 2. Check role: Chỉ có Admin mới có thể login thành công
        if (user.Role?.ToLower() != "admin")
        {
            // Nếu không phải Admin, báo lỗi
            TempData["err"] = "Chỉ Admin mới có thể đăng nhập vào hệ thống.";
            return RedirectToAction("Index");
        }

        //giai đoạn xác thực
        // Tạo các Claims cho người dùng
        var claims = new List<Claim>
        {
            // ClaimTypes.Name sẽ được dùng bởi User.Identity.Name trên View
            new Claim(ClaimTypes.Name, user.Username!), 
            
            // ClaimTypes.Role sẽ được dùng để kiểm tra quyền và hiển thị trên Sidebar
            new Claim(ClaimTypes.Role, user.Role ?? "staff"), 
            
            // Thêm UserId vào Claims
            new Claim("UserId", user.UserId.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            // Thiết lập nếu bạn muốn Remember Me (Giữ trạng thái đăng nhập)
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) // Ví dụ: Hết hạn sau 60 phút
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),authProperties);

        // 3. Mã hóa UserId sang Base64
        string userIdBase64 = EncodeBase64(user.UserId.ToString());

        // 4. Truyền UserId đã mã hóa về View qua TempData
        TempData["LoggedInUserIdBase64"] = userIdBase64;

        // Dòng này (TempData["success"]) sẽ kích hoạt logic JavaScript trong Index.cshtml
        TempData["success"] = "Đăng nhập thành công. Đang chuyển hướng...";

        return View("Index");
    }

    // Hàm tiện ích: Mã hóa Base64
    private string EncodeBase64(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    [Route("v2")]
    public IActionResult Login()
    {
        return View("Login");
    }

  
    // Đăng xuất
    [HttpGet("logout")] // Định tuyến thành /login/logout
    public async Task<IActionResult> Logout()
    {
        // Xóa tất cả các thông tin xác thực (claims/cookies) của người dùng hiện tại
        await HttpContext.SignOutAsync(
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme
        );

        // Chuyển hướng người dùng về trang đăng nhập
        return RedirectToAction("Index", "Login"); // Trả về trang /login
    }
}
