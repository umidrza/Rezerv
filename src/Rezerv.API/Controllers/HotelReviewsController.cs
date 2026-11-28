using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rezerv.Application.DTOs.HotelReview;
using Rezerv.Application.Interfaces;
namespace Rezerv.API.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/reviews")]
public class HotelReviewsController : ApiControllerBase
{
    private readonly IHotelReviewService _reviewService;

    public HotelReviewsController(IHotelReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetHotelReviewsAsync(
            hotelId,
            cancellationToken);
        return Ok(reviews);
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateReview(
        Guid hotelId,
        [FromBody] CreateHotelReviewDto dto,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var review = await _reviewService.CreateReviewAsync(
            hotelId,
            userId,
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetReviews),
            new { hotelId },
            review);
    }

    [HttpGet("eligibility")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CanReview(
        Guid hotelId,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized();

        var canReview = await _reviewService.CanReviewHotelAsync(
            hotelId,
            userId,
            cancellationToken);

        return Ok(canReview);
    }
}