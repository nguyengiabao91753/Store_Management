using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Components.Pages.Auth;

public partial class Login
{
    [Inject] private IAuthService AuthService { get; set; } = default!;

    private LoginModel loginModel = new();
    private bool showPassword = false;
    private bool isLoading = false;
    private string errorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        // Nếu đã đăng nhập rồi thì redirect về home
        if (await AuthService.IsAuthenticatedAsync())
        {
            Nav.NavigateTo("/");
        }
    }

    private void TogglePassword()
    {
        showPassword = !showPassword;
    }

    private async Task HandleLogin()
    {
        if (isLoading) return;
        
        isLoading = true;
        errorMessage = string.Empty;

        try
        {
            var loginDto = new LoginDto
            {
                Username = loginModel.Username,
                Password = loginModel.Password
            };

            var result = await AuthService.LoginAsync(loginDto);

            if (result.IsSuccess)
            {
                // Đăng nhập thành công, chuyển về trang chủ
                Nav.NavigateTo("/", false);
            }
            else
            {
                errorMessage = result.Message ?? "Đăng nhập thất bại";
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

    private void LoginWithGoogle()
    {
        // Add Google OAuth logic
        errorMessage = "Tính năng đăng nhập Google đang được phát triển";
    }

    private void LoginWithFacebook()
    {
        // Add Facebook OAuth logic
        errorMessage = "Tính năng đăng nhập Facebook đang được phát triển";
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3-50 ký tự")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
