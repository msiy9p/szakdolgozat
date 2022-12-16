using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public class WarningTagDto
{
    public ShortName Name { get; init; }
    public WarningTagId? WarningTagId { get; init; }

    public WarningTagDto(ShortName name, WarningTagId? warningTagId)
    {
        Name = name;
        WarningTagId = warningTagId;
    }
}