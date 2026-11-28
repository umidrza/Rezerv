using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.Interfaces;

namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/favorites")]
[Authorize(Roles = "Customer")]
public class HotelFavoritesController : ApiControllerBase
{
    private readonly IHotelFavoriteService _favoriteService;

    public HotelFavoritesController(IHotelFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var favorites = await _favoriteService.GetMyFavoritesAsync(userId);

        return Ok(favorites);
    }

    [HttpPost("{hotelId:guid}")]
    public async Task<IActionResult> AddFavorite(Guid hotelId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _favoriteService.AddFavoriteAsync(userId, hotelId);

        return NoContent();
    }

    [HttpDelete("{hotelId:guid}")]
    public async Task<IActionResult> RemoveFavorite(Guid hotelId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _favoriteService.RemoveFavoriteAsync(userId, hotelId);

        return NoContent();
    }

    [HttpGet("{hotelId:guid}/status")]
    public async Task<IActionResult> IsFavorite(Guid hotelId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var isFavorite = await _favoriteService.IsFavoriteAsync(
            userId,
            hotelId);

        return Ok(isFavorite);
    }
}