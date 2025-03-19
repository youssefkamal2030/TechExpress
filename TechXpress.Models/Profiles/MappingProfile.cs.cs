using AutoMapper;
using TechXpress.Models.entitis;
using TechXpress.Models.Dto_s;

namespace TechXpress.Models.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ShoppingCart <-> ShoppingCartDTO
            CreateMap<ShoppingCart, ShoppingCartDTO>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
            CreateMap<ShoppingCartDTO, ShoppingCart>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            // CartItem <-> CartItemDTO
            CreateMap<CartItem, CartItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<CartItemDTO, CartItem>();

            // Order <-> OrderDTO
            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<OrderDTO, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            // OrderItem <-> OrderItemDTO
            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<OrderItemDTO, OrderItem>();

            // Product <-> ProductDTO
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<ProductDTO, Product>();

            // Category <-> CategoryDTO
            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();

          
        }
    }
}