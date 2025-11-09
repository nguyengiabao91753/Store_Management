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
    private readonly ICloudinaryService _cloudinaryService;

    public ProductController(
        IProductService productService,
        ISupplierService supplierService,
        ICategoryService categoryService,
        ICloudinaryService cloudinaryService)
    {
        _productService = productService;
        _supplierService = supplierService;
        _categoryService = categoryService;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<IActionResult> Index(bool active = true)
    {
        var rs = await _productService.GetAllProductsAsync(active);
        ViewBag.Products = rs.IsSuccess ? rs.Data : new List<Product>();

        if (!rs.IsSuccess)
            TempData["err"] = "Lấy danh sách sản phẩm thất bại: " + rs.Message;

        return View();
    }

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View("Create", new Product());
    }

    [HttpPost]
    [Route("store")]
    public async Task<IActionResult> Store(Product product)
    {
        if (!ModelState.IsValid)
        {
            TempData["err"] = "Thêm thất bại.";
            await LoadDropdowns(product.SupplierId, product.CategoryId);
            return View("Create", product);
        }

        product.CreatedAt = DateTime.Now;

        if (product.ImageFile != null)
        {
            product.ProductImage = await _cloudinaryService.UploadImageAsync(product.ImageFile, "products");
        }
        else
        {
            product.ProductImage = "/Admin/dist/img/default-150x150.png"; // ảnh mặc định
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

        if (!rs.IsSuccess || rs.Data == null)
        {
            TempData["err"] = "Không tìm thấy sản phẩm.";
            return RedirectToAction("Index");
        }

        rs.Data.Quantity = rs.Data.Inventories.FirstOrDefault()?.Quantity ?? 0;
        await LoadDropdowns(rs.Data.SupplierId, rs.Data.CategoryId);
        return View("Edit", rs.Data);
    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update(Product product)
    {
        if (!ModelState.IsValid)
        {
            TempData["err"] = "Cập nhật thất bại.";
            await LoadDropdowns(product.SupplierId, product.CategoryId);
            return View("Edit", product);
        }

        var existing = await _productService.GetProductByIdAsync(product.ProductId);
        if (!existing.IsSuccess || existing.Data == null)
        {
            TempData["err"] = "Sản phẩm không tồn tại.";
            return RedirectToAction("Index");
        }

        string oldImage = existing.Data.ProductImage;
        if (product.ImageFile != null)
        {
            product.ProductImage = await _cloudinaryService.UploadImageAsync(product.ImageFile, "products");

            if (!string.IsNullOrEmpty(oldImage))
            {
                string publicId = oldImage.Split('/').Last().Split('.').First();
                await _cloudinaryService.DeleteImageAsync(publicId);
            }
        }
        else
        {
            product.ProductImage = oldImage;
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
        TempData[rs.IsSuccess ? "success" : "err"] = rs.IsSuccess ? "Xóa thành công" : "Xóa thất bại";
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("restore/{id}")]
    public async Task<IActionResult> Restore(int id)
    {
        var rs = await _productService.RestoreProductAsync(id);
        TempData[rs.IsSuccess ? "success" : "err"] = rs.IsSuccess ? rs.Message : "Phục hồi thất bại";
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var rs = await _productService.GetProductByIdAsync(id);
        if (!rs.IsSuccess) TempData["err"] = "Lấy sản phẩm thất bại";
        return rs.IsSuccess ? View("Detail", rs.Data) : RedirectToAction("Index");
    }

    private async Task LoadDropdowns(int? selectedSupplierId = null, int? selectedCategoryId = null)
    {
        var suppliers = await _supplierService.GetAllSuppliersAsync();
        var categories = await _categoryService.GetAllCategoriesAsync();

        ViewBag.Suppliers = new SelectList(suppliers.Data?.Where(s => s.Active == true), "SupplierId", "Name", selectedSupplierId);
        ViewBag.Categories = new SelectList(categories.Data?.Where(c => c.Active == true), "CategoryId", "CategoryName", selectedCategoryId);
    }
}
