using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

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
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }
    }
}
