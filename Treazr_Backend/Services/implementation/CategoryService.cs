using AutoMapper;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.CategoryDTO;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var allCategories = await _categoryRepository.GetAllAsync();
            return allCategories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }
        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }
        public async Task<CategoryDTO> AddAsync(CategoryDTO categoryDTO)
        {
            var newCategory = new Category
            {
                Name = categoryDTO.Name
            };
            await _categoryRepository.AddAsync(newCategory);
            categoryDTO.Id = newCategory.Id;
            return categoryDTO;
        }
        public async Task<CategoryDTO> UpdateAsync(int id, CategoryDTO dto)
        {
            var updateCategory = await _categoryRepository.GetByIdAsync(id);
            if (updateCategory == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            updateCategory.Name = dto.Name;
            await _categoryRepository.UpdateAsync(updateCategory);
            return new CategoryDTO
            {
                Id = updateCategory.Id,
                Name = updateCategory.Name
            };
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException("Category not found");
            }
            await _categoryRepository.DeleteAsync(id);
            return true;
        }
    }
}
