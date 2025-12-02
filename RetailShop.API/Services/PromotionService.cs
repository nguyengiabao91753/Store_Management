using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RetailShop.API.Data;
using RetailShop.API.Dtos;
using RetailShop.API.Services.IServices;

namespace RetailShop.API.Services;

public class PromotionService : IPromotionService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public PromotionService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    public async Task<ResponseDto> GetPromotionByCode(string code)
    {
       var rs = new ResponseDto();
         var promotion = await _db.Promotions.FirstOrDefaultAsync(p => p.PromoCode == code);
        if (promotion != null && promotion.Status == "active")
        {
            if(promotion.UsageLimit <= 0)
            {
                rs.IsSuccess = false;
                rs.Message = "This Promotion reach usage Limit!";
                return rs;
            }
            rs.Result = _mapper.Map<PromotionDTO>(promotion);
            rs.IsSuccess = true;
        }
        else
        {
            rs.IsSuccess = false;
            rs.Message = "Promotion code not found.";
        }

        return rs;
    }

    public async Task<ResponseDto> UpdatePromotionCount(int promoId)
    {
        var rs = new ResponseDto();
        var promotion = await _db.Promotions.FirstOrDefaultAsync(p => p.PromoId == promoId);
        if (promotion != null)
        {
            promotion.UsedCount += 1 ;
            promotion.UsageLimit -= 1;
            _db.Promotions.Update(promotion);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
        }
        else
        {
            rs.IsSuccess = false;
            rs.Message = "Promotion not found.";
        }
        return rs;
    }
}
