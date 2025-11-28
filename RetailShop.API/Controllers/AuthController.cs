using Microsoft.AspNetCore.Mvc;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        /// <param name="registerDto">Thông tin đăng ký</param>
        /// <returns>Kết quả đăng ký</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không hợp lệ!",
                    Result = ModelState
                });
            }

            var result = await _authService.RegisterAsync(registerDto);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }

        /// <summary>
        /// Đăng nhập vào hệ thống
        /// </summary>
        /// <param name="loginDto">Thông tin đăng nhập</param>
        /// <returns>Token và thông tin user</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Dữ liệu không hợp lệ!",
                    Result = ModelState
                });
            }

            var result = await _authService.LoginAsync(loginDto);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }

        /// <summary>
        /// Lấy thông tin user theo ID
        /// </summary>
        /// <param name="userId">ID của user</param>
        /// <returns>Thông tin user</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy user!"
                });
            }
            
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Message = "Lấy thông tin user thành công!",
                Result = user
            });
        }

        /// <summary>
        /// Kiểm tra username đã tồn tại chưa
        /// </summary>
        /// <param name="username">Username cần kiểm tra</param>
        /// <returns>True nếu đã tồn tại</returns>
        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var exists = await _authService.UserExistsAsync(username);
            
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Message = exists ? "Username đã tồn tại!" : "Username khả dụng!",
                Result = exists
            });
        }
    }
}
