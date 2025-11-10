using RetailShop.Client.Models;

namespace RetailShop.Client.Services.IServices
{
    public interface IUserPOSService
    {
        Task<User?> GetUserByCredentials(string username, string password);
    }
}
