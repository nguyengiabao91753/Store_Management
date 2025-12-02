using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices;

public interface IInventoryService
{
    Task<ResponseDto> UpdateProductStock(int productId, int quantity);
}
