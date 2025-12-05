using System.ComponentModel.DataAnnotations;

namespace RetailShop.API.Dtos
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = null!;

        public bool Active { get; set; } = true;
    }
}
