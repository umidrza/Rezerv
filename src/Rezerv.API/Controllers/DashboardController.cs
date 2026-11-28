using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var dashboard = await _dashboardService.GetAdminDashboardAsync();
        return Ok(dashboard);
    }

    [HttpGet("manager")]
    [Authorize(Roles = "HotelManager")]
    public async Task<IActionResult> GetManagerDashboard()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var dashboard = await _dashboardService.GetManagerDashboardAsync(userId);
        return Ok(dashboard);
    }

    [HttpGet("customer")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCustomerDashboard()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var dashboard = await _dashboardService.GetCustomerDashboardAsync(userId);
        return Ok(dashboard);
    }
}