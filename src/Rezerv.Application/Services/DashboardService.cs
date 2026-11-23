using Rezerv.Application.DTOs.Dashboard;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Enums;

namespace Rezerv.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUserRepository _userRepository;

    public DashboardService(
        IBookingRepository bookingRepository,
        IHotelRepository hotelRepository,
        IUserRepository userRepository)
    {
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
        _userRepository = userRepository;
    }

    public async Task<DashboardDto> GetAdminDashboardAsync()
    {
        var recentBookings = await _bookingRepository.GetRecentAsync(5);

        return new DashboardDto
        {
            TotalBookings = await _bookingRepository.CountAsync(),
            PendingBookings = await _bookingRepository.CountAsync(BookingStatus.Submitted),
            ApprovedBookings = await _bookingRepository.CountAsync(BookingStatus.Approved),
            TotalHotels = await _hotelRepository.CountAsync(),
            ActiveHotels = await _hotelRepository.CountActiveAsync(),
            TotalUsers = await _userRepository.CountAsync(),
            RecentBookings = recentBookings.Select(BookingService.MapToDto).ToList()
        };
    }

    public async Task<DashboardDto> GetManagerDashboardAsync(Guid managerId)
    {
        var recentBookings = await _bookingRepository.GetRecentByManagerIdAsync(managerId, 5);

        return new DashboardDto
        {
            TotalBookings = await _bookingRepository.CountByManagerIdAsync(managerId),
            PendingBookings = await _bookingRepository.CountByManagerIdAsync(
                managerId, BookingStatus.Submitted),
            ApprovedBookings = await _bookingRepository.CountByManagerIdAsync(
                managerId, BookingStatus.Approved),
            TotalHotels = await _hotelRepository.CountByManagerIdAsync(managerId),
            ActiveHotels = await _hotelRepository.CountActiveByManagerIdAsync(managerId),
            RecentBookings = recentBookings.Select(BookingService.MapToDto).ToList()
        };
    }

    public async Task<DashboardDto> GetCustomerDashboardAsync(Guid userId)
    {
        var recentBookings = await _bookingRepository.GetRecentByUserIdAsync(userId, 5);

        return new DashboardDto
        {
            TotalBookings = await _bookingRepository.CountByUserIdAsync(userId),
            PendingBookings = await _bookingRepository.CountByUserIdAsync(
                userId, BookingStatus.Submitted),
            ApprovedBookings = await _bookingRepository.CountByUserIdAsync(
                userId, BookingStatus.Approved),
            RecentBookings = recentBookings.Select(BookingService.MapToDto).ToList()
        };
    }
}