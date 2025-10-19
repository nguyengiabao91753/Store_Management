using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services.IServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
[Route("customer")]
public class CustomerController : Controller
{
    private readonly ICustomerService _customerService;

    // Inject service qua constructor
    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // -------------------------------
    // HIỂN THỊ DANH SÁCH KHÁCH HÀNG
    // -------------------------------
    public async Task<IActionResult> Index(string? searchName, string? searchPhone, string? searchEmail)
    {
        var result = await _customerService.SearchCustomersAsync(searchName, searchPhone, searchEmail);

        if (!result.IsSuccess)
        {
            ViewBag.ErrorMessage = result.Message;
            return View(new List<Customer>());
        }

        return View(result.Data);
    }
    // -------------------------------
    // TẠO MỚI KHÁCH HÀNG
    // -------------------------------
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            return Json(new { success = false, message = "Vui lòng nhập tên khách hàng!" });

        if (string.IsNullOrWhiteSpace(customer.Email) && string.IsNullOrWhiteSpace(customer.Phone))
            return Json(new { success = false, message = "Vui lòng nhập email hoặc số điện thoại!" });

        // Regex kiểm tra email hợp lệ
        var emailRegex = new Regex(@"^[\w\.\-]+@([\w\-]+\.)+[\w\-]{2,4}$");
        if (!string.IsNullOrWhiteSpace(customer.Email) && !emailRegex.IsMatch(customer.Email))
            return Json(new { success = false, message = "Email không hợp lệ!" });

        // Regex kiểm tra số điện thoại Việt Nam hợp lệ
        var phoneRegex = new Regex(@"^(0|\+84)[0-9]{9}$");
        if (!string.IsNullOrWhiteSpace(customer.Phone) && !phoneRegex.IsMatch(customer.Phone))
            return Json(new { success = false, message = "Số điện thoại không hợp lệ!" });

        // Kiểm tra trùng trong DB
        var exists = await _customerService.IsEmailOrPhoneExistAsync(customer.Email, customer.Phone);
        if (exists)
            return Json(new { success = false, message = "Khách hàng đã tồn tại!" });

        // Thêm mới
        customer.CreatedAt = DateTime.Now;
        var result = await _customerService.CreateCustomerAsync(customer);

        if (result.IsSuccess)
            return Json(new { success = true, message = "Thêm khách hàng thành công!" });

        return Json(new { success = false, message = "Thêm khách hàng thất bại: " + result.Message });
    }

    // -------------------------------
    // CHỈNH SỬA KHÁCH HÀNG
    // -------------------------------
    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Customer model)
    {
        if (!string.IsNullOrWhiteSpace(model.Email))
        {
            var emailRegex = new Regex(@"^[\w\.\-]+@([\w\-]+\.)+[\w\-]{2,4}$");
            if (!emailRegex.IsMatch(model.Email))
                return Json(new { success = false, message = "Email không hợp lệ!" });
        }

        if (!string.IsNullOrWhiteSpace(model.Phone))
        {
            var phoneRegex = new Regex(@"^(0|\+84)[0-9]{9}$");
            if (!phoneRegex.IsMatch(model.Phone))
                return Json(new { success = false, message = "Số điện thoại không hợp lệ!" });
        }

        var existing = await _customerService.GetCustomerByIdAsync(model.CustomerId);
        if (existing == null)
            return Json(new { success = false, message = "Không tìm thấy khách hàng!" });

        //Kiểm tra trùng email/sdt
        var duplicate = await _customerService.IsEmailOrPhoneExistAsync(model.CustomerId, model.Email, model.Phone);
        if (duplicate)
            return Json(new { success = false, message = "Email hoặc số điện thoại đã tồn tại!" });

        existing.Data.Name = model.Name;
        existing.Data.Phone = model.Phone;
        existing.Data.Email = model.Email;
        existing.Data.Address = model.Address;

        await _customerService.UpdateCustomerAsync(existing.Data);
        return Json(new { success = true, message = "Cập nhật khách hàng thành công!" });
    }

    // -------------------------------
    // XÓA KHÁCH HÀNG
    // -------------------------------
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteCustomerAsync(id);
        if (!result.IsSuccess)
        {
            return Json(new { success = false, message = result.Message });
        }

        return Json(new { success = true, message = "Xóa khách hàng thành công!" });
    }
    
     // -------------------------------
    // KHÔI PHỤC KHÁCH HÀNG
    // -------------------------------
    [HttpPost]
    [Route("restore/{id}")]
    public async Task<IActionResult> Restore(int id)
    {
        var result = await _customerService.RestoreCustomerAsync(id);
        if (!result.IsSuccess)
        {
           return Json(new { success = false, message = result.Message });
        }

        return  Json(new { success = true, message = "Khôi phục khách hàng thành công!" });
    }
}