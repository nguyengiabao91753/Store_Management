using RetailShop.Blazor.Data;
using RetailShop.Blazor.Models;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _db;

    public CartService(AppDbContext db)
    {
        _db = db;
    }

    public bool AddToCart(int product, int quantity)
    {
        try {          
            var existingCartItem = _db.Carts.FirstOrDefault(c => c.ProductId == product);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _db.Carts.Update(existingCartItem);
            }
            else
            {
                Cart cart = new Cart
                {
                    ProductId = product,
                    Quantity = quantity,
                    CreatedAt = DateTime.Now
                };
                _db.Carts.Add(cart);
            }
            _db.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool ClearCart(List<int> products)
    {
        try
        {
            var cartItems = _db.Carts.Where(c => products.Contains(c.ProductId)).ToList();
            _db.Carts.RemoveRange(cartItems);
            _db.SaveChanges();
            return true;
        }catch (Exception)
        {
            return false;
        }
    }

    public List<Cart> GetCart()
    {
        var cartItems = _db.Carts.ToList();
        return cartItems;
    }
}
