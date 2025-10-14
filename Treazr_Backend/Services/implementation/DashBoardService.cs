using Microsoft.EntityFrameworkCore;
using Treazr_Backend.Common;
using Treazr_Backend.Data;
using Treazr_Backend.DTOs.OrderDto;
using Treazr_Backend.Models;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Services.implementation
{
 

    public class DashBoardService:IDashBoardService
    {
        private readonly AppDbContext _context;

        public DashBoardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<object?>> GetDashboardStatsAsync(string type = "all")
        {
            //var today = DateTime.UtcNow.Date;
            //var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalInventoryValue = await _context.Products.SumAsync(p => p.Price * p.CurrentStock);
            var lowStockCount = await _context.Products.CountAsync(p => p.CurrentStock < 5);

            var deliveredCount = await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .CountAsync();

            var shippedCount = await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Shipped)
                .CountAsync();

            var totalRevenue = await _context.Orders
                .Where(o => o.OrderStatus == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount);


            //TopSellingProductDTO? topSellingToday = await _context.OrderItems
            //    .Include(i=>i.ImageData)
            //    .Where(i => i.Order.OrderStatus != OrderStatus.Pending &&
            //                i.Order.OrderStatus != OrderStatus.Cancelled &&
            //                i.Order.CreatedOn.Date == today)
            //    .GroupBy(i => i.Product.Name)
            //    .Select(g => new TopSellingProductDTO
            //    {
            //        ProductName = g.Key,

            //        OrdersCount = g.Select(i => i.OrderId).Distinct().Count()
            //    })
            //    .OrderByDescending(x => x.OrdersCount)
            //    .FirstOrDefaultAsync();

            //TopSellingProductDTO? topSellingWeek = await _context.OrderItems
            //    .Where(i => i.Order.OrderStatus != OrderStatus.Pending &&
            //                i.Order.OrderStatus != OrderStatus.Cancelled &&
            //                i.Order.CreatedOn.Date >= startOfWeek)
            //    .GroupBy(i => i.Product.Name)
            //    .Select(g => new TopSellingProductDTO
            //    {
            //        ProductName = g.Key,

            //        OrdersCount = g.Select(i => i.OrderId).Distinct().Count()
            //    })
            //    .OrderByDescending(x => x.OrdersCount)
            //    .FirstOrDefaultAsync();


            object data = type.ToLower() switch
            {
                "user" => new { TotalUsers = totalUsers },
                "revenue" => new { TotalRevenue = totalRevenue },
                "products" => new
                {
                    TotalProducts = totalProducts,
                    TotalInventoryValue = totalInventoryValue,
                    LowStockCount = lowStockCount
                },
                "orders" => new
                {
                    DeliveredOrdersCount = deliveredCount,
                    ShippedOrdersCount = shippedCount
                },
                //"top-today" => topSellingToday ?? new TopSellingProductDTO(),
                //"top-week" => topSellingWeek ?? new TopSellingProductDTO(),
                _ => new
                {
                    TotalUsers = totalUsers,
                    TotalRevenue = totalRevenue,
                    TotalProducts = totalProducts,
                    TotalInventoryValue = totalInventoryValue,
                    LowStockCount = lowStockCount,
                    DeliveredOrdersCount = deliveredCount,
                    ShippedOrdersCount = shippedCount,
                    //TopSellingToday = topSellingToday ?? new TopSellingProductDTO(),
                    //TopSellingWeek = topSellingWeek ?? new TopSellingProductDTO()
                }
            };

            return new ApiResponse<object?>(200, "Dashboard stats fetched successfully", data);
        }
    }
}
