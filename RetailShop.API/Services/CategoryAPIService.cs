using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Services
{
    public class CategoryAPIService : ICategoryAPIService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CategoryAPIService (AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task<ResponseDto> getAllCategoryAsync()
        {
            var response = new ResponseDto();
            try
            {
                var category = _db.Categories
                    .Where(c => c.Active == true)
                    .ToList();
                response.IsSuccess = true;
                response.Result = _mapper.Map<List<CategoryDTO>>(category);
                response.Message = "Lay danh sach category thanh cong";

                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Result = null;
                response.Message = "Loi: "+ ex.Message;
                return response;
            }
        }
    }
}
