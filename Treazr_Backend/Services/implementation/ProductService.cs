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



        public async Task<ApiResponse<object>> GetProductsByCategoryAsync(int categoryId, int? pageNumber = null, int? pageSize = null)
        {
            try
            {
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                {
                    return new ApiResponse<object>(404, $"Category not found with id: {categoryId}");
                }

                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .Where(p => p.CategoryId == categoryId)
                    .OrderBy(p => p.Name)
                    .AsQueryable();

                if (pageNumber == null || pageSize == null)
                {
                    var allProducts = await query.ToListAsync();
                    var allProductDtos = _mapper.Map<IEnumerable<ProductDTO>>(allProducts);

                    return new ApiResponse<object>(
                        200,
                        "All products fetched successfully for this category",
                        new
                        {
                            CategoryId = categoryId,
                            TotalCount = allProductDtos.Count(),
                            Products = allProductDtos
                        }
                    );
                }

                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                var totalCount = await query.CountAsync();

                var pagedProducts = await query
                    .Skip(((int)pageNumber - 1) * (int)pageSize)
                    .Take((int)pageSize)
                    .ToListAsync();

                var pagedProductDtos = _mapper.Map<IEnumerable<ProductDTO>>(pagedProducts);

                var pagedResult = new
                {
                    CategoryId = categoryId,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                    Products = pagedProductDtos
                };

                return new ApiResponse<object>(200, "Products fetched successfully for this category", pagedResult);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, $"Error fetching products: {ex.Message}");
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


            product.Images = product.Images
         .Where(img => dto.ExistingImageIds.Contains(img.Id))
         .ToList();

            // Add new images
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

        public async Task<ApiResponse<object>> GetProductsAsync(string? search = null, int? pageNumber = null, int? pageSize = null)
        {
            // Base query
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .AsQueryable();

            // ✅ Apply optional filtering
            if (!string.IsNullOrWhiteSpace(search))
            {
                string lowerSearch = search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(lowerSearch) ||
                    p.Brand.ToLower().Contains(lowerSearch) ||
                    p.Category.Name.ToLower().Contains(lowerSearch));
            }

            // ✅ If no pagination provided → return all filtered products
            if (pageNumber == null || pageSize == null)
            {
                var allProducts = await query.ToListAsync();
                var allProductDtos = _mapper.Map<IEnumerable<ProductDTO>>(allProducts);

                return new ApiResponse<object>(
                    200,
                    string.IsNullOrWhiteSpace(search)
                        ? "All products fetched successfully"
                        : "Filtered products fetched successfully",
                    new
                    {
                        TotalCount = allProductDtos.Count(),
                        Products = allProductDtos
                    }
                );
            }

            // ✅ Ensure pagination defaults
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var totalCount = await query.CountAsync();

            var pagedProducts = await query
                .OrderBy(p => p.Name)
                .Skip(((int)pageNumber - 1) * (int)pageSize)
                .Take((int)pageSize)
                .ToListAsync();

            var productDtos = _mapper.Map<IEnumerable<ProductDTO>>(pagedProducts);

            var pagedResult = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Products = productDtos
            };

            return new ApiResponse<object>(
                200,
                string.IsNullOrWhiteSpace(search)
                    ? "Products fetched successfully"
                    : "Filtered products fetched successfully",
                pagedResult
            );
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
        public async Task<ApiResponse<object>> GetNewestProductsAsync(int? count = null)
        {
            try
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Images)
                    .Where(p => !p.IsDeleted && p.IsActive)
                    .OrderByDescending(p => p.CreatedOn)
                    .AsQueryable();

                if (count.HasValue && count > 0)
                {
                    query = query.Take(count.Value);
                }

                var products = await query.ToListAsync();

                var productDtos = products.Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Brand = p.Brand,
                    Price = p.Price,
                    CurrentStock = p.CurrentStock,
                    InStock = p.InStock,
                    CreatedOn = p.CreatedOn,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category?.Name ?? string.Empty,
                    ImageBase64 = p.Images
                        .Select(i => $"data:{i.ImageMimeType};base64,{Convert.ToBase64String(i.ImageData)}")
                        .ToList()
                }).ToList();

                return new ApiResponse<object>(200, "Newest products fetched successfully", productDtos
               );
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, $"Error fetching newest products: {ex.Message}");
            }
        }


        public async Task<ApiResponse<IEnumerable<ProductDTO>>> GetFilteredProducts(string? name)
        {
            var query = _context.Products
                .Include(q => q.Category)
                .Include(q => q.Images)
                        .Where(q => q.IsActive)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(q => q.Name.Contains(name) || q.Category.Name.Contains(name) || q.Brand.Contains(name));

            //if (!string.IsNullOrWhiteSpace(category))
            //    query = query.Where(q => q.Category.Name.Contains(category)||  q.Category.Name.Contains(category) );

            var products = await query.ToListAsync();

            var productDto = _mapper.Map<IEnumerable<ProductDTO>>(products);

            return new ApiResponse<IEnumerable<ProductDTO>>(200, "filtered products successfully", productDto);

        }
    }
}
