using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Treazr_Backend.Services.interfaces;

namespace Treazr_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy ="Admin")]
    public class DashBoardController:ControllerBase
    {
        private readonly IDashBoardService _dashboard;

        public DashBoardController(IDashBoardService dashboard)
        {
            _dashboard = dashboard;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard(string type)
        {
            var result= await _dashboard.GetDashboardStatsAsync(type);
            return StatusCode(result.StatusCode, result);
        }

    }
}
