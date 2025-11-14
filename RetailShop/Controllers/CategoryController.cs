using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RetailShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var rs = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = rs.Data ?? new List<Category>();
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var rs = await _categoryService.CreateCategoryAsync(model);

            if (!rs.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, rs.Message);
                return View(model);
            }

            TempData["Success"] = rs.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var rs = await _categoryService.GetCategoryByIdAsync(id);
            if (!rs.IsSuccess || rs.Data == null)
            {
                return NotFound();
            }
            return View(rs.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var rs = await _categoryService.UpdateCategoryAsync(model);

            if (!rs.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, rs.Message);
                return View(model);
            }

            TempData["Success"] = rs.Message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var rs = await _categoryService.GetCategoryByIdAsync(id);
            if (!rs.IsSuccess || rs.Data == null)
            {
                return NotFound();
            }
            return View(rs.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var rs = await _categoryService.SoftDeleteCategoryAsync(id);

            if (!rs.IsSuccess)
            {
                TempData["Error"] = rs.Message;
            }
            else
            {
                TempData["Success"] = rs.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
