using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Components.Pages.Auth;

public partial class Register
{
    [Inject] private ICustomerAuthService CustomerAuthService { get; set; } = default!;

    private RegisterModel registerModel = new();
    private bool showPassword = false;
    private bool isLoading = false;
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        // Nếu đã đăng nhập rồi thì redirect về home
        if (await CustomerAuthService.IsAuthenticatedAsync())
        {
            Nav.NavigateTo("/", true);
        }
    }

    private void TogglePassword()
    {
        showPassword = !showPassword;
    }

    private int GetPasswordStrength()
    {
        if (string.IsNullOrEmpty(registerModel.Password)) return 0;
        if (registerModel.Password.Length < 6) return 25;
        if (registerModel.Password.Length < 8) return 50;
        if (registerModel.Password.Length < 10) return 75;
        return 100;
    }

    private string GetPasswordStrengthText()
    {
        var strength = GetPasswordStrength();
        if (strength == 0) return "";
        if (strength <= 25) return "Yếu";
        if (strength <= 50) return "Trung bình";
        if (strength <= 75) return "Tốt";
        return "Mạnh";
    }

    private async Task HandleRegister()
    {
        if (isLoading) return;

        isLoading = true;
        errorMessage = string.Empty;
        successMessage = string.Empty;

        try
        {
            var registerDto = new CustomerRegisterDto
            {
                Name = registerModel.Name,
                Phone = registerModel.Phone,
                Email = registerModel.Email,
                Password = registerModel.Password,
                Address = registerModel.Address
            };

            var result = await CustomerAuthService.RegisterAsync(registerDto);

            if (result.IsSuccess)
            {
                successMessage = "Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.";
                
                // Chờ 2s để hiển thị success message rồi chuyển về login
                await Task.Delay(2000);
                Nav.NavigateTo("/login");
            }
            else
            {
                errorMessage = result.Message ?? "Đăng ký thất bại";
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Có lỗi xảy ra: " + ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }

    private void RegisterWithGoogle()
    {
        errorMessage = "Tính năng đăng ký Google đang được phát triển";
    }

    private void RegisterWithFacebook()
    {
        errorMessage = "Tính năng đăng ký Facebook đang được phát triển";
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Địa chỉ không được vượt quá 255 ký tự")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Range(typeof(bool), "true", "true", ErrorMessage = "Bạn phải đồng ý với điều khoản sử dụng")]
        public bool AgreeToTerms { get; set; }
    }
}
