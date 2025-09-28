using AutoMapper;
using Treazr_Backend.DTOs.CategoryDTO;
using Treazr_Backend.Models;

namespace Treazr_Backend.Profiles
{
    public class MappingProfile :Profile
    {
        public MappingProfile() 
        {
            CreateMap<Category, CategoryDTO>();

        
        }
    }
}
