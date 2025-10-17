﻿using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers;
[Route("product")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    public async Task<IActionResult> Index()
    {
        var rs = await _productService.GetAllProductsAsync();
        if (rs.IsSuccess)
        {
            ViewBag.suppliers = rs.Data;
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
    public IActionResult Create()
    {
        return View("Create");
    }

    [HttpPost]
    [Route("store")]
    public async Task<IActionResult> Store(Product product)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["err"] = "Thêm thất bại: " + string.Join(", ", errors);
            return View("Create", product);
        }
        var result = await _productService.CreateProductAsync(product);
        if (result.IsSuccess)
        {
            TempData["success"] = "Thêm thành công";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["err"] = "Thêm thất bại: " + result.Message;
            return View("Create", product);
        }
        
    }

    [HttpGet]
    [Route("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var rs = await _productService.GetProductByIdAsync(id);
        if(rs.IsSuccess)
        {
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
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["err"] = "Cập nhật thất bại: " + string.Join(", ", errors);
            return View("Edit", product);
        }
        var rs = await _productService.UpdateProductAsync(product);
        if (rs.IsSuccess)
        {
            TempData["success"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["err"] = "Cập nhật thất bại: " + rs.Message;
            return View("Edit", product);
        }
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
}