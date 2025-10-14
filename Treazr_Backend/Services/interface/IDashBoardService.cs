using Treazr_Backend.Common;

namespace Treazr_Backend.Services.interfaces
{
     public interface IDashBoardService
    {
        Task<ApiResponse<object?>> GetDashboardStatsAsync(string type = "all");
    }
}
