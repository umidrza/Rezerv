using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.Interfaces;
using System.Security.Claims;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
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
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dashboard = await _dashboardService.GetManagerDashboardAsync(userId);
        return Ok(dashboard);
    }

    [HttpGet("customer")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCustomerDashboard()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var dashboard = await _dashboardService.GetCustomerDashboardAsync(userId);
        return Ok(dashboard);
    }
}