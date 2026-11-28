using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.Room;
using Rezerv.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ApiControllerBase
{
    private readonly IRoomManagementService _roomService;

    public RoomsController(IRoomManagementService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> CreateRoom(
        CreateRoomDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var room = await _roomService.CreateRoomAsync(
                dto,
                userId,
                isAdmin,
                cancellationToken);

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
    public async Task<IActionResult> GetRoom(
        Guid id,
        CancellationToken cancellationToken)
    {
        var room = await _roomService.GetRoomAsync(id, cancellationToken);

        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("roomtype/{roomTypeId:guid}")]
    public async Task<IActionResult> GetRoomsByRoomType(
        Guid roomTypeId,
        CancellationToken cancellationToken)
    {
        var rooms = await _roomService.GetRoomsByRoomTypeAsync(
            roomTypeId,
            cancellationToken);

        return Ok(rooms);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> UpdateRoom(
    Guid id,
    UpdateRoomDto dto,
    CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var room = await _roomService.UpdateRoomAsync(
                id, dto, userId, isAdmin, cancellationToken);

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

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,HotelManager")]
    public async Task<IActionResult> ToggleRoomStatus(
        Guid id,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");

        try
        {
            var room = await _roomService.ToggleRoomStatusAsync(
                id, userId, isAdmin, cancellationToken);

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
}