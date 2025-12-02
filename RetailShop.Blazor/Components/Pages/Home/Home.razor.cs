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
        if (_productService == null)
        {
            Products = new List<ProductDTO>();
            return;
        }

        var rs = await _productService.GetAllProductsAsync();

        if (rs != null && rs.Result != null)
        {
            try
            {
                var list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(rs.Result));
                Products = list ?? new List<ProductDTO>();
            }
            catch
            {
                Products = new List<ProductDTO>();
            }
        }
        else
        {
            Products = new List<ProductDTO>();
        }
    }
}
