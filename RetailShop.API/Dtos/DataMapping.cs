using AutoMapper;
using RetailShop.API.Models;

namespace RetailShop.API.Dtos;

public class DataMapping : Profile
{
    public DataMapping()
    {
        CreateMap<Product, ProductDTO>()
            //Map Categpry
            .ForMember(dest => dest.CategoryId, src => src.MapFrom(s => s.Category.CategoryId))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty))
            //Map Supplier
            .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.Supplier != null ? src.SupplierId : null))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null))
            .ForMember(dest => dest.SupplierEmail, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Email : null))
            .ForMember(dest => dest.SupplierPhone, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Phone : null))
            .ForMember(dest => dest.SupplierAddress, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Address : null))

            //Map Inventory
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Inventory != null ? src.Inventory.Quantity : 0));

        CreateMap<ProductDTO, Product>();


        // ==================== ORDER MAPPING ====================

        CreateMap<Order, OrderDTO>()
            // Customer info
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Name : string.Empty))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Phone : null))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Email : null))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.Address : null))

            // User info
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User != null ? src.User.Username : string.Empty))

            // Promotion
            .ForMember(dest => dest.PromoCode, opt => opt.MapFrom(src => src.Promo != null ? src.Promo.PromoCode : null))

            // Các trường trực tiếp
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))

            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Payment.PaymentMethod))

            // OrderItems
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

        // ==================== ORDER ITEM MAPPING ====================

        CreateMap<OrderItem, OrderItemDTO>()
            .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.OrderItemId))
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))

            // Product info
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductImage : null))
            .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Product != null ? src.Product.Barcode : null))
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Product != null ? src.Product.Unit : null))

            // Các trường khác
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal));

        // Nếu bạn cần map ngược (DTO → Entity) thì thêm dòng này
        CreateMap<OrderDTO, Order>();
        CreateMap<OrderItemDTO, OrderItem>();

        // ==================== PROMOTION MAPPING ====================
        CreateMap<Promotion, PromotionDTO>();
        CreateMap<ProductDTO, ProductDTO>();
    }
}
