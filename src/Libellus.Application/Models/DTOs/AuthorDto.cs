using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public class AuthorDto
{
    public Name Name { get; init; }
    public AuthorId? AuthorId { get; init; }

    public AuthorDto(Name name, AuthorId? authorId)
    {
        Name = name;
        AuthorId = authorId;
    }
}