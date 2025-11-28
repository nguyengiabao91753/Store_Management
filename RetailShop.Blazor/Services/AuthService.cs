using Microsoft.JSInterop;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;
using System.Text.Json;

namespace RetailShop.Blazor.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        private readonly IJSRuntime _jsRuntime;
        private UserDto? _currentUser;
        private string? _token;
        public event Action? OnAuthStateChanged;

        public AuthService(IBaseService baseService, IJSRuntime jsRuntime)
        {
            _baseService = baseService;
            _jsRuntime = jsRuntime;
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(_token);
        public UserDto? CurrentUser => _currentUser;
        public string? Token => _token;

        public async Task<ResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDto
                {
                    ApiType = Ubility.SD.ApiType.POST,
                    Data = registerDto,
                    Url = Ubility.SD.ServierAPI + "/api/auth/register"
                });

                return response ?? new ResponseDto { IsSuccess = false, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Lỗi khi đăng ký: " + ex.Message
                };
            }
        }

        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDto
                {
                    ApiType = Ubility.SD.ApiType.POST,
                    Data = loginDto,
                    Url = Ubility.SD.ServierAPI + "/api/auth/login"
                });

                if (response != null && response.IsSuccess && response.Result != null)
                {
                    // Parse LoginResponseDto từ response.Result
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(
                        response.Result.ToString()!,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (loginResponse != null)
                    {
                        // Lưu thông tin user và token
                        _currentUser = new UserDto
                        {
                            UserId = loginResponse.UserId,
                            Username = loginResponse.Username,
                            FullName = loginResponse.FullName,
                            Role = loginResponse.Role,
                            Active = true
                        };
                        _token = loginResponse.Token;

                        // Lưu vào localStorage
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", 
                            JsonSerializer.Serialize(_currentUser));

                        await _jsRuntime.InvokeVoidAsync("console.log", $"[Blazor] Login thành công: user={_currentUser?.Username}, token={_token}");

                        // Notify subscribers that auth state changed
                        OnAuthStateChanged?.Invoke();
                    }
                }

                return response ?? new ResponseDto { IsSuccess = false, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Lỗi khi đăng nhập: " + ex.Message
                };
            }
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDto
                {
                    ApiType = Ubility.SD.ApiType.GET,
                    Url = Ubility.SD.ServierAPI + $"/api/auth/user/{userId}"
                });

                if (response != null && response.IsSuccess && response.Result != null)
                {
                    return JsonSerializer.Deserialize<UserDto>(
                        response.Result.ToString()!,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ResponseDto> CheckUsernameAsync(string username)
        {
            try
            {
                var response = await _baseService.SendAsync(new RequestDto
                {
                    ApiType = Ubility.SD.ApiType.GET,
                    Url = Ubility.SD.ServierAPI + $"/api/auth/check-username/{username}"
                });

                return response ?? new ResponseDto { IsSuccess = false, Message = "Không có phản hồi từ server" };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "Lỗi khi kiểm tra username: " + ex.Message
                };
            }
        }

        public async Task LogoutAsync()
        {
            // Xóa thông tin user và token
            _currentUser = null;
            _token = null;

            // Xóa khỏi localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");

            await _jsRuntime.InvokeVoidAsync("console.log", "[Blazor] Logout: đã xóa phiên đăng nhập");
            
            // Notify subscribers that auth state changed
            OnAuthStateChanged?.Invoke();
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            // Nếu đã có token trong memory thì return ngay
            if (!string.IsNullOrEmpty(_token) && _currentUser != null)
            {
                await _jsRuntime.InvokeVoidAsync("console.log", $"[Blazor] Đã xác thực từ memory: user={_currentUser?.Username}, token={_token}");
                return true;
            }

            // Nếu chưa có, thử load từ localStorage
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
                {
                    _token = token;
                    _currentUser = JsonSerializer.Deserialize<UserDto>(userJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    await _jsRuntime.InvokeVoidAsync("console.log", $"[Blazor] Đã xác thực từ localStorage: user={_currentUser?.Username}, token={_token}");
                    return true;
                }
            }
            catch
            {
                // Nếu có lỗi, clear authentication state
                _token = null;
                _currentUser = null;
            }

            return false;
        }

        // Method để load authentication state từ localStorage khi app start
        public async Task InitializeAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userJson))
                {
                    _token = token;
                    _currentUser = JsonSerializer.Deserialize<UserDto>(userJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch
            {
                // Nếu có lỗi, clear authentication state
                await LogoutAsync();
            }
        }
    }
}
