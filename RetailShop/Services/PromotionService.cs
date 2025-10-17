using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;

namespace RetailShop.Services;

public class PromotionService : IPromotionService
{
    private readonly AppDbContext _db;
    public PromotionService(AppDbContext db)
    {
        _db = db;
    }
    public async Task<ResultService<Promotion>> GetPromotionByIdAsync(int id)
    {
        var rs = new ResultService<Promotion>();
        try
        {
            var promotion = await _db.Promotions.FindAsync(id);
            if (promotion == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Promotion not found.";
            }
            else
            {
                rs.IsSuccess = true;
                rs.Data = promotion;
                rs.Message = "Promotion retrieved successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving promotion: {ex.Message}";
        }
        return rs;
    }
    public async Task<ResultService<Promotion>> CreatePromotionAsync(Promotion promotion)
    {
        var rs = new ResultService<Promotion>();
        try
        {
            await _db.Promotions.AddAsync(promotion);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Data = promotion;
            rs.Message = "Promotion created successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error creating promotion: {ex.Message}";
        }
        return rs;
    }
    public async Task<ResultService<List<Promotion>>> GetAllPromotionsAsync()
    {
        var rs = new ResultService<List<Promotion>>();
        try
        {
            var promotions = await _db.Promotions.ToListAsync();
            rs.IsSuccess = true;
            rs.Data = promotions;
            rs.Message = "Promotions retrieved successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error retrieving promotions: {ex.Message}";
        }
        return rs;
    }
    public async Task<ResultService<Promotion>> UpdatePromotionAsync(Promotion promotion)
    {
        var rs = new ResultService<Promotion>();
        try
        {
            var existingPromotion = await _db.Promotions.FindAsync(promotion.PromoId);
            if (existingPromotion == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Promotion not found.";
                return rs;
            }
            else
            {
                existingPromotion.PromoCode = promotion.PromoCode;
                existingPromotion.Description = promotion.Description;
                existingPromotion.DiscountType = promotion.DiscountType;
                existingPromotion.DiscountValue = promotion.DiscountValue;
                existingPromotion.StartDate = promotion.StartDate;
                existingPromotion.EndDate = promotion.EndDate;
                existingPromotion.MinOrderAmount = promotion.MinOrderAmount;
                existingPromotion.UsageLimit = promotion.UsageLimit;
                existingPromotion.UsedCount = promotion.UsedCount;
                existingPromotion.Status = promotion.Status;
                _db.Promotions.Update(existingPromotion);
                _db.SaveChanges();
                rs.IsSuccess = true;
                rs.Data = existingPromotion;
                rs.Message = "Promotion updated successfully.";
            }
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error updating promotion: {ex.Message}";
        }
        return rs;
    }
}

