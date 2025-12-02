using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;
using RetailShop.Blazor.Ubility;
using Newtonsoft.Json;
using Microsoft.JSInterop;

namespace RetailShop.Blazor.Services
{
    public class CustomerAuthService : ICustomerAuthService
    {
        private readonly IBaseService _baseService;
        private readonly IJSRuntime _jsRuntime;
        private readonly CustomerStateService _customerStateService;

        public CustomerAuthService(IBaseService baseService, IJSRuntime jsRuntime, CustomerStateService customerStateService)
        {
            _baseService = baseService;
            _jsRuntime = jsRuntime;
            _customerStateService = customerStateService;
        }

        public async Task<ResponseDto> RegisterAsync(CustomerRegisterDto registerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerDto,
                Url = SD.ServierAPI + "/api/CustomerAuth/register"
            });
        }

        public async Task<ResponseDto> LoginAsync(CustomerLoginDto loginDto)
        {
            var response = await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginDto,
                Url = SD.ServierAPI + "/api/CustomerAuth/login"
            });

            if (response.IsSuccess && response.Result != null)
            {
                var loginResponse = JsonConvert.DeserializeObject<CustomerLoginResponseDto>(Convert.ToString(response.Result));
                if (loginResponse != null)
                {
                    try
                    {
                        var customer = new CustomerDto
                        {
                            CustomerId = loginResponse.CustomerId,
                            Name = loginResponse.Name,
                            Email = loginResponse.Email,
                            Phone = loginResponse.Phone,
                            Address = loginResponse.Address
                        };

                        // Lưu token và thông tin customer vào LocalStorage
                        await _jsRuntime.InvokeVoidAsync("authInterop.setCustomerToken", loginResponse.Token);
                        await _jsRuntime.InvokeVoidAsync("authInterop.setCustomerInfo", customer);
                        
                        // Cập nhật state service để UI có thể phản ứng ngay lập tức
                        _customerStateService.SetCustomer(customer);
                    }
                    catch (Exception ex)
                    {
                        // Nếu JS không sẵn sàng, trả về lỗi
                        return new ResponseDto
                        {
                            IsSuccess = false,
                            Message = "Có lỗi xảy ra: " + ex.Message
                        };
                    }
                }
            }

            return response;
        }

        public async Task<CustomerDto?> GetCustomerProfileAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<CustomerDto?>("authInterop.getCustomerInfo");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string?>("authInterop.getCustomerToken");
                return !string.IsNullOrEmpty(token);
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("authInterop.clearCustomerAuth");
            }
            catch
            {
                // Ignore errors during logout
            }
            
            // Cập nhật state service
            _customerStateService.ClearCustomer();
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>("authInterop.getCustomerToken");
            }
            catch
            {
                return null;
            }
        }
    }
}
