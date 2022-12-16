using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Domain.ViewModels;

public sealed class BookCompactVm
{
    private readonly List<AuthorVm> _authors = new();

    public BookId BookId { get; init; }
    public BookFriendlyId BookFriendlyId { get; init; }
    public Title Title { get; init; }
    public IReadOnlyCollection<AuthorVm> Authors => _authors;

    public BookCompactVm(BookId bookId, BookFriendlyId bookFriendlyId, Title title, IEnumerable<AuthorVm> authors)
    {
        BookId = bookId;
        BookFriendlyId = bookFriendlyId;
        Title = Guard.Against.Null(title);
        _authors.AddRange(authors);
    }
}