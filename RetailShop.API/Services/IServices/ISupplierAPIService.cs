using RetailShop.API.Dtos;

namespace RetailShop.API.Services.IServices
{
    public interface ISupplierAPIService
    {
        public Task<ResponseDto> getAllSuplierAsync();
    }
}
