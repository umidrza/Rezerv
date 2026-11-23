using Rezerv.Application.DTOs.Booking;

namespace Rezerv.Application.DTOs.Dashboard;

public class DashboardDto
{
    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int ApprovedBookings { get; set; }

    public int TotalHotels { get; set; }
    public int ActiveHotels { get; set; }

    public int TotalUsers { get; set; }

    public List<BookingDto> RecentBookings { get; set; } = [];
}