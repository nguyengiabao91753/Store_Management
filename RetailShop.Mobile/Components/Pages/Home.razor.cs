using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailShop.Mobile.Components.Pages
{
    public partial class Home
    {
        private string searchQuery = "";
        private string activeTab = "Popular";

        private List<Product> products = new List<Product>
        {
            new Product { Id = 1, Name = "Naurta Chair", Price = 71.00m, ImageUrl = "/placeholder.svg?height=200&width=200" },
            new Product { Id = 2, Name = "Svante Chair", Price = 35.21m, ImageUrl = "/placeholder.svg?height=200&width=200" },
            new Product { Id = 3, Name = "Plant", Price = 71.00m, ImageUrl = "/placeholder.svg?height=200&width=200" },
            new Product { Id = 4, Name = "Gordon Lamp", Price = 71.00m, ImageUrl = "/placeholder.svg?height=200&width=200" }
        };

        private void SetActiveTab(string tab)
        {
            activeTab = tab;
        }

        private void AddToCart(Product product)
        {
            // Add to cart logic here
            Console.WriteLine($"[v0] Added {product.Name} to cart");
        }

        public class Product
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public decimal Price { get; set; }
            public string ImageUrl { get; set; } = "";
        }
    }
}
