using AutoMapper;
using Treazr_Backend.DTOs.CategoryDTO;
using Treazr_Backend.DTOs.ProductDTO;
using Treazr_Backend.Models;

namespace Treazr_Backend.Profiles
{
    public class MappingProfile :Profile
    {
        public MappingProfile() 
        {
            CreateMap<Category, CategoryDTO>();

            CreateMap<Product, ProductDTO>()
         .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
         .ForMember(dest => dest.ImageBase64, opt => opt.MapFrom(src => src.Images
             .Select(i => $"data:{i.ImageMimeType};base64,{Convert.ToBase64String(i.ImageData)}").ToList()
         ));

            CreateMap<AddProductDTO, Product>()
            .ForMember(dest => dest.Images, opt => opt.Ignore()) 
            .ForMember(dest => dest.InStock, opt => opt.MapFrom(src => src.CurrentStock > 0))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));


            // Map only when the source value is not null
            CreateMap<UpdateProductDTO, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        }
    }
}
