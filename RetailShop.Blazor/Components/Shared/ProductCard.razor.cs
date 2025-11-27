using Microsoft.AspNetCore.Components;
using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Components.Shared;

public partial class ProductCard
{
    [Parameter] public ProductDTO Product { get; set; } = default!;

    [Inject]
    public NavigationManager Navigation { get; set; } = default!;
    private void ViewProduct()
    {
        Navigation.NavigateTo($"/product/{Product.ProductId}");
    }
}
