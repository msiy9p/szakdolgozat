using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Common.Security;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using static Libellus.Domain.Errors.DomainErrors;
using DomainGroup = Libellus.Domain.Entities.Group;
using DomainPost = Libellus.Domain.Entities.Post;
using PersistenceGroup = Libellus.Infrastructure.Persistence.DataModels.Group;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class GroupRepository : BaseRepository<GroupRepository>, IGroupRepository
{
    public UserId CurrentUserId { get; init; }
    private readonly IDateTimeProvider _dateTimeProvider;

    public GroupRepository(ApplicationContext context, ICurrentUserService currentUserService,
        ILogger<GroupRepository> logger, IDateTimeProvider dateTimeProvider) : base(context, logger)
    {
        if (currentUserService.UserId is null)
        {
            throw new ArgumentNullException(nameof(currentUserService), "UserId cannot be null.");
        }

        _dateTimeProvider = dateTimeProvider;

        CurrentUserId = currentUserService.UserId.Value;
    }

    private async Task<UserVm?> GetUserVmAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        IUserReadOnlyRepository userRepository = new UserRepository(Context, Logger);
        var result = await userRepository.GetVmByIdAsync(userId, cancellationToken);
        if (result.IsError)
        {
            return null;
        }

        return result.Value;
    }

    private async Task<UserPicturedVm?> GetUserPicturedVmAsync(UserId userId,
        CancellationToken cancellationToken = default)
    {
        IUserReadOnlyRepository userRepository = new UserRepository(Context, Logger);
        var result = await userRepository.GetPicturedVmByIdAsync(userId, cancellationToken);
        if (result.IsError)
        {
            return null;
        }

        return result.Value;
    }

    private async Task<Result> CreateDefaultMembership(GroupId groupId, CancellationToken cancellationToken = default)
    {
        var ownerId = await Context.GroupRoles
            .Where(x => x.NameNormalized == SecurityConstants.GroupRoles.Owner.ToNormalizedUpperInvariant())
            .Select(x => x.GroupRoleId)
            .FirstAsync(cancellationToken);

        var datetime = _dateTimeProvider.UtcNow;
        var item = new GroupUserMembership(groupId, CurrentUserId, ownerId,
            datetime, datetime);

        await Context.AddAsync(item, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<bool>> ExistsAsync(GroupId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainGroup>> GetByIdAsync(GroupId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<DomainGroup>();
        }

        return GroupMapper.Map(found);
    }

    public async Task<Result<GroupNameVm>> GetNameVmByIdAsync(GroupId id, CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .Where(x => x.Id == id)
            .Select(x => new { GroupId = x.Id, FriendlyId = x.FriendlyId, Name = x.Name })
            .FirstOrDefaultAsync(cancellationToken);

        if (found is null)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<GroupNameVm>();
        }

        var vm = new GroupNameVm(found.GroupId, new GroupFriendlyId(found.FriendlyId), new Name(found.Name));

        return vm.ToResult();
    }

    public async Task<Result<DomainGroup>> GetByIdWithPostsAsync(GroupId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<DomainGroup>();
        }

        var posts = await Context.Posts
            .Include(x => x.Label)
            .Where(x => x.GroupId == id)
            .ApplySortOrder(SortOrder.Descending)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(posts.Count);
        foreach (var item in posts)
        {
            if (item.CreatorId.HasValue)
            {
                var userVm = await GetUserPicturedVmAsync(item.CreatorId.Value, cancellationToken);
                if (userVm is null)
                {
                    var userVm2 = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
                    var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                    var c1 = PostMapper.Map(item, temp.Value!);
                    if (c1.IsSuccess)
                    {
                        output.Add(c1.Value!);
                    }
                }

                var c2 = PostMapper.Map(item, userVm!);
                if (c2.IsSuccess)
                {
                    output.Add(c2.Value!);
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    output.Add(c2.Value!);
                }
            }
        }

        return GroupMapper.Map(found, output);
    }

    public async Task<Result<PaginationDetail<DomainGroup>>> GetByIdWithPostsAsync(GroupId id,
        PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return GroupErrors.GroupNotFound.ToErrorResult<PaginationDetail<DomainGroup>>();
        }

        var count = await Context.Posts
            .Where(x => x.GroupId == found.Id)
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var posts = await Context.Posts
            .Include(x => x.Label)
            .Where(x => x.GroupId == id)
            .ApplySortOrder(SortOrder.Descending)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(posts.Count);
        foreach (var item in posts)
        {
            if (item.CreatorId.HasValue)
            {
                var userVm = await GetUserPicturedVmAsync(item.CreatorId.Value, cancellationToken);
                if (userVm is null)
                {
                    var userVm2 = await GetUserVmAsync(item.CreatorId.Value, cancellationToken);
                    var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                    var c1 = PostMapper.Map(item, temp.Value!);
                    if (c1.IsSuccess)
                    {
                        output.Add(c1.Value!);
                    }
                }

                var c2 = PostMapper.Map(item, userVm!);
                if (c2.IsSuccess)
                {
                    output.Add(c2.Value!);
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    output.Add(c2.Value!);
                }
            }
        }

        var outputResult = GroupMapper.Map(found, output);
        if (outputResult.IsError)
        {
            return Result<PaginationDetail<DomainGroup>>.Error(outputResult.Errors);
        }

        return Result<PaginationDetail<DomainGroup>>.Success(
            new PaginationDetail<DomainGroup>(count, adjusted, outputResult.Value!));
    }

    public async Task<Result<ICollection<DomainGroup>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainGroup>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var count = await Context.Groups
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainGroup>>>.Success(
            new PaginationDetail<ICollection<DomainGroup>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainGroup>>> GetAllMemberAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => x.Members.Any(y => y.UserId == CurrentUserId))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainGroup>>>> GetAllMemberAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var count = await Context.Groups
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => x.Members.Any(y => y.UserId == CurrentUserId))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainGroup>>>.Success(
            new PaginationDetail<ICollection<DomainGroup>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<GroupNameVm>>> GetAllMemberNamesAsync(
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var found = await Context.Groups
            .Include(x => x.Members)
            .Where(x => x.Members.Any(y => y.UserId == CurrentUserId))
            .ApplySortOrder(sortOrder)
            .Select(x => new
            {
                GroupId = x.Id,
                FriendlyId = x.FriendlyId,
                Name = x.Name,
            })
            .ToListAsync(cancellationToken);

        var output = new List<GroupNameVm>(found.Count);
        output.AddRange(found.Select(item =>
            new GroupNameVm(item.GroupId,
                new GroupFriendlyId(item.FriendlyId),
                new Name(item.Name))));

        return output.ToResultCollection();
    }

    public async Task<Result<ICollection<DomainGroup>>> FindByNameAsync(Name name,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainGroup>>();
        }

        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .Where(x => EF.Functions.ILike(x.NameNormalized, $"%{name.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainGroup>>>> FindByNameAsync(Name name,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (name is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainGroup>>>();
        }

        var count = await Context.Groups
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .Where(x => EF.Functions.ILike(x.NameNormalized, $"%{name.ValueNormalized}%"))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .Where(x => EF.Functions.ILike(x.NameNormalized, $"%{name.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainGroup>>>.Success(
            new PaginationDetail<ICollection<DomainGroup>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainGroup>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainGroup>>();
        }

        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainGroup>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainGroup>>>();
        }

        var count = await Context.Groups
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await Context.Groups
            .Include(x => x.Labels)
            .Include(x => x.Members)
            .Where(x => !x.IsPrivate || x.Members.Any(y => y.UserId == CurrentUserId))
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainGroup>(found.Count);
        foreach (var item in found)
        {
            var map = GroupMapper.Map(item);

            if (map.IsSuccess)
            {
                output.Add(map.Value!);
            }
        }

        return Result<PaginationDetail<ICollection<DomainGroup>>>.Success(
            new PaginationDetail<ICollection<DomainGroup>>(count, adjusted, output));
    }

    public async Task<Result> AddIfNotExistsAsync(DomainGroup entity, CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return exists;
        }

        if (exists.Value)
        {
            return Result.Success();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var item = GroupMapper.Map(entity);

        await Context.Groups.AddAsync(item, cancellationToken);

        await CreateDefaultMembership(entity.Id, cancellationToken);

        var labelChangeTracker = entity.GetLabelTracker();
        if (labelChangeTracker.HasChanges)
        {
            ILabelRepository labelRepository = new LabelRepository(Context, entity.Id, Logger);
            foreach (var label in labelChangeTracker.GetItems())
            {
                var result = await labelRepository.AddIfNotExistsAsync(label, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        var postChangeTracker = entity.GetPostTracker();
        if (postChangeTracker.HasChanges)
        {
            IPostRepository postRepository =
                new PostRepository(Context, CurrentUserId, entity.Id, Logger, _dateTimeProvider);
            foreach (var post in postChangeTracker.GetItems())
            {
                var result = await postRepository.AddIfNotExistsAsync(post, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        var invitationChangeTracker = entity.GetInvitationTracker();
        if (invitationChangeTracker.HasChanges)
        {
            IInvitationRepository invitationRepository = new InvitationRepository(Context, Logger);
            foreach (var invitation in invitationChangeTracker.GetItems())
            {
                var result = await invitationRepository.AddIfNotExistsAsync(invitation, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        return Result.Success();
    }

    public async Task<Result> AddWithDefaultIfNotExistsAsync(DomainGroup entity,
        CancellationToken cancellationToken = default)
    {
        var result = await AddIfNotExistsAsync(entity, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var helper = new GroupCreatorHelper(Context, Logger, CurrentUserId, entity.Id, _dateTimeProvider);
        return await helper.AddDefaultsIfNotExistsAsync(cancellationToken);
    }

    public async Task<Result> UpdateAsync(DomainGroup entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        var labelChangeTracker = entity.GetLabelTracker();
        if (labelChangeTracker.HasChanges)
        {
            ILabelRepository labelRepository = new LabelRepository(Context, entity.Id, Logger);
            await Context.Labels
                .Where(x => x.GroupId == entity.Id)
                .Where(x => labelChangeTracker.GetRemovedIds().Contains(x.Id))
                .ExecuteDeleteAsync(cancellationToken);

            var correctedLabels = new List<LabelId>();
            var removeLabels = new List<LabelId>();
            foreach (var labelId in labelChangeTracker.GetNewItemsChronologically())
            {
                var label = labelChangeTracker.GetById(labelId);
                var result = await labelRepository.AddIfNotExistsAsync(label!, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }

                if (labelId != result.Value)
                {
                    removeLabels.Add(labelId);
                    correctedLabels.Add(result.Value);
                }
            }

            foreach (var labelId in removeLabels)
            {
                entity.RemoveLabelById(labelId, _dateTimeProvider);
            }

            foreach (var labelId in correctedLabels)
            {
                var result = await labelRepository.GetByIdAsync(labelId, cancellationToken);
                if (result.IsSuccess)
                {
                    entity.AddLabel(result.Value, _dateTimeProvider);
                }
            }
        }

        var postChangeTracker = entity.GetPostTracker();
        if (postChangeTracker.HasChanges)
        {
            IPostRepository postRepository =
                new PostRepository(Context, CurrentUserId, entity.Id, Logger, _dateTimeProvider);
            await Context.Posts
                .Where(x => x.GroupId == entity.Id)
                .Where(x => postChangeTracker.GetRemovedIds().Contains(x.Id))
                .ExecuteDeleteAsync(cancellationToken);

            var removePosts = new List<PostId>();
            foreach (var postId in postChangeTracker.GetNewItemsChronologically())
            {
                var post = postChangeTracker.GetById(postId);
                var result = await postRepository.AddIfNotExistsAsync(post!, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }

                if (postId != result.Value)
                {
                    removePosts.Add(result.Value);
                }
            }

            foreach (var postId in removePosts)
            {
                entity.RemovePostById(postId, _dateTimeProvider);
            }
        }

        var item = GroupMapper.Map(entity);

        Context.Groups.Update(item);

        var invitationChangeTracker = entity.GetInvitationTracker();
        if (invitationChangeTracker.HasChanges)
        {
            IInvitationRepository invitationRepository = new InvitationRepository(Context, Logger);
            await Context.Invitations
                .Where(x => x.GroupId == item.Id)
                .Where(x => invitationChangeTracker.GetRemovedIds().Contains(x.Id))
                .ExecuteDeleteAsync(cancellationToken);

            foreach (var invitationId in invitationChangeTracker.GetNewItemsChronologically())
            {
                var invitation = invitationChangeTracker.GetById(invitationId);
                var result = await invitationRepository.AddIfNotExistsAsync(invitation!, cancellationToken);
                if (result.IsError)
                {
                    return result;
                }
            }
        }

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(GroupId id, CancellationToken cancellationToken = default)
    {
        await Context.Groups.Where(x => x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainGroup entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Groups.Remove(GroupMapper.Map(entity));

        return await Task.FromResult(Result.Success());
    }
}

internal static class GroupRepositoryHelper
{
    public static IQueryable<PersistenceGroup> ApplySortOrder(this IQueryable<PersistenceGroup> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.NameNormalized)
                    .ThenBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.NameNormalized)
                    .ThenByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistenceGroup> ApplyPagination(this IQueryable<PersistenceGroup> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }
}