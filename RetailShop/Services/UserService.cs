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
        public async Task<ResultService<List<User>>> GetAllUsersAsync(bool includeAdmin = true)
        {
            var rs = new ResultService<List<User>>();
            try
            {
                // 1. Khởi tạo truy vấn
                var query = _db.Users.AsQueryable();

                // 2. Áp dụng điều kiện lọc nếu không muốn bao gồm Admin
                if (!includeAdmin)
                {
                    // Chỉ lấy những user có Role KHÔNG phải là 'admin'
                    query = query.Where(u => u.Role != "admin");
                }

                // 3. Thực thi truy vấn và tải danh sách
                var users = await query.ToListAsync();

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
                // 1. Kiểm tra username trùng lặp
                // Controller đã truyền đối tượng 'user' (là existingUser) 
                // với Username cũ (không thay đổi trên form Edit)
                var duplicateUserName = await _db.Users
                    .AnyAsync(u => u.Username == user.Username && u.UserId != user.UserId);

                if (duplicateUserName)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"Username '{user.Username}' đã được người dùng khác sử dụng.";
                    return rs;
                }

                // 2. LƯU THAY ĐỔI
                // Dùng Update() để đảm bảo EF biết đối tượng này đã được sửa
                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                rs.IsSuccess = true;
                rs.Data = user;
                rs.Message = "Cập nhật người dùng thành công.";
            }
            catch (Exception ex)
            {
                //...
                rs.IsSuccess = false;
                rs.Message = $"Lỗi khi cập nhật người dùng: {ex.Message}";
            }
            return rs;
        }

        // =========================================================
        // 5. DELETE USER
        // =========================================================
        public async Task<ResultService<User>> DeleteUserAsync(int id)
        {
            var rs = new ResultService<User>();
            try
            {
                // 1. Tìm User cần xóa
                var userToDelete = await _db.Users.FindAsync(id);

                if (userToDelete == null)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"Không tìm thấy User với ID = {id} cần xóa.";
                    return rs;
                }

                // 2. Thực hiện xóa
                _db.Users.Remove(userToDelete);

                // 3. Lưu thay đổi vào database
                await _db.SaveChangesAsync();

                rs.IsSuccess = true;
                rs.Message = $"User '{userToDelete.FullName}' đã được xóa thành công.";
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Message = $"Xóa User thất bại do lỗi hệ thống: {ex.Message}";
            }
            return rs;
        }

        // =========================================================
        // 6. LOCK USER (Set Active = false)
        // =========================================================
        public async Task<ResultService<User>> LockUserAsync(int id)
        {
            var rs = new ResultService<User>();
            try
            {
                var userToLock = await _db.Users.FindAsync(id);

                if (userToLock == null)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"Không tìm thấy User với ID = {id} cần khóa.";
                    return rs;
                }

                if (!userToLock.Active)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"User '{userToLock.FullName}' đã bị khóa.";
                    return rs;
                }

                // Thực hiện Khóa (Active = false)
                userToLock.Active = false;

                // Cập nhật trạng thái
                _db.Users.Update(userToLock);
                await _db.SaveChangesAsync();

                rs.IsSuccess = true;
                rs.Message = $"User '{userToLock.FullName}' đã được khóa thành công.";
                rs.Data = userToLock;
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Message = $"Khóa User thất bại do lỗi hệ thống: {ex.Message}";
            }
            return rs;
        }

        // =========================================================
        // 7. RESTORE USER (Set Active = true)
        // =========================================================
        public async Task<ResultService<User>> RestoreUserAsync(int id)
        {
            var rs = new ResultService<User>();
            try
            {
                var userToRestore = await _db.Users.FindAsync(id);

                if (userToRestore == null)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"Không tìm thấy User với ID = {id} cần khôi phục.";
                    return rs;
                }

                if (userToRestore.Active)
                {
                    rs.IsSuccess = false;
                    rs.Message = $"User '{userToRestore.FullName}' đã được Khôi phục.";
                    return rs;
                }

                // Thực hiện Khôi phục (Active = true)
                userToRestore.Active = true;

                // Cập nhật trạng thái
                _db.Users.Update(userToRestore);
                await _db.SaveChangesAsync();

                rs.IsSuccess = true;
                rs.Message = $"User '{userToRestore.FullName}' đã được khôi phục thành công.";
                rs.Data = userToRestore;
            }
            catch (Exception ex)
            {
                rs.IsSuccess = false;
                rs.Message = $"Khôi phục User thất bại do lỗi hệ thống: {ex.Message}";
            }
            return rs;
        }
    }
}