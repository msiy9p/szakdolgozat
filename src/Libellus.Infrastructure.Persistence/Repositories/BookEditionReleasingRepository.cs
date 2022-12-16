using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using PersistenceBookEdition = Libellus.Infrastructure.Persistence.DataModels.BookEdition;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class BookEditionReleasingRepository : BaseRepository<BookEditionReleasingRepository>,
    IBookEditionReleasingRepository
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public BookEditionReleasingRepository(ApplicationContext context, ILogger<BookEditionReleasingRepository> logger,
        IDateTimeProvider dateTimeProvider) : base(context, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    internal BookEditionReleasingRepository(ApplicationContext context, ILogger logger,
        IDateTimeProvider dateTimeProvider) : base(context, logger)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<ICollection<BookEditionId>>> GetAllIdsReleasingAsync(
        CancellationToken cancellationToken = default)
    {
        var found = await Context.BookEditions
            .ApplyReleaseFilter(_dateTimeProvider)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        return found.ToResultCollection();
    }

    public async Task<Result<BookEditionReleasingVm>> GetReleasingVmByIdAsync(BookEditionId id,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.BookEditions
            .Where(x => x.Id == id)
            .ApplyReleaseFilter(_dateTimeProvider)
            .Select(x => new { x.Title, x.PublishedOn })
            .FirstOrDefaultAsync(cancellationToken);

        if (found is null)
        {
            return BookEditionErrors.BookEditionNotFound.ToErrorResult<BookEditionReleasingVm>();
        }

        var userEmailVms = await Context.BookEditions
            .Include(x => x.Book)
            .ThenInclude(x => x.Group)
            .ThenInclude(x => x.Members)
            .ThenInclude(x => x.User)
            .Where(x => x.Id == id)
            .ApplyReleaseFilter(_dateTimeProvider)
            .SelectMany(x => x.Book.Group.Members
                .Select(y => new UserEmailVm(y.User.Email!, y.User.UserName!)))
            .ToListAsync(cancellationToken);

        var temp = found.PublishedOn!.Value.ToDateTimeUtc();
        var date = new DateOnly(temp.Year, temp.Month, temp.Day);

        return new BookEditionReleasingVm(id, found!.Title, date, userEmailVms).ToResult();
    }
}

internal static class BookEditionReleasingRepositoryHelper
{
    public static IQueryable<PersistenceBookEdition> ApplyReleaseFilter(
        this IQueryable<PersistenceBookEdition> queryable, IDateTimeProvider dateTimeProvider)
    {
        var instant = dateTimeProvider.Now;

        return queryable
            .Where(x => x.PublishedOn != null && x.PublishedOn > instant &&
                        (x.PublishedOn.Value - instant).TotalHours <= 24);
    }
}