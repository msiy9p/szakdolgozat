using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.DefaultEntities;
using Libellus.Domain.Models;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Microsoft.Extensions.Logging;

namespace Libellus.Infrastructure.Persistence.Repositories.Common;

internal sealed class GroupCreatorHelper : BaseRepository
{
    private readonly UserId _currentUserId;
    private readonly GroupId _currentGroupId;
    private readonly IDateTimeProvider _dateTimeProvider;

    internal GroupCreatorHelper(ApplicationContext context, ILogger logger, UserId currentUserId,
        GroupId currentGroupId, IDateTimeProvider dateTimeProvider) : base(context, logger)
    {
        _currentUserId = currentUserId;
        _currentGroupId = currentGroupId;
        _dateTimeProvider = dateTimeProvider;
    }

    internal async Task<Result> AddDefaultsIfNotExistsAsync(CancellationToken cancellationToken = default)
    {
        if (DefaultFormats.HasDefaults)
        {
            IFormatRepository repository = new FormatRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultFormats.Create(_currentUserId, _dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        if (DefaultGenres.HasDefaults)
        {
            IGenreRepository repository = new GenreRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultGenres.Create(_currentUserId, _dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        if (DefaultLabels.HasDefaults)
        {
            ILabelRepository repository = new LabelRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultLabels.Create(_dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        if (DefaultLanguages.HasDefaults)
        {
            ILanguageRepository repository = new LanguageRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultLanguages.Create(_currentUserId, _dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        if (DefaultLiteratureForms.HasDefaults)
        {
            ILiteratureFormRepository repository = new LiteratureFormRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultLiteratureForms.Create(_currentUserId, _dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        if (DefaultPublishers.HasDefaults)
        {
            IPublisherRepository repository = new PublisherRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultPublishers.Create(_currentUserId, _dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        if (DefaultTags.HasDefaults)
        {
            ITagRepository repository = new TagRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultTags.Create(_currentUserId, _dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        if (DefaultWarningTags.HasDefaults)
        {
            IWarningTagRepository repository = new WarningTagRepository(Context, _currentGroupId, Logger);
            foreach (var item in DefaultWarningTags.Create(_currentUserId, _dateTimeProvider))
            {
                var result = await repository.AddIfNotExistsAsync(item, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        return Result.Success();
    }
}