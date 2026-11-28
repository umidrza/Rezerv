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
    public async Task<IActionResult> GetMyFavorites(
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var favorites = await _favoriteService.GetMyFavoritesAsync(
            userId,
            cancellationToken);

        return Ok(favorites);
    }

    [HttpPost("{hotelId:guid}")]
    public async Task<IActionResult> AddFavorite(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _favoriteService.AddFavoriteAsync(
            userId,
            hotelId,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{hotelId:guid}")]
    public async Task<IActionResult> RemoveFavorite(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        await _favoriteService.RemoveFavoriteAsync(
            userId,
            hotelId,
            cancellationToken);

        return NoContent();
    }

    [HttpGet("{hotelId:guid}/status")]
    public async Task<IActionResult> IsFavorite(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var isFavorite = await _favoriteService.IsFavoriteAsync(
            userId,
            hotelId,
            cancellationToken);

        return Ok(isFavorite);
    }
}