using Microsoft.EntityFrameworkCore;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Models;
using RetailShop.API.Services.IServices;
using BCrypt.Net;

namespace RetailShop.API.Services
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly AppDbContext _context;

        public CustomerAuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> RegisterAsync(CustomerRegisterDto registerDto)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                if (await CustomerExistsAsync(registerDto.Email))
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Email đã được sử dụng!"
                    };
                }

                // Mã hóa password bằng BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Tạo customer mới
                var customer = new Customer
                {
                    Name = registerDto.Name,
                    Phone = registerDto.Phone,
                    Email = registerDto.Email,
                    Password = hashedPassword,
                    Address = registerDto.Address,
                    CreatedAt = DateTime.Now,
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Đăng ký thành công!",
                    Result = new CustomerDto
                    {
                        CustomerId = customer.CustomerId,
                        Name = customer.Name,
                        Email = customer.Email,
                        Phone = customer.Phone,
                        Address = customer.Address,
                        CreatedAt = customer.CreatedAt,
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

        public async Task<ResponseDto> LoginAsync(CustomerLoginDto loginDto)
        {
            try
            {
                // Tìm customer theo email
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Email == loginDto.Email);

                if (customer == null)
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Email không tồn tại hoặc tài khoản đã bị khóa!"
                    };
                }

                // Kiểm tra password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, customer.Password))
                {
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Mật khẩu không đúng!"
                    };
                }

                // Tạo response với thông tin customer
                var loginResponse = new CustomerLoginResponseDto
                {
                    CustomerId = customer.CustomerId,
                    Name = customer.Name,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    Token = "customer_token_" + customer.CustomerId,
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

        public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                var customer = await _context.Customers
                    .Where(c => c.CustomerId == customerId)
                    .Select(c => new CustomerDto
                    {
                        CustomerId = c.CustomerId,
                        Name = c.Name,
                        Email = c.Email,
                        Phone = c.Phone,
                        Address = c.Address,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                return customer;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CustomerExistsAsync(string email)
        {
            return await _context.Customers
                .AnyAsync(c => c.Email == email);
        }
    }
}
