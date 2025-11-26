using RetailShop.Blazor.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailShop.Blazor.Services.IServices;
public interface IBaseService
{
    Task<ResponseDto?> SendAsync(RequestDto requestDto);
}
