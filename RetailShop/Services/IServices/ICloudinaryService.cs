namespace RetailShop.Services.IServices;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<bool> DeleteImageAsync(string publicId);

}
