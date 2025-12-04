using System.ComponentModel.DataAnnotations;

namespace RetailShop.Blazor.Dtos
{
    // Dành cho đăng ký khách hàng
    public class CustomerRegisterDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Address { get; set; }
    }

    // Dành cho đăng nhập khách hàng
    public class CustomerLoginDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Khi đăng nhập thành công
    public class CustomerLoginResponseDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // Dành cho thông tin khách hàng bình thường
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
