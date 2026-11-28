using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Rezerv.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected bool TryGetUserId(out Guid userId)
    {
        return Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out userId);
    }
}
