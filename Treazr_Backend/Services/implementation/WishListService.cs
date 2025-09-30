using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Common;
using Treazr_Backend.Data;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class WishListService : IWishlistService
    {
        private readonly AppDbContext _Context;

        public WishListService(AppDbContext context)
        {
            _Context = context;
        }

        public async Task<ApiResponse<object>> GetWishlistAsync(int userId)
        {
            var items = await _Context.Wishlist
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Include(x => x.Product)
                .ToListAsync();

            var result = items.Select(i => new
            {
                i.ProductId,
                i.Product.Name,
                i.Product.Price,
                i.Product.Brand,
                Images = i.Product.Images
                    .Where(img => img.IsMain)
                    .Select(img => new { img.ImageData, img.ImageMimeType })
                    .FirstOrDefault()
            });

            return new ApiResponse<object>(200, "Wishlist fetched successfully", result);
        }


        public async Task<ApiResponse<string>> ToggleWishlistasync(int userId, int productId)
        {
            var existing = await _Context.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId && !w.IsDeleted);

            if (existing != null)
            {
                _Context.Wishlist.Remove(existing);
                await _Context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Product removed from wishlist");
            }

            var user = await _Context.Users.FindAsync(userId);
            if (user == null)
            {
                return new ApiResponse<string>(404, "User not found");
            }

            var product = await _Context.Products.FindAsync(productId);
            if (product == null || !product.IsActive)
            {
                return new ApiResponse<string>(404, "Product not found or inactive");
            }

            var wishlist = new Wishlist
            {
                UserId = userId,
                ProductId = productId
            };

            _Context.Wishlist.Add(wishlist);

            try
            {
                await _Context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return new ApiResponse<string>(500, $"Error saving wishlist: {ex.InnerException?.Message}");
            }

            return new ApiResponse<string>(200, "Product added to wishlist");
        }

    }
}
