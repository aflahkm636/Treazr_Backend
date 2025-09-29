using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Treazr_Backend.Common;
using Treazr_Backend.DTOs.ProductDTO;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
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

        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return StatusCode(products.StatusCode, products);
        }


        //[HttpGet("all")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetAllProducts()
        //{
        //    var products = await _productService.GetAllProductsAsync();
        //    return StatusCode(products.StatusCode, products);
        //}

    }
}
