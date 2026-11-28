using Microsoft.Extensions.Caching.Memory;
using Rezerv.Application.DTOs.Dashboard;
using Rezerv.Application.Interfaces;

namespace Rezerv.Application.Services;

public class DashboardService : IDashboardService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromSeconds(15);

    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;

    public DashboardService(
        IBookingRepository bookingRepository,
        IHotelRepository hotelRepository,
        IUserRepository userRepository,
        IMemoryCache cache)
    {
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
        _cache = cache;
    }

    public Task<DashboardDto> GetAdminDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        return GetOrCreateDashboardAsync(
            "dashboard:admin",
            cancellationToken,
            async () =>
            {
                var bookings = await _bookingRepository.GetDashboardCountsAsync(
                    cancellationToken);
                var hotels = await _hotelRepository.GetDashboardCountsAsync(
                    cancellationToken);
                var totalUsers = await _userRepository.CountAsync(
                    cancellationToken);
                var recentBookings = await _bookingRepository.GetRecentAsync(
                    5,
                    cancellationToken);

                return new DashboardDto
                {
                    TotalBookings = bookings.TotalBookings,
                    PendingBookings = bookings.PendingBookings,
                    ApprovedBookings = bookings.ApprovedBookings,
                    TotalHotels = hotels.TotalHotels,
                    ActiveHotels = hotels.ActiveHotels,
                    TotalUsers = totalUsers,
                    RecentBookings = recentBookings
                        .Select(BookingService.MapToDto)
                        .ToList()
                };
            });
    }

    public Task<DashboardDto> GetManagerDashboardAsync(
        Guid managerId,
        CancellationToken cancellationToken = default)
    {
        return GetOrCreateDashboardAsync(
            $"dashboard:manager:{managerId}",
            cancellationToken,
            async () =>
            {
                var bookings = await _bookingRepository
                    .GetDashboardCountsByManagerIdAsync(
                        managerId,
                        cancellationToken);
                var hotels = await _hotelRepository
                    .GetDashboardCountsByManagerIdAsync(
                        managerId,
                        cancellationToken);
                var recentBookings = await _bookingRepository
                    .GetRecentByManagerIdAsync(
                        managerId,
                        5,
                        cancellationToken);

                return new DashboardDto
                {
                    TotalBookings = bookings.TotalBookings,
                    PendingBookings = bookings.PendingBookings,
                    ApprovedBookings = bookings.ApprovedBookings,
                    TotalHotels = hotels.TotalHotels,
                    ActiveHotels = hotels.ActiveHotels,
                    RecentBookings = recentBookings
                        .Select(BookingService.MapToDto)
                        .ToList()
                };
            });
    }

    public Task<DashboardDto> GetCustomerDashboardAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return GetOrCreateDashboardAsync(
            $"dashboard:customer:{userId}",
            cancellationToken,
            async () =>
            {
                var bookings = await _bookingRepository
                    .GetDashboardCountsByUserIdAsync(userId, cancellationToken);
                var recentBookings = await _bookingRepository
                    .GetRecentByUserIdAsync(userId, 5, cancellationToken);

                return new DashboardDto
                {
                    TotalBookings = bookings.TotalBookings,
                    PendingBookings = bookings.PendingBookings,
                    ApprovedBookings = bookings.ApprovedBookings,
                    RecentBookings = recentBookings
                        .Select(BookingService.MapToDto)
                        .ToList()
                };
            });
    }

    private async Task<DashboardDto> GetOrCreateDashboardAsync(
        string cacheKey,
        CancellationToken cancellationToken,
        Func<Task<DashboardDto>> createDashboardAsync)
    {
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;
                cancellationToken.ThrowIfCancellationRequested();
                return await createDashboardAsync();
            }) ?? throw new InvalidOperationException(
                "Dashboard cache returned an empty result.");
    }
}
