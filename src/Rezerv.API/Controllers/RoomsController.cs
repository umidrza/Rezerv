using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Room;
using Rezerv.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomManagementService _roomService;

    public RoomsController(IRoomManagementService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> CreateRoom(CreateRoomDto dto)
    {
        var userId = Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var room = await _roomService.CreateRoomAsync(
                dto,
                userId,
                isAdmin);

            return Ok(room);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var room = await _roomService.GetRoomAsync(id);

        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("roomtype/{roomTypeId:guid}")]
    public async Task<IActionResult> GetRoomsByRoomType(Guid roomTypeId)
    {
        var rooms = await _roomService.GetRoomsByRoomTypeAsync(roomTypeId);

        return Ok(rooms);
    }
}