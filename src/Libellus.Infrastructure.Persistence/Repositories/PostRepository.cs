using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using Libellus.Infrastructure.Persistence.DataModels.Contexts;
using Libellus.Infrastructure.Persistence.Mapping;
using Libellus.Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Libellus.Domain.Errors.DomainErrors;
using DomainComment = Libellus.Domain.Entities.Comment;
using DomainPost = Libellus.Domain.Entities.Post;
using PersistencePost = Libellus.Infrastructure.Persistence.DataModels.Post;

namespace Libellus.Infrastructure.Persistence.Repositories;

internal sealed class PostRepository : BaseGroupedRepository<PostRepository, PersistencePost>, IPostRepository
{
    public UserId CurrentUserId { get; init; }
    private readonly IDateTimeProvider _dateTimeProvider;

    public PostRepository(ApplicationContext context, ICurrentUserService currentUserService,
        ICurrentGroupService currentGroupService,
        ILogger<PostRepository> logger, IDateTimeProvider dateTimeProvider) : base(context, currentGroupService, logger)
    {
        if (currentUserService.UserId is null)
        {
            throw new ArgumentNullException(nameof(currentUserService), "UserId cannot be null.");
        }

        _dateTimeProvider = dateTimeProvider;

        CurrentUserId = currentUserService.UserId.Value;
    }

    internal PostRepository(ApplicationContext context, UserId currentUserId, GroupId currentGroupId, ILogger logger,
        IDateTimeProvider dateTimeProvider) : base(context,
        currentGroupId, logger)
    {
        _dateTimeProvider = dateTimeProvider;
        CurrentUserId = currentUserId;
    }

    protected override IQueryable<PersistencePost> GetFiltered()
    {
        return Context.Posts
            .Include(x => x.LockedPost)
            .Include(x => x.Label)
            .Where(x => x.GroupId == CurrentGroupId);
    }

    private async Task<bool> IsGroupMemberAsync(CancellationToken cancellationToken = default)
    {
        return await Context.GroupUserMemberships
            .Where(x => x.GroupId == CurrentGroupId)
            .AnyAsync(x => x.UserId == CurrentUserId, cancellationToken);
    }

    public async Task<Result<bool>> ExistsAsync(PostId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        return Result<bool>.Success(found);
    }

    public async Task<Result<DomainPost>> GetByIdAsync(PostId id, CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return PostErrors.PostNotFound.ToErrorResult<DomainPost>();
        }

        Result<DomainPost> mapResult;
        if (found.CreatorId.HasValue)
        {
            var userVm = await GetUserPicturedVmAsync(found.CreatorId.Value, cancellationToken);
            if (userVm is null)
            {
                var userVm2 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                mapResult = PostMapper.Map(found, temp.Value!);
            }
            else
            {
                mapResult = PostMapper.Map(found, userVm!);
            }
        }
        else
        {
            mapResult = PostMapper.Map(found, null);
        }

        if (mapResult.IsError)
        {
            return mapResult;
        }

        var commentCount = await GetCommentCountAsync(id, cancellationToken);
        if (commentCount.IsSuccess)
        {
            mapResult.Value.SetCommentCountOffset(commentCount.Value);
        }

