using Microsoft.AspNetCore.Identity;

namespace Rezerv.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
}