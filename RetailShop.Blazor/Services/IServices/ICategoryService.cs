using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Services.IServices
{
    public interface ICategoryService 
    {
        Task<ResponseDto> GetAllCategory();
    }
}
