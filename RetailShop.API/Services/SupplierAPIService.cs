using AutoMapper;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Services
{
    public class SupplierAPIService : ISupplierAPIService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public SupplierAPIService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ResponseDto> getAllSuplierAsync()
        {
            var response = new ResponseDto();
            try
            {
                var supplier = _db.Suppliers
                    .Where(s => s.Active == true)
                    .ToList();

                response.IsSuccess = true;
                response.Result = _mapper.Map<List<SupplierDTO>>(supplier);
                response.Message = "Lay danh sach category thanh cong";

                return response;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Result = null;
                response.Message = "Loi: " + ex.Message;
                return response;
            }
        }
    }
}
