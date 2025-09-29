using AutoMapper;
using Microsoft.AspNetCore.Http;
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

        public async Task<ApiResponse<IEnumerable<CategoryDTO>>> GetAllAsync()
        {
            try
            {
                var allCategories = await _categoryRepository.GetAllAsync();
                var dtoList = allCategories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToList();

                return new ApiResponse<IEnumerable<CategoryDTO>>(
                    StatusCodes.Status200OK,
                    "Categories fetched successfully",
                    dtoList
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CategoryDTO>>(
                    StatusCodes.Status500InternalServerError,
                    $"Error fetching categories: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<CategoryDTO?>> GetByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return new ApiResponse<CategoryDTO?>(
                        StatusCodes.Status404NotFound,
                        "Category not found"
                    );
                }

                var dto = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name
                };

                return new ApiResponse<CategoryDTO?>(
                    StatusCodes.Status200OK,
                    "Category fetched successfully",
                    dto
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryDTO?>(
                    StatusCodes.Status500InternalServerError,
                    $"Error fetching category: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<CategoryDTO>> AddAsync(CategoryDTO categoryDTO)
        {
            try
            {
                var newCategory = new Category
                {
                    Name = categoryDTO.Name
                };

                await _categoryRepository.AddAsync(newCategory);
                categoryDTO.Id = newCategory.Id;

                return new ApiResponse<CategoryDTO>(
                    StatusCodes.Status201Created,
                    "Category added successfully",
                    categoryDTO
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryDTO>(
                    StatusCodes.Status500InternalServerError,
                    $"Error adding category: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<CategoryDTO>> UpdateAsync(int id, CategoryDTO dto)
        {
            try
            {
                var updateCategory = await _categoryRepository.GetByIdAsync(id);
                if (updateCategory == null)
                {
                    return new ApiResponse<CategoryDTO>(
                        StatusCodes.Status404NotFound,
                        "Category not found"
                    );
                }

                updateCategory.Name = dto.Name;
                await _categoryRepository.UpdateAsync(updateCategory);

                var updatedDto = new CategoryDTO
                {
                    Id = updateCategory.Id,
                    Name = updateCategory.Name
                };

                return new ApiResponse<CategoryDTO>(
                    StatusCodes.Status200OK,
                    "Category updated successfully",
                    updatedDto
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryDTO>(
                    StatusCodes.Status500InternalServerError,
                    $"Error updating category: {ex.Message}"
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return new ApiResponse<bool>(
                        StatusCodes.Status404NotFound,
                        "Category not found",
                        false
                    );
                }

                await _categoryRepository.DeleteAsync(id);

                return new ApiResponse<bool>(
                    StatusCodes.Status200OK,
                    "Category deleted successfully",
                    true
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(
                    StatusCodes.Status500InternalServerError,
                    $"Error deleting category: {ex.Message}",
                    false
                );
            }
        }
    }
}
