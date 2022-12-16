using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public class LanguageDto
{
    public ShortName Name { get; init; }
    public LanguageId? LanguageId { get; init; }

    public LanguageDto(ShortName name, LanguageId? languageId)
    {
        Name = name;
        LanguageId = languageId;
    }
}