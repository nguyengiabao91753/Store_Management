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
}
