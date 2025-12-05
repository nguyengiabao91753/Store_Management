using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers;
[Route("api/supplier")]
[ApiController]
public class SupplierController : ControllerBase
{
    private readonly ISupplierAPIService _supplierAPIService;
    public SupplierController (ISupplierAPIService supplierAPIService)
    {
        _supplierAPIService = supplierAPIService;
    }

    [HttpGet("get-supplier")]
    public async Task<IActionResult> GetAllSupplier()
    {
        var result = await _supplierAPIService.getAllSuplierAsync();
        if (result.IsSuccess)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }
}