using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Treazr_Backend.Common;
using Treazr_Backend.Data;
using Treazr_Backend.Models;
using Treazr_Backend.Repository.interfaces;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;
        private readonly IProductRepository _productRepository;

        public CartService(AppDbContext context, IProductRepository productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<ApiResponse<object>> GetCartForUserAsync(int userId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

                if (cart == null || !cart.Items.Any())
                {
                    return new ApiResponse<object>(200, "Cart is empty", new { Items = Array.Empty<object>() });
                }

                var cartResponse = new
                {
                    TotalQuantity = cart.Items.Sum(i => i.Quantity),
                    TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity),
                    Items = cart.Items.Select(i => new
                    {
                        i.Id,
                        i.ProductId,
                        i.Name,
                        i.Price,
                        i.Quantity,
                        i.ImageData,
                        i.ImageMimeType
                    })
                };

                return new ApiResponse<object>(200, "Cart fetched successfully", cartResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>(500, $"An error occurred while fetching the cart: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> AddToCartAsync(int userId, int ProductId, int quantity)
        {
            try
            {
                var product = await _productRepository.GetProductWithDetailsAsync(ProductId);
                if (product == null) return new ApiResponse<string>(404, "Product not found");
                if (!product.IsActive) return new ApiResponse<string>(400, "Product is deactivated");
                if (!product.InStock) return new ApiResponse<string>(400, "Product is out of stock");
                if (quantity < 1 || quantity > 5) return new ApiResponse<string>(400, "Quantity must be between 1 and 5");

                var cart = await _context.Carts.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted)
                    ?? new Cart { UserId = userId, Items = new List<CartItem>() };

                if (!_context.Carts.Local.Contains(cart)) _context.Carts.Add(cart);

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == ProductId);
                if (existingItem != null)
                {
                    if (existingItem.Quantity + quantity > 5)
                        return new ApiResponse<string>(400, "Quantity cannot exceed 5 per item");
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var mainImage = product.Images.FirstOrDefault(i => i.IsMain);
                    cart.Items.Add(new CartItem
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Quantity = quantity,
                        ImageData = mainImage?.ImageData,
                        ImageMimeType = mainImage?.ImageMimeType
                    });
                }

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Product added to cart successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(500, $"An error occurred while adding to cart: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> UpdateCartItemAsync(int userId, int cartItemId, int quantity)
        {
            try
            {
                if (quantity < 1 || quantity > 5)
                    return new ApiResponse<string>(400, "Quantity must be between 1 and 5");

                var cartItem = await _context.CartItems.Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId);

                if (cartItem == null) return new ApiResponse<string>(404, "Cart item not found");

                cartItem.Quantity = quantity;
                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Cart item quantity updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(500, $"An error occurred while updating the cart item: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> RemoveCartItemasync(int userId, int CartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems.Include(ci => ci.Cart)
                    .FirstOrDefaultAsync(ci => ci.Id == CartItemId && ci.Cart.UserId == userId);

                if (cartItem == null) return new ApiResponse<string>(404, "Cart item not found");

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Cart item removed successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(500, $"An error occurred while removing the cart item: {ex.Message}");
            }
        }

        public async Task<ApiResponse<string>> ClearCartAsync(int userId)
        {
            try
            {
                var cart = await _context.Carts.Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsDeleted);

                if (cart != null) _context.CartItems.RemoveRange(cart.Items);

                await _context.SaveChangesAsync();
                return new ApiResponse<string>(200, "Cart cleared successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(500, $"An error occurred while clearing the cart: {ex.Message}");
            }
        }
    }
}