        return mapResult;
    }

    public async Task<Result<ICollection<DomainPost>>> GetAllAsync(SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<int>> GetCountAsync(CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var count = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<int>> GetCommentCountAsync(PostId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .AnyAsync(x => x.Id == id, cancellationToken);

        if (!found)
        {
            return PostErrors.PostNotFound.ToErrorResult<int>();
        }

        var count = await Context.Comments
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.PostId == id)
            .CountAsync(cancellationToken);

        return Result<int>.Success(count);
    }

    public async Task<Result<bool>> IsLockedAsync(PostId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return PostErrors.PostNotFound.ToErrorResult<bool>();
        }

        var result = await Context.LockedPosts.AnyAsync(x => x.PostId == id, cancellationToken);
        return result.ToResult();
    }

    public async Task<Result<UserId?>> GetCreatorIdAsync(PostId id, CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefaultAsync(cancellationToken);

        return found.ToResult();
    }

    public async Task<Result<DomainPost>> GetByIdWithCommentsAsync(PostId id, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return PostErrors.PostNotFound.ToErrorResult<DomainPost>();
        }

        var persistenceComments = await Context.Comments
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.PostId == found.Id)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var comments = new List<DomainComment>(persistenceComments.Count);
        foreach (var comment in persistenceComments)
        {
            var userVm3 = await GetUserPicturedVmAsync(comment.CreatorId, cancellationToken);
            if (userVm3 is null)
            {
                var userVm4 = await GetUserVmAsync(comment.CreatorId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm4!.UserId, userVm3!.UserName, null);

                var result1 = CommentMapper.Map(comment, temp.Value!);
                if (result1.IsSuccess)
                {
                    comments.Add(result1.Value!);
                }
            }
            else
            {
                var result = CommentMapper.Map(comment, userVm3!);
                if (result.IsSuccess)
                {
                    comments.Add(result.Value!);
                }
            }
        }

        if (found.CreatorId.HasValue)
        {
            var userVm = await GetUserPicturedVmAsync(found.CreatorId.Value, cancellationToken);
            if (userVm is null)
            {
                var userVm2 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                return PostMapper.Map(found, temp.Value!, comments);
            }

            return PostMapper.Map(found, userVm!, comments);
        }

        return PostMapper.Map(found, null, comments);
    }

    public async Task<Result<PaginationDetail<DomainPost>>> GetByIdWithCommentsAsync(PostId id,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (found is null)
        {
            return PostErrors.PostNotFound.ToErrorResult<PaginationDetail<DomainPost>>();
        }

        var count = await Context.Comments
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.PostId == found.Id)
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var persistenceComments = await Context.Comments
            .Where(x => x.GroupId == CurrentGroupId)
            .Where(x => x.PostId == found.Id)
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var comments = new List<DomainComment>(persistenceComments.Count);
        foreach (var comment in persistenceComments)
        {
            var userVm3 = await GetUserPicturedVmAsync(comment.CreatorId, cancellationToken);
            if (userVm3 is null)
            {
                var userVm4 = await GetUserVmAsync(comment.CreatorId, cancellationToken);
                var temp = UserPicturedVm.Create(userVm4!.UserId, userVm3!.UserName, null);

                var result1 = CommentMapper.Map(comment, temp.Value!);
                if (result1.IsSuccess)
                {
                    comments.Add(result1.Value!);
                }
            }
            else
            {
                var result = CommentMapper.Map(comment, userVm3!);
                if (result.IsSuccess)
                {
                    comments.Add(result.Value!);
                }
            }
        }

        Result<DomainPost> outputResult;

        if (found.CreatorId.HasValue)
        {
            var userVm = await GetUserPicturedVmAsync(found.CreatorId.Value, cancellationToken);
            if (userVm is null)
            {
                var userVm2 = await GetUserVmAsync(found.CreatorId.Value, cancellationToken);
                var temp = UserPicturedVm.Create(userVm2!.UserId, userVm2!.UserName, null);

                outputResult = PostMapper.Map(found, temp.Value!, comments);
            }
            else
            {
                outputResult = PostMapper.Map(found, userVm!, comments);
            }
        }
        else
        {
            outputResult = PostMapper.Map(found, null, comments);
        }

        if (outputResult.IsError)
        {
            return Result<PaginationDetail<DomainPost>>.Error(outputResult.Errors);
        }

        outputResult.Value.SetCommentCountOffset(count - comments.Count);

        return Result<PaginationDetail<DomainPost>>.Success(
            new PaginationDetail<DomainPost>(count, adjusted, outputResult.Value!));
    }

    public async Task<Result<PaginationDetail<ICollection<DomainPost>>>> GetAllAsync(PaginationInfo pagination,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var count = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return Result<PaginationDetail<ICollection<DomainPost>>>.Success(
            new PaginationDetail<ICollection<DomainPost>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainPost>>> GetAllAsync(ShortName labelName,
        SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var label = await Context.Labels
            .Where(x => x.GroupId == CurrentGroupId)
            .FirstOrDefaultAsync(x => x.NameNormalized == labelName.ValueNormalized, cancellationToken);

        if (label is null)
        {
            return Result<ICollection<DomainPost>>.Success(Array.Empty<DomainPost>());
        }

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => x.LabelId != null && x.LabelId == label.Id)
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainPost>>>> GetAllAsync(ShortName labelName,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        var isMember = await IsGroupMemberAsync(cancellationToken);

        var label = await Context.Labels
            .Where(x => x.GroupId == CurrentGroupId)
            .FirstOrDefaultAsync(x => x.NameNormalized == labelName.ValueNormalized, cancellationToken);

        if (label is null)
        {
            return Result<PaginationDetail<ICollection<DomainPost>>>.Success(
                new PaginationDetail<ICollection<DomainPost>>(0, pagination, Array.Empty<DomainPost>()));
        }

        var count = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => x.LabelId != null && x.LabelId == label.Id)
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => x.LabelId != null && x.LabelId == label.Id)
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return Result<PaginationDetail<ICollection<DomainPost>>>.Success(
            new PaginationDetail<ICollection<DomainPost>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainPost>>> FindByTitleAsync(Title title,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainPost>>();
        }

        var isMember = await IsGroupMemberAsync(cancellationToken);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainPost>>>> FindByTitleAsync(Title title,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (title is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainPost>>>();
        }

        var isMember = await IsGroupMemberAsync(cancellationToken);

        var count = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => EF.Functions.ILike(x.TitleNormalized, $"%{title.ValueNormalized}%"))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return Result<PaginationDetail<ICollection<DomainPost>>>.Success(
            new PaginationDetail<ICollection<DomainPost>>(count, adjusted, output));
    }

    public async Task<Result<ICollection<DomainPost>>> SearchAsync(SearchTerm searchTerm,
        SortOrder sortOrder = SortOrder.Ascending, CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<ICollection<DomainPost>>();
        }

        var isMember = await IsGroupMemberAsync(cancellationToken);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return output.ToResultCollection();
    }

    public async Task<Result<PaginationDetail<ICollection<DomainPost>>>> SearchAsync(SearchTerm searchTerm,
        PaginationInfo pagination, SortOrder sortOrder = SortOrder.Ascending,
        CancellationToken cancellationToken = default)
    {
        if (searchTerm is null)
        {
            return GeneralErrors.InputIsNull.ToErrorResult<PaginationDetail<ICollection<DomainPost>>>();
        }

        var isMember = await IsGroupMemberAsync(cancellationToken);

        var count = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .CountAsync(cancellationToken);

        var adjusted = pagination.Adjust(count);

        var found = await GetFiltered()
            .ApplyPublicVisibilityFilter(!isMember)
            .Where(x => x.SearchVectorOne.Matches(searchTerm.ValueNormalized) ||
                        x.SearchVectorTwo.Matches(searchTerm.ValueNormalized))
            .ApplySortOrder(sortOrder)
            .ApplyPagination(adjusted)
            .ToListAsync(cancellationToken);

        var output = new List<DomainPost>(found.Count);
        foreach (var item in found)
        {
            var commentCount = await GetCommentCountAsync(item.Id, cancellationToken);

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
                        if (commentCount.IsSuccess)
                        {
                            c1.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c1.Value!);
                    }
                }
                else
                {
                    var c2 = PostMapper.Map(item, userVm!);
                    if (c2.IsSuccess)
                    {
                        if (commentCount.IsSuccess)
                        {
                            c2.Value.SetCommentCountOffset(commentCount.Value);
                        }

                        output.Add(c2.Value!);
                    }
                }
            }
            else
            {
                var c2 = PostMapper.Map(item, null);
                if (c2.IsSuccess)
                {
                    if (commentCount.IsSuccess)
                    {
                        c2.Value.SetCommentCountOffset(commentCount.Value);
                    }

                    output.Add(c2.Value!);
                }
            }
        }

        return Result<PaginationDetail<ICollection<DomainPost>>>.Success(
            new PaginationDetail<ICollection<DomainPost>>(count, adjusted, output));
    }

    public Result<UserId?> GetCreatorId(PostId id)
    {
        var found = GetFiltered()
            .Where(x => x.Id == id)
            .Select(x => x.CreatorId)
            .FirstOrDefault();

        return found.ToResult();
    }

    public async Task<Result<PostId>> AddIfNotExistsAsync(DomainPost entity,
        CancellationToken cancellationToken = default)
    {
        var exists = await ExistsAsync(entity.Id, cancellationToken);

        if (exists.IsError)
        {
            return Result<PostId>.Error(exists.Errors);
        }

        if (exists.Value)
        {
            return entity.Id.ToResult();
        }

        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        if (entity.Label is not null)
        {
            ILabelRepository labelRepo = new LabelRepository(Context, CurrentGroupId, Logger);
            var existResult = await labelRepo.AddIfNotExistsAsync(entity.Label, cancellationToken);
            if (existResult.IsError)
            {
                return Result<PostId>.Error(existResult.Errors);
            }

            if (entity.Label.Id != existResult.Value)
            {
                var label = await labelRepo.GetByIdAsync(existResult.Value, cancellationToken);
                if (label.IsError)
                {
                    return Result<PostId>.Error(label.Errors);
                }

                entity.ChangeLabel(label.Value, _dateTimeProvider);
            }
        }

        var item = PostMapper.Map(entity, CurrentGroupId);

        await Context.Posts.AddAsync(item, cancellationToken);

        var changeTracker = entity.GetCommentTracker();
        if (!changeTracker.HasChanges)
        {
            return entity.Id.ToResult();
        }

        foreach (var comment in changeTracker.GetItems())
        {
            var newComment = CommentMapper.Map(comment, CurrentGroupId, entity.Id);

            await Context.Comments.AddAsync(newComment, cancellationToken);
        }

        return entity.Id.ToResult();
    }

    public async Task<Result> UpdateAsync(DomainPost entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        if (entity.Label is not null)
        {
            ILabelRepository labelRepo = new LabelRepository(Context, CurrentGroupId, Logger);
            var existResult = await labelRepo.AddIfNotExistsAsync(entity.Label, cancellationToken);
            if (existResult.IsError)
            {
                return Result<PostId>.Error(existResult.Errors);
            }

            if (entity.Label.Id != existResult.Value)
            {
                var label = await labelRepo.GetByIdAsync(existResult.Value, cancellationToken);
                if (label.IsError)
                {
                    return Result<PostId>.Error(label.Errors);
                }

                entity.ChangeLabel(label.Value, _dateTimeProvider);
            }
        }

        var item = PostMapper.Map(entity, CurrentGroupId);

        Context.Posts.Update(item);

        var changeTracker = entity.GetCommentTracker();
        if (!changeTracker.HasChanges)
        {
            return Result.Success();
        }

        await Context.Comments
            .Where(x => x.PostId == item.Id)
            .Where(x => changeTracker.GetRemovedIds().Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);

        foreach (var commentId in changeTracker.GetNewItemsChronologically())
        {
            var comment = changeTracker.GetById(commentId);
            var newComment = CommentMapper.Map(comment!, CurrentGroupId, entity.Id);

            await Context.Comments.AddAsync(newComment, cancellationToken);
        }

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteAsync(PostId id, CancellationToken cancellationToken = default)
    {
        await Context.Posts.Where(x => x.GroupId == CurrentGroupId && x.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(DomainPost entity, CancellationToken cancellationToken = default)
    {
        if (entity.HasDomainEvents)
        {
            Context._domainEventContainer.GatherAndClearEvents(entity);
        }

        Context.Posts.Remove(PostMapper.Map(entity, CurrentGroupId));

        return await Task.FromResult(Result.Success());
    }

    public async Task<Result> DeleteByFriendlyIdAsync(PostFriendlyId friendlyId,
        CancellationToken cancellationToken = default)
    {
        var found = await GetFiltered()
            .Where(x => x.FriendlyId == friendlyId.Value)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (found == default)
        {
            return PostErrors.PostNotFound.ToErrorResult<DomainPost>();
        }

        var item = new PersistencePost()
        {
            Id = found,
            GroupId = CurrentGroupId
        };

        Context.Posts.Remove(item);

        return Result.Success();
    }
}

internal static class PostRepositoryHelper
{
    public static IQueryable<PersistencePost> ApplySortOrder(this IQueryable<PersistencePost> queryable,
        SortOrder sortOrder)
    {
        switch (sortOrder)
        {
            case SortOrder.Ascending:
                return queryable
                    .OrderBy(x => x.CreatedOnUtc);

            case SortOrder.Descending:
                return queryable
                    .OrderByDescending(x => x.CreatedOnUtc);

            default:
                goto case SortOrder.Ascending;
        }
    }

    public static IQueryable<PersistencePost> ApplyPagination(this IQueryable<PersistencePost> queryable,
        PaginationInfo pagination)
    {
        return queryable
            .Skip(pagination.GetSkip())
            .Take(pagination.GetTake());
    }

    public static IQueryable<PersistencePost> ApplyPublicVisibilityFilter(this IQueryable<PersistencePost> queryable,
        bool apply = true)
    {
        if (apply)
        {
            return queryable
                .Where(x => !x.IsMemberOnly);
        }

        return queryable;
    }
}