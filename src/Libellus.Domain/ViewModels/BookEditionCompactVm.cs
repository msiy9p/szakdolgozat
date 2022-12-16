using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.ViewModels;

public sealed class BookEditionCompactVm
{
    private readonly List<AuthorVm> _authors = new();

    public BookEditionId BookEditionId { get; init; }
    public BookEditionFriendlyId BookEditionFriendlyId { get; init; }
    public Title Title { get; init; }
    public IReadOnlyCollection<AuthorVm> Authors => _authors;

    public BookEditionCompactVm(BookEditionId bookEditionId, BookEditionFriendlyId bookEditionFriendlyId, Title title,
        IEnumerable<AuthorVm> authors)
    {
        BookEditionId = bookEditionId;
        BookEditionFriendlyId = bookEditionFriendlyId;
        Title = Guard.Against.Null(title);
        _authors.AddRange(authors);
    }

    public static BookEditionCompactVm Convert(BookEdition bookEdition)
    {
        Guard.Against.Null(bookEdition);

        return new BookEditionCompactVm(bookEdition.Id, bookEdition.FriendlyId, bookEdition.Title,
            bookEdition.Book.Authors);
    }
}