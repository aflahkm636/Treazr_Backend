using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.CategoryDTO;
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

            var response = await _categoryService.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return StatusCode(category.StatusCode, category);

        }
        [HttpPost]
        public async Task<IActionResult> Add(CategoryDTO dto)
        {
            var result = await _categoryService.AddAsync(dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryDTO dto)
        {
            var result = await _categoryService.UpdateAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            return StatusCode(success.StatusCode, success);
        }
    }
}
