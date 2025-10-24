using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers;
[Route("supplier")]
public class SupplierController : Controller
{
    private readonly ISupplierService _supplierService;
    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }
    public async Task<IActionResult> Index()
    {
        var rs = await _supplierService.GetAllSuppliersAsync();
        if (rs.IsSuccess)
        {
            ViewBag.suppliers = rs.Data;
        }
        else
        {
            TempData["err"] = "Lấy danh sách nhà cung cấp thất bại: " + rs.Message;
            ViewBag.Suppliers = new List<Supplier>();
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
    public async Task<IActionResult> Store(Supplier supplier)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["err"] = "Thêm thất bại: " + string.Join(", ", errors);
            return View("Create", supplier);
        }
        var result = await _supplierService.CreateSupplierAsync(supplier);
        if (result.IsSuccess)
        {
            TempData["success"] = "Thêm thành công";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["err"] = "Thêm thất bại: " + result.Message;
            return View("Create", supplier);
        }
    }

    [HttpGet]
    [Route("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var rs = await _supplierService.GetSupplierByIdAsync(id);
        if(rs.IsSuccess)
        {
            return View("Edit", rs.Data);
        }
        TempData["err"] = "Lấy nhà cung cấp thất bại: " + rs.Message;
        return RedirectToAction("Index");

    }

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update(Supplier supplier)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["err"] = "Cập nhật thất bại: " + string.Join(", ", errors);
            return View("Edit", supplier);
        }
        var rs = await _supplierService.UpdateSupplierAsync(supplier);
        if (rs.IsSuccess)
        {
            TempData["success"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["err"] = "Cập nhật thất bại: " + rs.Message;
            return View("Edit", supplier);
        }
    }

    [HttpGet]
    [Route("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var rs = await _supplierService.GetSupplierByIdAsync(id);
        if (rs.IsSuccess)
        {
            return View("Detail", rs.Data);
        }
        TempData["err"] = "Lấy nhà cung cấp thất bại: " + rs.Message;
        return RedirectToAction("Index");
    }
}