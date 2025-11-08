using Microsoft.EntityFrameworkCore;
using RetailShop.Client.Data;
using RetailShop.Client.Models;
using RetailShop.Client.Services.IServices;

namespace RetailShop.Client.Services;

public class PromotionPOSService : IPromotionPOSService
{
    private readonly AppDbContext _db;
    public PromotionPOSService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<List<Promotion>> GetAllActivePromotionsAsync()
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var promotions =await _db.Promotions
                                .Where(p => p.Status.Equals("active") && today >= p.StartDate && today <= p.EndDate)
                                .ToListAsync();
            return promotions;
        }
        catch (Exception)
        {
            return new List<Promotion>();
        }
    }
    public Promotion? GetPromotionByCode(string code)
    {
        var promo = _db.Promotions
           .FirstOrDefault(p => p.PromoCode == code && p.Status == "active"
                                && p.StartDate <= DateOnly.FromDateTime(DateTime.Now)
                                && p.EndDate >= DateOnly.FromDateTime(DateTime.Now)
                                && p.UsageLimit>0);

        return promo;
    }

    public async Task<Promotion?> UpdateUseCount(int promotionId)
    {
        var promo = await _db.Promotions.FindAsync(promotionId);
        if (promo != null && promo.UsageLimit > 0)
        {
            promo.UsageLimit -= 1;
            promo.UsedCount += 1;
            _db.Promotions.Update(promo);
            await _db.SaveChangesAsync();
            return promo;
        }
        return null;
    }
}
