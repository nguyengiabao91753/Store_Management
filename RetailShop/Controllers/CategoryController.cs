using Microsoft.AspNetCore.Mvc;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers;

[Route("category")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // === READ: Danh sách danh mục ===
    public async Task<IActionResult> Index()
    {
        var rs = await _categoryService.GetAllCategoriesAsync();
        if (rs.IsSuccess)
        {
            ViewBag.Categories = rs.Data;
        }
        else
        {
            TempData["err"] = "Lấy danh sách danh mục thất bại: " + rs.Message;
            ViewBag.Categories = new List<Category>();
        }
        return View();
    }

    // === CREATE: Hiển thị form thêm mới ===
    [HttpGet]
    [Route("create")]
    public IActionResult Create()
    {
        return View("Create");
    }

    // === CREATE: Lưu danh mục mới ===
    [HttpPost]
    [Route("store")]
    public async Task<IActionResult> Store(Category category)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            TempData["err"] = "Thêm thất bại: " + string.Join(", ", errors);
            return View("Create", category);
        }

        var result = await _categoryService.CreateCategoryAsync(category);
        if (result.IsSuccess)
        {
            TempData["success"] = "Thêm danh mục thành công!";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["err"] = "Thêm thất bại: " + result.Message;
            return View("Create", category);
        }
    }

    // === EDIT: Hiển thị form sửa ===
    [HttpGet]
    [Route("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var rs = await _categoryService.GetCategoryByIdAsync(id);
        if (rs.IsSuccess)
        {
            return View("Edit", rs.Data);
        }
        TempData["err"] = "Lấy thông tin danh mục thất bại: " + rs.Message;
        return RedirectToAction("Index");
    }

    // === UPDATE: Lưu cập nhật ===
    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update(Category category)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .ToList();
            TempData["err"] = "Cập nhật thất bại: " + string.Join(", ", errors);
            return View("Edit", category);
        }

        var rs = await _categoryService.UpdateCategoryAsync(category);
        if (rs.IsSuccess)
        {
            TempData["success"] = "Cập nhật thành công!";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["err"] = "Cập nhật thất bại: " + rs.Message;
            return View("Edit", category);
        }
    }

    // === DETAIL: Xem sản phẩm thuộc danh mục ===
    [HttpGet]
    [Route("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var rsCategory = await _categoryService.GetCategoryByIdAsync(id);
        if (!rsCategory.IsSuccess)
        {
            TempData["err"] = "Không tìm thấy danh mục.";
            return RedirectToAction("Index");
        }

        var rsProducts = await _categoryService.GetProductsByCategoryIdAsync(id);

        ViewBag.Products = rsProducts.IsSuccess ? rsProducts.Data : new List<Product>();
        return View("Detail", rsCategory.Data);
    }
}
