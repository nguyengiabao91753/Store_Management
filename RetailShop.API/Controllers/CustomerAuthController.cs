using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAuthController : ControllerBase
    {
        private readonly ICustomerAuthService _customerAuthService;

        public CustomerAuthController(ICustomerAuthService customerAuthService)
        {
            _customerAuthService = customerAuthService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto>> Register([FromBody] CustomerRegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "Dữ liệu không hợp lệ" 
                });
            }

            var result = await _customerAuthService.RegisterAsync(registerDto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto>> Login([FromBody] CustomerLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "Dữ liệu không hợp lệ" 
                });
            }

            var result = await _customerAuthService.LoginAsync(loginDto);
            return Ok(result);
        }

        [HttpGet("profile/{customerId}")]
        public async Task<ActionResult<CustomerDto>> GetProfile(int customerId)
        {
            var customer = await _customerAuthService.GetCustomerByIdAsync(customerId);
            
            if (customer == null)
            {
                return NotFound(new ResponseDto 
                { 
                    IsSuccess = false, 
                    Message = "Không tìm thấy khách hàng" 
                });
            }

            return Ok(new ResponseDto 
            { 
                IsSuccess = true, 
                Result = customer 
            });
        }

        [HttpGet("check-email/{email}")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            var exists = await _customerAuthService.CustomerExistsAsync(email);
            return Ok(new ResponseDto 
            { 
                IsSuccess = true, 
                Result = exists 
            });
        }
    }
}
