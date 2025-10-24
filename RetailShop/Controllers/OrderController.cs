using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers;
[Route("orders")]

public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public async Task<IActionResult> Index()
    {
        var rs = await _orderService.GetAllOrdersAsync();
        if (rs.IsSuccess)
        {
            ViewBag.orders = rs.Data;
        }
        else
        {
            TempData["err"] = "Lấy danh sách đơn hàng thất bại: " + rs.Message;
            ViewBag.orders = new List<object>();
        }
        return View();
    }

    [HttpGet]
    [Route("detail/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var rs = await _orderService.GetOrderByIdAsync(id);
        if (rs.IsSuccess)
        {
            return View(rs.Data);
        }
        else
        {
            TempData["err"] = "Lấy thông tin đơn hàng thất bại: " + rs.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Route("edit/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var rs = await _orderService.GetOrderByIdAsync(id);
        if (rs.IsSuccess)
        {
            return View("Edit", rs.Data);
        }
        TempData["err"] = "Lấy đơn hàng thất bại: " + rs.Message;
        return RedirectToAction("Index");
    }

}

