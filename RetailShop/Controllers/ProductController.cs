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

    public async Task<IActionResult> Index()
    {
        var rs = await _productService.GetAllProductsAsync();
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

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View("Create");
    }

    [HttpPost]
    [Route("store")]
    public async Task<IActionResult> Store(Product product)
    {
        //Nếu người dùng để trống ProductImage → gán ảnh mặc định
        if (string.IsNullOrWhiteSpace(product.ProductImage))
        {
            product.ProductImage = @"No Image";
        }
        if (string.IsNullOrWhiteSpace(product.ProductImage))
        {
            product.CreatedAt = DateTime.Now;
        }
        if (!ModelState.IsValid)
        {
            TempData["err"] = "Thêm thất bại: " + string.Join(", ",
                ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            await LoadDropdowns(product.SupplierId, product.CategoryId);
            return View("Create", product);
        }

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
        if (rs.IsSuccess)
        {
            await LoadDropdowns(rs.Data.SupplierId, rs.Data.CategoryId);
            return View("Edit", rs.Data);
        }

        TempData["err"] = "Lấy sản phẩm thất bại: " + rs.Message;
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
