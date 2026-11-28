using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace Rezerv.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected bool TryGetUserId(out Guid userId)
    {
        return Guid.TryParse(
            User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value,
            out userId);
    }
}
