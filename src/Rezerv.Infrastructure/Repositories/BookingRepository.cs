using System.Data;
using Microsoft.EntityFrameworkCore;
using Rezerv.Application.Interfaces;
using Rezerv.Domain.Entities;
using Rezerv.Domain.Enums;
using Rezerv.Infrastructure.Data;
using Npgsql;

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
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddIfAvailableAsync(Booking booking)
    {
        try
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            var roomIsActive = await _context.Rooms
                .AnyAsync(r => r.Id == booking.RoomId && r.IsActive);

            var hasConflictingBooking = await _context.Bookings
                .AnyAsync(b =>
                    b.RoomId == booking.RoomId &&
                    (b.Status == BookingStatus.Submitted ||
                     b.Status == BookingStatus.Approved) &&
                    b.CheckIn < booking.CheckOut &&
                    b.CheckOut > booking.CheckIn);

            if (!roomIsActive || hasConflictingBooking)
            {
                throw new InvalidOperationException(
                    "Room is no longer available for these dates.");
            }

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (PostgresException ex) when (IsSerializationFailure(ex))
        {
            throw CreateAvailabilityConflictException(ex);
        }
        catch (DbUpdateException ex) when (IsSerializationFailure(ex))
        {
            throw CreateAvailabilityConflictException(ex);
        }
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetByHotelIdAsync(Guid hotelId)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .Where(b => b.HotelId == hotelId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Bookings
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsRoomBookedAsync(
        Guid roomId,
        DateTime checkIn,
        DateTime checkOut)
    {
        return await _context.Bookings.AnyAsync(b =>
            b.RoomId == roomId &&
            (b.Status == BookingStatus.Submitted ||
             b.Status == BookingStatus.Approved) &&
            b.CheckIn < checkOut &&
            b.CheckOut > checkIn);
    }

    public async Task<bool> ApproveAsync(Guid bookingId)
    {
        try
        {
            await using var transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable);

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return false;

            await _context.Entry(booking).ReloadAsync();

            if (booking.Status != BookingStatus.Submitted)
                return false;

            var roomIsActive = await _context.Rooms
                .AnyAsync(r => r.Id == booking.RoomId && r.IsActive);

            var hasConflictingBooking = await _context.Bookings
                .AnyAsync(b =>
                    b.Id != bookingId &&
                    b.RoomId == booking.RoomId &&
                    (b.Status == BookingStatus.Submitted ||
                     b.Status == BookingStatus.Approved) &&
                    b.CheckIn < booking.CheckOut &&
                    b.CheckOut > booking.CheckIn);

            if (!roomIsActive || hasConflictingBooking)
            {
                throw new InvalidOperationException(
                    "Room is no longer available for this booking.");
            }

            booking.Status = BookingStatus.Approved;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }
        catch (PostgresException ex) when (IsSerializationFailure(ex))
        {
            throw CreateAvailabilityConflictException(ex);
        }
        catch (DbUpdateException ex) when (IsSerializationFailure(ex))
        {
            throw CreateAvailabilityConflictException(ex);
        }
    }

    public async Task<int> CountAsync(BookingStatus? status = null)
    {
        return await BuildQuery(status).CountAsync();
    }

    public async Task<int> CountByUserIdAsync(
        Guid userId,
        BookingStatus? status = null)
    {
        return await BuildUserQuery(userId, status).CountAsync();
    }

    public async Task<int> CountByManagerIdAsync(
        Guid managerId,
        BookingStatus? status = null)
    {
        return await BuildManagerQuery(managerId, status).CountAsync();
    }

    public async Task<List<Booking>> GetRecentAsync(int count)
    {
        return await IncludeBookingDetails(BuildQuery())
            .OrderByDescending(b => b.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetRecentByUserIdAsync(
        Guid userId,
        int count)
    {
        return await IncludeBookingDetails(BuildUserQuery(userId))
            .OrderByDescending(b => b.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetRecentByManagerIdAsync(
        Guid managerId,
        int count)
    {
        return await IncludeBookingDetails(BuildManagerQuery(managerId))
            .OrderByDescending(b => b.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    private IQueryable<Booking> BuildQuery(BookingStatus? status = null)
    {
        var query = _context.Bookings.AsQueryable();

        if (status.HasValue)
            query = query.Where(b => b.Status == status.Value);

        return query;
    }

    private IQueryable<Booking> BuildManagerQuery(
        Guid managerId,
        BookingStatus? status = null)
    {
        var query = BuildQuery(status)
            .Where(b => b.Hotel.HotelManagers
                .Any(hm => hm.UserId == managerId));

        return query;
    }

    private IQueryable<Booking> BuildUserQuery(
        Guid userId,
        BookingStatus? status = null)
    {
        return BuildQuery(status)
            .Where(b => b.UserId == userId);
    }

    private static IQueryable<Booking> IncludeBookingDetails(
        IQueryable<Booking> query)
    {
        return query
            .Include(b => b.Hotel)
            .Include(b => b.Room)
                .ThenInclude(r => r.RoomType);
    }

    private static bool IsSerializationFailure(Exception exception)
    {
        return exception.GetBaseException() is PostgresException postgresException &&
               postgresException.SqlState == PostgresErrorCodes.SerializationFailure;
    }

    private static InvalidOperationException CreateAvailabilityConflictException(
        Exception exception)
    {
        return new InvalidOperationException(
            "Room availability changed during the operation. Please try again.",
            exception);
    }

}