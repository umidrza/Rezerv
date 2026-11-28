using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.HotelManager;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class HotelManagersController : ControllerBase
{
    private readonly IHotelManagerService _hotelManagerService;

    public HotelManagersController(IHotelManagerService hotelManagerService)
    {
        _hotelManagerService = hotelManagerService;
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignManager(
        AssignHotelManagerDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _hotelManagerService.AssignManagerAsync(
                dto,
                cancellationToken);

            return Ok(new { message = "Manager assigned successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}