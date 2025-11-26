using RetailShop.Blazor.Models;

namespace RetailShop.Blazor.Services.IServices;

public interface ICartService
{
    public List<Cart> GetCart();

    public bool AddToCart(int product, int quantity);

    public bool ClearCart(List<int> products);
}
