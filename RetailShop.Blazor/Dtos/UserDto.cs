using System.ComponentModel.DataAnnotations;

namespace RetailShop.Blazor.Dtos
{
    // Dành cho đk tài khoản
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [StringLength(20)]
        public string? Role { get; set; } = "staff"; //TẠM THỜI ĐỂ KHÁCH HÀNG LẤY ROLE NHÂN VIÊN
    }

    // Dành cho đăng nhập
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Khi đăng nhập thành công
    public class LoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // Dành cho thông tin user bình thường
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool Active { get; set; }
    }
}
