using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Ubility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailShop.Blazor.Services;
public class ProductService : IProductService
{
    private readonly IBaseService _baseService;
    public ProductService(IBaseService baseService)
    {
        _baseService = baseService;
    }
    public async Task<ResponseDto?> GetAllProductsAsync()
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,
            Url = SD.ServierAPI + "/api/product/get-products"
        });
    }

    public async Task<ResponseDto?> GetProductById(int productId)
    {
        return await _baseService.SendAsync(new RequestDto()
        {
            ApiType = SD.ApiType.GET,

            Url = SD.ServierAPI + $"/api/product/get-product-by-id/{productId}"
        });
    }
}
