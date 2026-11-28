using System.ComponentModel.DataAnnotations;

namespace Rezerv.Application.DTOs.Common;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is Guid id && id != Guid.Empty;
    }
}
