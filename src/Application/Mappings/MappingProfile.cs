using Application.DTO;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != string.Empty))
            .ForMember(dest => dest.Price, opt => opt.Condition(src => src.Price != 0));
    }
}