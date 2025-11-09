using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers;

[Route("product")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ISupplierService _supplierService;
    private readonly ICategoryService _categoryService;

    public ProductController(IProductService productService, ISupplierService supplierService, ICategoryService categoryService)
    {
        _productService = productService;
        _supplierService = supplierService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(bool active = true)
    {
        var rs = await _productService.GetAllProductsAsync(active);
        if (rs.IsSuccess)
        {
            ViewBag.Products = rs.Data;
        }
        else
        {
            TempData["err"] = "Lấy danh sách sản phẩm thất bại: " + rs.Message;
            ViewBag.Products = new List<Product>();
        }
        return View();
    }

    private async Task<string> SaveImageAsync(IFormFile imageFile, string oldImagePath = null, int? currentProductId = null)
    {
        string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Admin/dist/img");
        string defaultImage = "/Admin/dist/img/default-150x150.png";

        // Nếu không có file mới → giữ ảnh cũ hoặc dùng ảnh mặc định
        if (imageFile == null || imageFile.Length == 0)
            return oldImagePath ?? defaultImage;

        // Tạo thư mục nếu chưa tồn tại
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        // Tạo tên file duy nhất
        string extension = Path.GetExtension(imageFile.FileName);
        string uniqueFileName = $"{Guid.NewGuid()}{extension}";
        string filePath = Path.Combine(uploadFolder, uniqueFileName);

        // Lưu ảnh vào thư mục wwwroot
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        string newImagePath = "/Admin/dist/img/" + uniqueFileName;

        // Nếu có ảnh cũ, xóa ảnh đó nếu không còn được sản phẩm khác sử dụng
        if (!string.IsNullOrWhiteSpace(oldImagePath) &&
            oldImagePath != defaultImage)
        {
            var allProducts = await _productService.GetAllProductsAsync();

            if (allProducts.IsSuccess && allProducts.Data != null)
            {
                bool usedByOthers = allProducts.Data.Any(p =>
                    p.ProductImage == oldImagePath &&
                    p.ProductId != currentProductId);

                if (!usedByOthers)
                {
                    string oldPhysicalPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        oldImagePath.TrimStart('/')
                    );

                    if (System.IO.File.Exists(oldPhysicalPath))
                        System.IO.File.Delete(oldPhysicalPath);
                }
            }
        }

        return newImagePath;
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        var model = new Product();
        return View("Create", model);
    }

    [HttpPost]
    [Route("store")]
    public async Task<IActionResult> Store(Product product)
    {
        if (!ModelState.IsValid)
        {
            TempData["err"] = "Thêm thất bại: " + string.Join(", ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            await LoadDropdowns(product.SupplierId, product.CategoryId);
            return View("Create", product);
        }

        // Gán ngày tạo
        product.CreatedAt = DateTime.Now;

        // Xử lý ảnh (trả về đường dẫn ảnh hợp lệ)
        product.ProductImage = await SaveImageAsync(product.ImageFile);

        var result = await _productService.CreateProductAsync(product);
        if (result.IsSuccess)
        {
            TempData["success"] = "Thêm thành công";
            return RedirectToAction("Index");
        }

        TempData["err"] = "Thêm thất bại: " + result.Message;
        await LoadDropdowns(product.SupplierId, product.CategoryId);
        return View("Create", product);
    }

        [HttpGet]
        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var rs = await _productService.GetProductByIdAsync(id);
        
            if (rs.IsSuccess && rs.Data != null)
            {
                rs.Data.Quantity = rs.Data.Inventories.FirstOrDefault()?.Quantity ?? 0;
                await LoadDropdowns(rs.Data.SupplierId, rs.Data.CategoryId);
                return View("Edit", rs.Data);
            }

            TempData["err"] = "Không tìm thấy sản phẩm hoặc lỗi: " + rs.Message;
            return RedirectToAction("Index");
        }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            TempData["err"] = "Cập nhật thất bại: " + string.Join(", ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            await LoadDropdowns(product.SupplierId, product.CategoryId);
            return View("Edit", product);
        }

        var existing = await _productService.GetProductByIdAsync(product.ProductId);
        if (!existing.IsSuccess || existing.Data == null)
        {
            TempData["err"] = "Không tìm thấy sản phẩm để cập nhật.";
            return RedirectToAction("Index");
        }

        // Gọi hàm xử lý ảnh, tự động giữ ảnh cũ hoặc xóa ảnh cũ nếu cần
        product.ProductImage = await SaveImageAsync(
            product.ImageFile,
            existing.Data.ProductImage,
            product.ProductId
        );

        var rs = await _productService.UpdateProductAsync(product);
        if (rs.IsSuccess)
        {
            TempData["success"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }

        TempData["err"] = "Cập nhật thất bại: " + rs.Message;
        await LoadDropdowns(product.SupplierId, product.CategoryId);
        return View("Edit", product);
    }

    [HttpPost]
    [Route("delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var rs = await _productService.DeleteProductAsync(id);
        if (rs.IsSuccess)
        {
            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Index");
        }

        TempData["err"] = "Xóa thất bại: " + rs.Message;
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("restore/{id}")]
    public async Task<IActionResult> Restore(int id)
    {
        var rs = await _productService.RestoreProductAsync(id);
        if (rs.IsSuccess)
        {
            TempData["success"] = rs.Message;
        }
        else
        {
            TempData["err"] = "Phục hồi thất bại";
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var rs = await _productService.GetProductByIdAsync(id);
        if (rs.IsSuccess)
        {
            return View("Detail", rs.Data);
        }

        TempData["err"] = "Lấy sản phẩm thất bại: " + rs.Message;
        return RedirectToAction("Index");
    }

    private async Task LoadDropdowns(int? selectedSupplierId = null, int? selectedCategoryId = null)
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        var categories = await _categoryService.GetAllCategoriesAsync();

        // Lọc chỉ những cái có Active = true
        var activeSuppliers = suppliers.Data?.Where(s => s.Active == true).ToList() ?? new();
        var activeCategories = categories.Data?.Where(c => c.Active == true).ToList() ?? new();

        ViewBag.Suppliers = new SelectList(activeSuppliers, "SupplierId", "Name", selectedSupplierId);
        ViewBag.Categories = new SelectList(activeCategories, "CategoryId", "CategoryName", selectedCategoryId);
    }
}
