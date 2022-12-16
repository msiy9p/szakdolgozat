using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.ViewModels;

public sealed class AuthorVm
{
    public AuthorId AuthorId { get; init; }
    public AuthorFriendlyId AuthorFriendlyId { get; init; }
    public Name Name { get; init; }

    public AuthorVm(AuthorId authorId, AuthorFriendlyId authorFriendlyId, Name name)
    {
        AuthorId = authorId;
        AuthorFriendlyId = authorFriendlyId;
        Name = Guard.Against.Null(name);
    }
}