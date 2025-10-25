using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.Models;
using RetailShop.Services;
using RetailShop.Services.IServices;

namespace RetailShop.Controllers;
[Route("order")]

public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    public async Task<IActionResult> Index()
    {
        // Handle possible TempData message (from Details redirect)
        if (TempData["err"] != null)
        {
            ViewBag.Error = TempData["err"];
            TempData.Remove("err"); // clear after reading
        }

        var rs = await _orderService.GetAllOrdersAsync();
        if (rs.IsSuccess)
        {
            ViewBag.orders = rs.Data;
        }
        else
        {
            ViewBag.Error = "Lấy danh sách đơn hàng thất bại: " + rs.Message;
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

    [HttpGet]
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

    [HttpPost]
    [Route("update")]
    public async Task<IActionResult> Update (Order order)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["err"] = "Cập nhật thất bại: " + string.Join(", ", errors);
            return View("Edit", order);
        }
        int orderId = order.OrderId;
        var rs = await _orderService.UpdateOrderAsync(order);
        if (rs.IsSuccess)
        {
            TempData["success"] = "Cập nhật thành công";
            return RedirectToAction("Index");
        }
        else
        {
            TempData["err"] = "Cập nhật thất bại: " + rs.Message;
            return View("Edit", order);
        }
    }
}

