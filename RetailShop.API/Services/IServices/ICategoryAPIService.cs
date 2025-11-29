using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices
{
    public interface ICategoryAPIService
    {
        public Task<ResponseDto> getAllCategoryAsync();
    }
}
