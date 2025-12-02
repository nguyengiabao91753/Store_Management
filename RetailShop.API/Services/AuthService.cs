using Microsoft.EntityFrameworkCore;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Models;
using RetailShop.API.Services.IServices;
using BCrypt.Net;

namespace RetailShop.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Kiểm tra username đã tồn tại chưa
                if (await UserExistsAsync(registerDto.Username))
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Username đã tồn tại!"
                    };
                }

                // Mã hóa password bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Tạo user mới
                var user = new User
                {
                    Username = registerDto.Username,
                    Password = hashedPassword,
                    FullName = registerDto.FullName,
                    Role = registerDto.Role ?? "User",
                    CreatedAt = DateTime.Now,
                    Active = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công!",
                    Result = new UserDto
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        FullName = user.FullName,
                        Role = user.Role,
                        CreatedAt = user.CreatedAt,
                        Active = user.Active
                    }
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Đã có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Tìm user theo username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.Active);

                if (user == null)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Username không tồn tại hoặc tài khoản đã bị khóa!"
                    };
                }

                // Kiểm tra password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu không đúng!"
                    };
                }

                // Tạo response với thông tin user
                var loginResponse = new LoginResponseDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Role = user.Role,
                    Token = "simple_token_" + user.UserId, // Token đơn giản cho đồ án
                    Message = "Đăng nhập thành công!"
                };

                return new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Đăng nhập thành công!",
                    Result = loginResponse
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Đã có lỗi xảy ra: " + ex.Message
                };
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Where(u => u.UserId == userId && u.Active)
                    .Select(u => new UserDto
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        FullName = u.FullName,
                        Role = u.Role,
                        CreatedAt = u.CreatedAt,
                        Active = u.Active
                    })
                    .FirstOrDefaultAsync();

                return user;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }
    }
}
