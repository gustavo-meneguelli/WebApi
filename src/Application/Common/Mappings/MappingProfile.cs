using Application.Features.Categories.DTOs;
using Application.Features.Products.DTOs;
using Application.Features.ProductReviews.DTOs;
using Application.Features.Carts.DTOs;
using Application.Features.Orders.DTOs;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // === Product Mappings ===
        CreateMap<CreateProductDto, Product>().ReverseMap();

        // Suporta update parcial: só mapeia campos diferentes dos valores default
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != string.Empty))
            .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != string.Empty))
            .ForMember(dest => dest.ImageUrl, opt => opt.Condition(src => src.ImageUrl != string.Empty))
            .ForMember(dest => dest.Price, opt => opt.Condition(src => src.Price != 0))
            .ForMember(dest => dest.CategoryId, opt => opt.Condition(src => src.CategoryId != 0));

        CreateMap<Product, ProductResponseDto>();
        
        // === Category Mappings ===
        CreateMap<CreateCategoryDto, Category>();

        // Suporta update parcial: só mapeia campos diferentes dos valores default
        CreateMap<UpdateCategoryDto, Category>()
            .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != string.Empty));

        CreateMap<Category, CategoryResponseDto>();

        // === ProductReview Mappings ===
        CreateMap<CreateProductReviewDto, ProductReview>();
        CreateMap<ProductReview, ProductReviewResponseDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.UserAvatarChoice, opt => opt.MapFrom(src => src.User.AvatarChoice));

        // === Cart Mappings ===
        CreateMap<CartItem, CartItemResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
            .ForMember(dest => dest.YourPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.CurrentPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.Savings, opt => opt.MapFrom(src => src.UnitSavings));

        CreateMap<Cart, CartResponseDto>()
            .ForMember(dest => dest.TotalSavings, opt => opt.MapFrom(src => src.TotalSavings));

        // === Order Mappings ===
        CreateMap<OrderItem, OrderItemResponseDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Name : ErrorMessages.ProductUnavailable));

        CreateMap<Order, OrderResponseDto>();
    }
}


