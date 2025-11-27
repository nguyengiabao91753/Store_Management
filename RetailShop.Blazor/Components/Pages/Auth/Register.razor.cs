using System.ComponentModel.DataAnnotations;

namespace RetailShop.Blazor.Components.Pages.Auth;

public partial class Register
{
    private RegisterModel registerModel = new();
    private bool showPassword = false;
    private bool isLoading = false;

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
        if (strength <= 25) return "Weak";
        if (strength <= 50) return "Fair";
        if (strength <= 75) return "Good";
        return "Strong";
    }

    private async Task HandleRegister()
    {
        isLoading = true;
        await Task.Delay(1500); // Simulate API call
        // Add your registration logic here
        Nav.NavigateTo("/");
    }

    private void RegisterWithGoogle()
    {
        // Add Google OAuth logic
    }

    private void RegisterWithFacebook()
    {
        // Add Facebook OAuth logic
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the terms and conditions")]
        public bool AgreeToTerms { get; set; }
    }
}
