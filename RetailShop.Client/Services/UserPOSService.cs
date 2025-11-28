using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;
using BCrypt.Net;

namespace RetailShop.Client.Services
{
    public class UserPOSService : IUserPOSService
    {
        private readonly AppDbContext _context;

        public UserPOSService(AppDbContext context)
        {
            _context = context;
        }

        // Chỉ lấy user theo username và password
        public async Task<User?> GetUserByCredentials(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return null;

            // Verify the provided password against the stored BCrypt hash
            try
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return user;
                }
            }
            catch
            {
                // If verification fails (invalid hash format etc.), treat as authentication failure
            }

            return null;
        }
    }
}
