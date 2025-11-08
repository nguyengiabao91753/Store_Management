using RetailShop.Dtos;
using RetailShop.Models;

namespace RetailShop.Services.IServices;
    public interface IPromotionService
    {
        Task<ResultService<List<Promotion>>> GetAllPromotionsAsync(Boolean active);
        Task<ResultService<Promotion>> GetPromotionByIdAsync(int id);
        Task<ResultService<Promotion>> CreatePromotionAsync(Promotion promotion);
        Task<ResultService<Promotion>> UpdatePromotionAsync(Promotion promotion);
        Task<ResultService<Promotion>> DeletePromotionAsync(int id);
        Task<ResultService<Promotion>> RestoreSupplierAsync(int id);

    
}
