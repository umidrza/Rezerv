using Rezerv.Application.Interfaces;

namespace Rezerv.Application.Security;

public static class HotelManagerAuthorization
{
    public static async Task EnsureCanManageHotelAsync(
        IHotelManagerRepository managerRepository,
        Guid userId,
        Guid hotelId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var isAssignedManager = isAdmin ||
            await managerRepository.IsManagerAssignedAsync(
                userId,
                hotelId,
                cancellationToken);

        EnsureCanManageHotel(isAdmin, isAssignedManager);
    }

    public static void EnsureCanManageHotel(
        bool isAdmin,
        bool isAssignedManager)
    {
        if (!isAdmin && !isAssignedManager)
            throw new UnauthorizedAccessException(
                "You are not assigned to this hotel.");
    }
}
