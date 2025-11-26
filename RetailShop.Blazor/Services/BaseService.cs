using Newtonsoft.Json;
using RetailShop.Blazor.Dtos;
using RetailShop.Blazor.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static RetailShop.Blazor.Ubility.SD;

namespace RetailShop.Blazor.Services;
public class BaseService : IBaseService
{
    private readonly IHttpClientFactory _httpClientFactory;
   
    public BaseService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

    }
    public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("RetailShop-API");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            
           
            message.RequestUri = new Uri(requestDto.Url);
            if (requestDto.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage? apiResponse = null;
            switch (requestDto.ApiType)
            {
                case ApiType.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await client.SendAsync(message);

            switch (apiResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new() { IsSuccess = false, Message = "Not Found" };
                case HttpStatusCode.Forbidden:
                    return new() { IsSuccess = false, Message = "Access Denied" };
                case HttpStatusCode.Unauthorized:
                    return new() { IsSuccess = false, Message = "Unauthorized" };
                case HttpStatusCode.InternalServerError:
                    return new() { IsSuccess = false, Message = "Internal Server Error" };
                default:
                    var apiContent = await apiResponse.Content.ReadAsStringAsync();
                    var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                    return apiResponseDto;
            }
        }
        catch (Exception ex)
        {
            var dto = new ResponseDto
            {
                Message = ex.Message.ToString(),
                IsSuccess = false,
            };
            return dto;
        }
    }
}
