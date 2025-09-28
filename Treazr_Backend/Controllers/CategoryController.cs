using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Treazr_Backend.DTOs.CategoryDTO;
using Treazr_Backend.Common;

using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;


        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {

            var Entities = await _categoryService.GetAllAsync();
            var categoryDTOs = _mapper.Map<List<CategoryDTO>>(Entities);
            return Ok(new ApiResponse<List<CategoryDTO>>(200, "Categories fetched successfully", categoryDTOs));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse<CategoryDTO>(404, "Category Not Found"));
            return Ok(new ApiResponse<CategoryDTO>(200, "Category fetched successfully", category));
        }
        [HttpPost]
        public async Task<IActionResult> Add(CategoryDTO dto)
        {
            var result = await _categoryService.AddAsync(dto);
            return Ok(new ApiResponse<CategoryDTO>(201, "Category created", result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryDTO dto)
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            if (result == null)
                return NotFound(new ApiResponse<string>(404, "Category not found"));

            return Ok(new ApiResponse<CategoryDTO>(200, "Category updated", result));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success)
                return NotFound(new ApiResponse<string>(404, "Category not found"));

            return Ok(new ApiResponse<string>(200, "Category deleted"));
        }
    }
}
