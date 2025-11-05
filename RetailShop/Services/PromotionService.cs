using Microsoft.EntityFrameworkCore;
using RetailShop.Data;
using RetailShop.Dtos;
using RetailShop.Models;
using RetailShop.Services.IServices;
using System;
using System.Text.Json;

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
            var existingPromotion = await _db.Promotions
                .FirstOrDefaultAsync(p => p.PromoCode == promotion.PromoCode);
            if (existingPromotion != null)
            {
                rs.IsSuccess = false;
                rs.Message = "Promotion Code already exists.";
                return rs;
            }
            var isValidResult = await isValid(promotion);
            if (!isValidResult.IsSuccess)
            {
                return new ResultService<Promotion>
                {
                    IsSuccess = false,
                    Message = isValidResult.Message
                };
            }
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
    public async Task<ResultService<List<Promotion>>> GetAllPromotionsAsync(bool active = true)
    {
        var rs = new ResultService<List<Promotion>>();
        try
        {
            String status = active ? "active" : "inactive";
            var promotions = await _db.Promotions.Where(p => p.Status == status).OrderByDescending(p => p.PromoId).ToListAsync();
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
    public async Task<ResultService<Boolean>> isValid(Promotion promotion)
    {
        var rs = new ResultService<Boolean>();
        try
        {
            // ======= VALIDATION SECTION =======
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (promotion.StartDate < today)
            {
                rs.IsSuccess = false;
                rs.Message = "Start date cannot be before today.";
                rs.Data = false;
                return rs;
            }
            if (promotion.EndDate < promotion.StartDate)
            {
                rs.IsSuccess = false;
                rs.Message = "End date must be after start date.";
                rs.Data = false;
                return rs;
            }
            if (promotion.DiscountValue <= 0)
            {
                rs.IsSuccess = false;
                rs.Message = "Discount value must be greater than 0.";
                rs.Data = false;
                return rs;
            }
            if (promotion.DiscountType?.ToLower() == "percent" && promotion.DiscountValue > 100)
            {
                rs.IsSuccess = false;
                rs.Message = "For percentage discount, value must be between 1 and 100.";
                rs.Data = false;
                return rs;
            }
            if (promotion.DiscountType?.ToLower() == "fixed" && promotion.DiscountValue < 1000)
            {
                rs.IsSuccess = false;
                rs.Message = "For VND discount, value must be greater than or equal to 1000.";
                rs.Data = false;
                return rs;
            }
            rs.IsSuccess = true;
            rs.Data = true;
            rs.Message = "Promotion is valid.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error validating promotion: {ex.Message}";
            rs.Data = false;
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
            var isValidResult = await isValid(promotion);
            if (!isValidResult.IsSuccess)
            {
                return new ResultService<Promotion>
                {
                    IsSuccess = false,
                    Message = isValidResult.Message
                };
            }

            // ======= UPDATE SECTION =======
            existingPromotion.PromoCode = promotion.PromoCode;
            existingPromotion.Description = promotion.Description;
            existingPromotion.DiscountType = promotion.DiscountType;
            existingPromotion.DiscountValue = promotion.DiscountValue;
            existingPromotion.StartDate = promotion.StartDate;
            existingPromotion.EndDate = promotion.EndDate;
            existingPromotion.MinOrderAmount = promotion.MinOrderAmount;
            existingPromotion.UsageLimit = promotion.UsageLimit;
            existingPromotion.Status = promotion.Status;

            _db.Promotions.Update(existingPromotion);
            await _db.SaveChangesAsync();

            rs.IsSuccess = true;
            rs.Data = existingPromotion;
            rs.Message = "Promotion updated successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error updating promotion: {ex.Message}";
        }

        return rs;
    }


    public async Task<ResultService<Promotion>> DeletePromotionAsync(int id)
    {
        var rs = new ResultService<Promotion>();
        try
        {
            
            var promotion = await _db.Promotions.FindAsync(id);
            if (promotion == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Promotion not found.";
                return rs;
            }

            var usedCount = promotion.UsedCount ?? 0;
            if (usedCount > 0 || promotion.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
            {
                // Soft-delete 
                promotion.Status = "inactive";
                _db.Promotions.Update(promotion);
                await _db.SaveChangesAsync();
                rs.IsSuccess = true;
                rs.Data = promotion;
                rs.Message = "Promotion was set to INACTIVE because it has usage or already started.";
                return rs;
            }

            // Hard delete when safe
            _db.Promotions.Remove(promotion);
            await _db.SaveChangesAsync();
            rs.IsSuccess = true;
            rs.Message = "Promotion deleted successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error deleting promotion: {ex.Message}";
        }
        return rs;
    }
    
    public async Task<ResultService<Promotion>> RestoreSupplierAsync(int id)
    {
        var rs = new ResultService<Promotion>();
        try
        {
            var existingPromotion = await _db.Promotions.FindAsync(id);
            if (existingPromotion == null)
            {
                rs.IsSuccess = false;
                rs.Message = "Promotion not found.";
                return rs;
            }
            existingPromotion.Status = "active";

            _db.Promotions.Update(existingPromotion);
            await _db.SaveChangesAsync();

            rs.IsSuccess = true;
            rs.Data = existingPromotion;
            rs.Message = "Promotion updated successfully.";
        }
        catch (Exception ex)
        {
            rs.IsSuccess = false;
            rs.Message = $"Error updating promotion: {ex.Message}";
        }

        return rs;
    }

}

