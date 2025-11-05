using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RetailShop.Models;
using RetailShop.Data;

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

        // 3. Mã hóa UserId sang Base64
        string userIdBase64 = EncodeBase64(user.UserId.ToString());

        // 4. Truyền UserId đã mã hóa về View qua TempData
        TempData["LoggedInUserIdBase64"] = userIdBase64;

        // Dòng này (TempData["success"]) sẽ kích hoạt logic JavaScript trong Index.cshtml
        TempData["success"] = "Đăng nhập thành công. Đang chuyển hướng...";

        return RedirectToAction("Index");
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
}
