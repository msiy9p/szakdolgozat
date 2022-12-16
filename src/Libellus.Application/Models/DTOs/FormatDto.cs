using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public class FormatDto
{
    public ShortName Name { get; init; }
    public bool IsDigital { get; init; }
    public FormatId? FormatId { get; init; }

    public FormatDto(ShortName name, bool isDigital, FormatId? formatId)
    {
        Name = name;
        IsDigital = isDigital;
        FormatId = formatId;
    }
}