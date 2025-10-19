using Microsoft.EntityFrameworkCore;
using RetailShop.Data; // AppDbContext nằm ở đây
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;


namespace RetailShop.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        // Dependency Injection
        public UserService(AppDbContext db)
        {
            _db = db;
        }

        // =========================================================
        // 1. GET ALL USERS
        // =========================================================
        public async Task<ResultService<List<User>>> GetAllUsersAsync()
        {
            var rs = new ResultService<List<User>>();
            try
            {
                // Load danh sách Users (có thể bỏ qua Orders để tối ưu)
                var users = await _db.Users.ToListAsync();

                rs.IsSuccess = true;
                rs.Data = users;
                rs.Message = "Danh sách User được tải thành công.";
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Message = $"Lỗi khi tải danh sách User: {ex.Message}";
            }
            return rs;
        }

        // =========================================================
        // 2. GET USER BY ID
        // =========================================================
        public async Task<ResultService<User>> GetUserByIdAsync(int id)
        {
            var rs = new ResultService<User>();
            try
            {
                // Sử dụng .Include(u => u.Orders) nếu cần tải kèm danh sách đơn hàng
                var user = await _db.Users.FindAsync(id);

                if (user == null)
                {
                    rs.IsSuccess = false;
                    rs.Message = "Không tìm thấy người dùng.";
                }
                else
                {
                    rs.IsSuccess = true;
                    rs.Data = user;
                    rs.Message = "Thông tin User được tải thành công.";
                }
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Message = $"Lỗi khi tìm User: {ex.Message}";
            }
            return rs;
        }

        // =========================================================
        // 3. CREATE USER
        // =========================================================
        public async Task<ResultService<User>> CreateUserAsync(User user)
        {
            var rs = new ResultService<User>();
            try
            {
                // kiểm tra Username đã tồn tại chưa?
                var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
                if (existingUser != null)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"Username '{user.Username}' đã tồn tại. Vui lòng chọn Username khác.";
                    return rs;
                }

                //Mã hóa Mật khẩu
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

                // Thiết lập Role mặc định nếu Controller không gửi Role hợp lệ
                if (string.IsNullOrEmpty(user.Role) || (user.Role != "admin" && user.Role != "staff"))
                {
                    user.Role = "staff";
                }

                // Thiết lập CreatedAt
                user.CreatedAt = DateTime.Now;

                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();

                rs.IsSuccess = true;
                rs.Data = user;
                rs.Message = "Tạo người dùng mới thành công.";
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Message = $"Lỗi khi tạo người dùng: {ex.Message}";
            }
            return rs;
        }

        // =========================================================
        // 4. EDIT USER
        // =========================================================
        public async Task<ResultService<User>> EditUserAsync(User user)
        {
            var rs = new ResultService<User>();
            try
            {
                //xác nhận có người dùng cần cập nhật
                var existingUser = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == user.UserId);
                if (existingUser == null)
                {
                    rs.IsSuccess = false;
                    rs.Message = "Không tìm thấy người dùng cần cập nhật.";
                    return rs;
                }

                //Username đã bị người khác sử dụng chưa?
                var duplicateUserName = await _db.Users.AnyAsync(u => u.Username == user.Username && u.UserId != user.UserId);

                if (duplicateUserName)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"Username '{user.Username}' đã được người dùng khác sử dụng.";
                    return rs;
                }

                //Nếu user.Password rỗng, giữ mật khẩu cũ
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = existingUser.Password;
                }
                else
                {
                    // Nếu có mật khẩu mới, phải mã hóa
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                
                _db.Users.Update(user);


                await _db.SaveChangesAsync();

                rs.IsSuccess = true;
                rs.Data = user;
                rs.Message = "Cập nhật người dùng thành công.";
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Message = $"Lỗi khi cập nhật người dùng: {ex.Message}";
            }
            return rs;
        }
    }
}