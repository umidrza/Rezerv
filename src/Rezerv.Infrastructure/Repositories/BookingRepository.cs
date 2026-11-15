using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Infrastructure.Data;

namespace Rezerv.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _context;

    public BookingRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Bookings
            .Include(b => b.BookingRooms)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.Bookings
            .Include(b => b.BookingRooms)
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }
}