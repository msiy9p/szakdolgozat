using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Comments.CommentExistById;
using Libellus.Application.Queries.Comments.GetAllComments;
using Libellus.Application.Queries.Comments.GetAllCommentsPaginated;
using Libellus.Application.Queries.Comments.GetCommentById;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Comments;

public sealed class CommentsQueryHandler :
    IQueryHandler<GetCommentByIdQuery, Comment>,
    IQueryHandler<GetAllCommentsQuery, ICollection<Comment>>,
    IQueryHandler<GetAllCommentsPaginatedQuery, PaginationDetail<ICollection<Comment>>>,
    IQueryHandler<CommentExistByIdQuery, bool>
{
    private readonly ICommentReadOnlyRepository _repository;

    public CommentsQueryHandler(ICommentReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Comment>> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.CommentId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Comment>>> Handle(GetAllCommentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Comment>>>> Handle(GetAllCommentsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Comment>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<bool>> Handle(CommentExistByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ExistsAsync(request.CommentId, cancellationToken);
    }
}