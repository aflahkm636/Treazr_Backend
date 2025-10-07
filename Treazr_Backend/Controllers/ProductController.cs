using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.ProductDTO;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductController:ControllerBase
    {

        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;

        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
          return StatusCode(product.StatusCode, product);

        }

        [HttpGet("category/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return StatusCode(products.StatusCode, products);
        }

        [HttpPost]
        [Authorize(Policy ="Admin")]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDTO addProductDTO)
        {
            var response= await _productService.AddProductAsync(addProductDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut]
        [Authorize(Policy ="Admin")]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDTO updateProductDTO)
        {
            var response=await _productService.UpdateProductASync(updateProductDTO);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch]
        [Authorize(Policy ="Admin")]
        public async Task<IActionResult> ToggleDelete([Range(1,int.MaxValue)] int id)
        {
            var response=await _productService.ToggleProductStatus(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return StatusCode(products.StatusCode, products);
        }

        [HttpGet("filter")]
        [AllowAnonymous]
        public async Task<IActionResult>GetfiteredProducts([FromQuery] string? filter)
        {
            var result=await _productService.GetFilteredProducts(filter);
            return StatusCode(result.StatusCode, result);
        }

    }
}
