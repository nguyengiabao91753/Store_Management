using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Dtos;
using RetailShop.Services.IServices;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace RetailShop.Controllers
{
    [Route("user")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        // Dependency Injection: Nhận IUserService qua constructor
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // -------------------------------------------------------------------
        // 1. ACTION: Hiển thị Danh sách (READ - List)
        // -------------------------------------------------------------------
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var result = await _userService.GetAllUsersAsync();

            if (result.IsSuccess)
            {
                // Trả về danh sách User cho View (dùng result.Data)
                return View(result.Data);
            }

            // Xử lý khi thất bại (ví dụ: lỗi kết nối database)
            // Có thể thêm thông báo lỗi vào ViewData hoặc TempData
            TempData["ErrorMessage"] = result.Message;
            return View(new List<User>()); // Trả về View với danh sách rỗng
        }

        // -------------------------------------------------------------------
        // 2. ACTION: Hiển thị chi tiết (READ - Single Item)
        // -------------------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var result = await _userService.GetUserByIdAsync(id.Value);

            if (!result.IsSuccess || result.Data == null) return NotFound();

            return View(result.Data);
        }

        // -------------------------------------------------------------------
        // 3. ACTION: Hiển thị Form Tạo mới (CREATE - GET)
        // -------------------------------------------------------------------
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // -------------------------------------------------------------------
        // 4. ACTION: Xử lý dữ liệu Form Tạo mới (CREATE - POST)
        // -------------------------------------------------------------------
        [HttpPost]
        //[ValidateAntiForgeryToken] // Bảo vệ chống tấn công CSRF
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(user);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Tạo User mới thành công!";
                    return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách
                }

                // Nếu Service trả về lỗi (ví dụ: Username đã tồn tại)
                ModelState.AddModelError("", result.Message ?? "Tạo User thất bại.");
            }

            // Nếu dữ liệu Model không hợp lệ hoặc Service lỗi, quay lại form
            return View(user);
        }

        // -------------------------------------------------------------------
        // 5. ACTION: Hiển thị Form Chỉnh sửa (EDIT - GET)
        // -------------------------------------------------------------------
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var result = await _userService.GetUserByIdAsync(id.Value);

            if (!result.IsSuccess || result.Data == null)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        // -------------------------------------------------------------------
        // 6. ACTION: Xử lý dữ liệu Form Chỉnh sửa (EDIT - POST)
        // -------------------------------------------------------------------
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
               
                var result = await _userService.EditUserAsync(user);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "Cập nhật User thành công!";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", result.Message ?? "Cập nhật User thất bại.");
            }

            return View(user);
        }
        // -------------------------------------------------------------------
        // 7. ACTION: Xóa dữ liệu User(POST - ID)
        // -------------------------------------------------------------------
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Message ?? "Xóa User thất bại!";
            }
            else
            {
                TempData["SuccessMessage"] = "Xóa User thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------
        // 8. ACTION: Khóa người dùng (LOCK - GET ID)
        // -------------------------------------------------------------------
        [HttpGet("lock/{id}")]
        public async Task<IActionResult> Lock(int id)
        {
            var result = await _userService.LockUserAsync(id);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message ?? "Khóa User thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Khóa User thất bại!";
            }
            return RedirectToAction(nameof(Index));
        }

        // -------------------------------------------------------------------
        // 9. ACTION: Khôi phục người dùng (RESTORE - GET ID)
        // -------------------------------------------------------------------
        [HttpGet("restore/{id}")]
        public async Task<IActionResult> Restore(int id)
        {
            var result = await _userService.RestoreUserAsync(id);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = result.Message ?? "Khôi phục User thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Khôi phục User thất bại!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
