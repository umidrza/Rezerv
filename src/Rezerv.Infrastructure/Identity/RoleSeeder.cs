
using Microsoft.AspNetCore.Identity;

namespace Rezerv.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles =
        {
            "Admin",
            "HotelManager",
            "Customer"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
