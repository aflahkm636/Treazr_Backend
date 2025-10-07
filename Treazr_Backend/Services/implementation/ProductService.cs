using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Common;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs.ProductDTO;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepo, AppDbContext context, IMapper mapper)
        {
            _productRepo = productRepo;
            _context = context;
            _mapper = mapper;
        }



        public async Task<ApiResponse<IEnumerable<ProductDTO>>> GetProductsByCategoryAsync(int categoryId)
        {
            try
            {
                var categoryExists = await _context.categories.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                {
                    return new ApiResponse<IEnumerable<ProductDTO>>(404, $"Category not found with id: {categoryId}");
                }

                var products = await _productRepo.GetProductsByCategoryAsync(categoryId);

                if (products == null || !products.Any())
                {
                    return new ApiResponse<IEnumerable<ProductDTO>>(200, "No products found for this category", new List<ProductDTO>());
                }

                var result = _mapper.Map<IEnumerable<ProductDTO>>(products);

                return new ApiResponse<IEnumerable<ProductDTO>>(200, "Products retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<ProductDTO>>(500, $"Error fetching products: {ex.Message}");
            }
        }


        public async Task<ApiResponse<ProductDTO?>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepo.GetProductWithDetailsAsync(id);

                if (product == null)
                    return new ApiResponse<ProductDTO?>(404, $"Product not found with id: {id}");

                var result = _mapper.Map<ProductDTO>(product);

                return new ApiResponse<ProductDTO?>(200, "Product Retrived Successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductDTO?>(500, $"Error fetching product: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductDTO>> AddProductAsync(AddProductDTO dto)
        {
            // Map basic fields
            var product = _mapper.Map<Product>(dto);

            foreach (var file in dto.Images)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                product.Images.Add(new ProductImage
                {
                    ImageData = ms.ToArray(),
                    ImageMimeType = file.ContentType
                });
            }

            _context.Products.Add(product);
            var isAdded = await _context.SaveChangesAsync() > 0;

            return isAdded ? new ApiResponse<ProductDTO>(200, "Product Added Successfully") :
                new ApiResponse<ProductDTO>(500, "Failed to add product");
        }

        public async Task<ApiResponse<ProductDTO>> UpdateProductASync(UpdateProductDTO dto)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .SingleOrDefaultAsync(p => p.Id == dto.Id);

            if (product == null)
                return new ApiResponse<ProductDTO>(404, "Product not Found");

            if (!string.IsNullOrWhiteSpace(dto.Name)) product.Name = dto.Name.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Description)) product.Description = dto.Description.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Brand)) product.Brand = dto.Brand.Trim();
            if (dto.Price.HasValue) product.Price = dto.Price.Value;
            if (dto.CategoryId.HasValue) product.CategoryId = dto.CategoryId.Value;
            if (dto.CurrentStock.HasValue)
            {
                product.CurrentStock = dto.CurrentStock.Value;
                product.InStock = dto.CurrentStock.Value > 0;
            }

            if (dto.IsActive.HasValue)
                product.IsActive = dto.IsActive.Value;

            //_mapper.Map(dto,product);


            if (dto.NewImages != null && dto.NewImages.Any())
            {
                foreach (var file in dto.NewImages)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    product.Images.Add(new ProductImage
                    {
                        ImageData = ms.ToArray(),
                        ImageMimeType = file.ContentType
                    });
                }
            }

            await _context.SaveChangesAsync();
            return new ApiResponse<ProductDTO>(200, "Product Updated Successfully");
        }

        public async Task<ApiResponse<IEnumerable<ProductDTO>>> GetAllProductsAsync()
        {
            var products=await _context.Products
                .Include(p=>p.Category)
                .Include(p=>p.Images)
                .ToListAsync();

           var productDto= _mapper.Map<IEnumerable<ProductDTO>>(products);
            return new ApiResponse<IEnumerable<ProductDTO>>(200, "products fetched succesfully", productDto);
        }
        public async Task<ApiResponse<string>> ToggleProductStatus(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return new ApiResponse<string>(404, "Product not found");
            }
            product.IsActive = !product.IsActive;

            product.IsDeleted = !product.IsDeleted;
            await _context.SaveChangesAsync();

            if (product.IsActive == true && product.IsDeleted == false)
            {
                return new ApiResponse<string>(200, "Product Activated Successfully");
            }
            else
            {
                return new ApiResponse<string>(200, "Product Deactivated Successfully");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductDTO>>> GetFilteredProducts(string? name)
        {
            var query =  _context.Products
                .Include(q => q.Category)
                .Include(q => q.Images)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(q => q.Name.Contains(name) ||  q.Category.Name.Contains(name)|| q.Brand.Contains(name));

            //if (!string.IsNullOrWhiteSpace(category))
            //    query = query.Where(q => q.Category.Name.Contains(category)||  q.Category.Name.Contains(category) );

            var products=await query.ToListAsync();

            var productDto= _mapper.Map<IEnumerable<ProductDTO>>(products);

            return new ApiResponse<IEnumerable<ProductDTO>>(200,"filtered products successfully" ,productDto);

        }
        }
}
