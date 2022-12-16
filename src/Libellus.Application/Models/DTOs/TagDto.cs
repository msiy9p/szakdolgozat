using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public class TagDto
{
    public ShortName Name { get; init; }
    public TagId? TagId { get; init; }

    public TagDto(ShortName name, TagId? tagId)
    {
        Name = name;
        TagId = tagId;
    }
}