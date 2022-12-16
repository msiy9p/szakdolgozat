using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public class PublisherDto
{
    public ShortName Name { get; init; }
    public PublisherId? PublisherId { get; init; }

    public PublisherDto(ShortName name, PublisherId? publisherId)
    {
        Name = name;
        PublisherId = publisherId;
    }
}