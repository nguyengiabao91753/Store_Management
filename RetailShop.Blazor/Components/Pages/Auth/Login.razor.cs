using System.ComponentModel.DataAnnotations;

namespace RetailShop.Blazor.Components.Pages.Auth;

public partial class Login
{
    private LoginModel loginModel = new();
    private bool showPassword = false;
    private bool isLoading = false;

    private void TogglePassword()
    {
        showPassword = !showPassword;
    }

    private async Task HandleLogin()
    {
        isLoading = true;
        await Task.Delay(1500); // Simulate API call
        // Add your login logic here
        Nav.NavigateTo("/");
    }

    private void LoginWithGoogle()
    {
        // Add Google OAuth logic
    }

    private void LoginWithFacebook()
    {
        // Add Facebook OAuth logic
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
