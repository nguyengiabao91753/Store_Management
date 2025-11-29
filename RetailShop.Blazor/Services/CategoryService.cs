using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Ubility;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseService _baseService;
        public CategoryService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> GetAllCategory()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ServierAPI + "/api/category/get-categories"
            });
        }
    }
}
