using Ardalis.GuardClauses;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Domain.ViewModels;

public sealed class BookEditionReleasingVm
{
    private readonly List<UserEmailVm> _users = new();

    public BookEditionId BookEditionId { get; init; }
    public string BookEditionTitle { get; init; }
    public DateOnly ReleaseDate { get; init; }
    public IReadOnlyCollection<UserEmailVm> Users => _users.AsReadOnly();

    public BookEditionReleasingVm(BookEditionId bookEditionId, string bookEditionTitle, DateOnly releaseDate,
        IEnumerable<UserEmailVm> users)
    {
        BookEditionId = bookEditionId;
        BookEditionTitle = Guard.Against.NullOrWhiteSpace(bookEditionTitle);
        ReleaseDate = releaseDate;
        Guard.Against.Null(users);

        foreach (var user in users)
        {
            if (user is not null)
            {
                _users.Add(user);
            }
        }
    }
}