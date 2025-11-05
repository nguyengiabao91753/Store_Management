using RetailShop.Dtos;
    using RetailShop.Models;

    namespace RetailShop.Services.IServices;

    public interface IUserService
    {
        Task<ResultService<List<User>>> GetAllUsersAsync();
        Task<ResultService<User>> GetUserByIdAsync(int id);
        Task<ResultService<User>> CreateUserAsync(User user);
        Task<ResultService<User>> EditUserAsync(User user);
        Task<ResultService<User>> DeleteUserAsync(int id);
        Task<ResultService<User>> LockUserAsync(int id);
        Task<ResultService<User>> RestoreUserAsync(int id);
}