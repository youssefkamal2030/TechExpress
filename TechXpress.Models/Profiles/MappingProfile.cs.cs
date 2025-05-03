using AutoMapper;
using TechXpress.Models.entitis; 
using TechXpress.Models.Dto_s;
using System;


namespace TechXpress.Models.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            
            CreateMap<ShoppingCart, ShoppingCartDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<ShoppingCartDTO, ShoppingCart>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<CartItem, CartItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.PriceAtAdd, opt => opt.MapFrom(src => src.PriceAtAdd));
            CreateMap<CartItemDTO, CartItem>()
                .ForMember(dest => dest.Product, opt => opt.Ignore());

            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapStringToOrderStatus(src.Status)));
            CreateMap<OrderDTO, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapOrderStatusToString(src.Status)));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
            CreateMap<OrderItemDTO, OrderItem>();

            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<ProductDTO, Product>();

            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
        }

        private OrderStatus MapStringToOrderStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return OrderStatus.Pending;

            if (status.Equals("paid", StringComparison.OrdinalIgnoreCase))
                return OrderStatus.Processing;
            else
                return OrderStatus.Pending;
        }

        private string MapOrderStatusToString(OrderStatus status)
        {
            if (status == OrderStatus.Processing || status == OrderStatus.Shipped || 
                status == OrderStatus.Delivered)
                return "paid";
            else
                return "not paid";
        }
    }
}