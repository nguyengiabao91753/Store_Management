using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Components.Pages.Home;

public partial class Home
{
    [Inject]
    private IProductService _productService { get; set; }

    private List<ProductDTO> Products = new();


    protected override async Task OnInitializedAsync()
    {
        var rs = await _productService.GetAllProductsAsync();
        Products = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(rs.Result));
    }
}
