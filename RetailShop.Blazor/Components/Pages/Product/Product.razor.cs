using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;

namespace RetailShop.Blazor.Components.Pages.Product;

public partial class Product
{
    [Inject]
    private IProductService _productService {  get; set; }
    private ICategoryService _categoryService { get; set; }



    private List<ProductDTO> allProducts = new();
    private IEnumerable<ProductDTO> filteredProducts = new List<ProductDTO>();
    private IEnumerable<ProductDTO> paginatedProducts = new List<ProductDTO>();
    private int currentPage = 1;
    private int pageSize = 12;
    private int totalPages = 0;
    private bool isLoading = true;

    // Filter states
    private string searchQuery = string.Empty;
    private HashSet<string> selectedCategories = new();
    private HashSet<string> selectedSuppliers = new();
    private decimal? minPrice;
    private decimal? maxPrice;
    private bool showActive = true;
    private bool showInactive = true;
    private string sortBy = "name-asc";

    // Available filter options
    private List<string> categories = new();
    private List<string> suppliers = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
    }

    private async Task LoadProducts()
    {
        isLoading = true;

        // TODO: Replace with actual API call
        //allProducts = await Http.GetFromJsonAsync<List<ProductDTO>>("api/products");

        if (_productService.GetAllProductsAsync() == null)
        {
            allProducts = new List<ProductDTO>();
            return;
        }

        var rs = await _productService.GetAllProductsAsync();
        if (rs != null && rs.Result != null)
        {
            try
            {
                var list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(rs.Result));
                allProducts = list ?? new List<ProductDTO>();
            }
            catch
            {
                allProducts = new List<ProductDTO>();
            }
        }
        else
        {
            allProducts = new List<ProductDTO>();
        }

        //// Mock data for demo
        //allProducts = GenerateMockProducts();

        // Extract unique categories and suppliers
        categories = allProducts
            .Where(p => !string.IsNullOrEmpty(p.CategoryName))
            .Select(p => p.CategoryName!)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        suppliers = allProducts
            .Where(p => !string.IsNullOrEmpty(p.SupplierName))
            .Select(p => p.SupplierName!)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        ApplyFilters();
        isLoading = false;
    }

    private void OnSearchChanged()
    {
        ApplyFilters();
    }

    private void ToggleCategory(string category)
    {
        if (selectedCategories.Contains(category))
            selectedCategories.Remove(category);
        else
            selectedCategories.Add(category);

        ApplyFilters();
    }

    private void ToggleSupplier(string supplier)
    {
        if (selectedSuppliers.Contains(supplier))
            selectedSuppliers.Remove(supplier);
        else
            selectedSuppliers.Add(supplier);

        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var query = allProducts.AsEnumerable();

        // Search filter
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            query = query.Where(p =>
                p.ProductName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                (p.CategoryName?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (p.SupplierName?.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ?? false)
            );
        }

        // Category filter
        if (selectedCategories.Any())
        {
            query = query.Where(p => selectedCategories.Contains(p.CategoryName ?? ""));
        }

        // Supplier filter
        if (selectedSuppliers.Any())
        {
            query = query.Where(p => selectedSuppliers.Contains(p.SupplierName ?? ""));
        }

        // Price range filter
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }
        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        // Status filter
        if (showActive && !showInactive)
        {
            query = query.Where(p => p.Active);
        }
        else if (!showActive && showInactive)
        {
            query = query.Where(p => !p.Active);
        }

        // Sorting
        query = sortBy switch
        {
            "name-asc" => query.OrderBy(p => p.ProductName),
            "name-desc" => query.OrderByDescending(p => p.ProductName),
            "price-asc" => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderBy(p => p.ProductName)
        };

        filteredProducts = query.ToList();

        currentPage = 1;
        ApplyPagination();
    }

    private void ApplyPagination()
    {
        totalPages = (int)Math.Ceiling(filteredProducts.Count() / (double)pageSize);
        paginatedProducts = filteredProducts
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private void GoToPage(int page)
    {
        if (page >= 1 && page <= totalPages)
        {
            currentPage = page;
            ApplyPagination();
        }
    }

    private void GoToNextPage()
    {
        if (currentPage < totalPages)
        {
            currentPage++;
            ApplyPagination();
        }
    }

    private void GoToPreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            ApplyPagination();
        }
    }

    private List<int> GetPageNumbers()
    {
        var pages = new List<int>();

        if (totalPages <= 7)
        {
            // Show all pages if 7 or less
            pages.AddRange(Enumerable.Range(1, totalPages));
        }
        else
        {
            // Always show first page
            pages.Add(1);

            if (currentPage <= 4)
            {
                // Near start: 1 2 3 4 5 ... last
                pages.AddRange(Enumerable.Range(2, 4));
                pages.Add(-1); // Ellipsis
                pages.Add(totalPages);
            }
            else if (currentPage >= totalPages - 3)
            {
                // Near end: 1 ... last-4 last-3 last-2 last-1 last
                pages.Add(-1); // Ellipsis
                pages.AddRange(Enumerable.Range(totalPages - 4, 5));
            }
            else
            {
                // Middle: 1 ... current-1 current current+1 ... last
                pages.Add(-1); // Ellipsis
                pages.AddRange(Enumerable.Range(currentPage - 1, 3));
                pages.Add(-1); // Ellipsis
                pages.Add(totalPages);
            }
        }

        return pages;
    }

    private bool HasActiveFilters()
    {
        return !string.IsNullOrWhiteSpace(searchQuery) ||
               selectedCategories.Any() ||
               selectedSuppliers.Any() ||
               minPrice.HasValue ||
               maxPrice.HasValue ||
               !(showActive && showInactive);
    }

    private void ClearFilters()
    {
        searchQuery = string.Empty;
        selectedCategories.Clear();
        selectedSuppliers.Clear();
        minPrice = null;
        maxPrice = null;
        showActive = true;
        showInactive = true;
        sortBy = "name-asc";
        ApplyFilters();
    }

    private List<ProductDTO> GenerateMockProducts()
    {
        var random = new Random();
        var categories = new[] { "Electronics", "Clothing", "Home & Garden", "Sports", "Books", "Toys" };
        var suppliers = new[] { "Tech Corp", "Fashion Inc", "Home Depot", "Sports World", "Book Store" };

        return Enumerable.Range(1, 50).Select(i => new ProductDTO
        {
            ProductId = i,
            ProductName = $"Product {i}",
            ProductImage = $"/placeholder.svg?height=400&width=400",
            CategoryId = random.Next(1, 7),
            CategoryName = categories[random.Next(categories.Length)],
            SupplierId = random.Next(1, 6),
            SupplierName = suppliers[random.Next(suppliers.Length)],
            Price = (decimal)(random.Next(10, 500) + random.NextDouble()),
            Unit = "pcs",
            Active = random.Next(0, 10) > 1, // 90% active
            Quantity = random.Next(0, 100),
            CreatedAt = DateTime.Now.AddDays(-random.Next(0, 365)),
            Barcode = $"BAR{i:D8}"
        }).ToList();
    }
}
