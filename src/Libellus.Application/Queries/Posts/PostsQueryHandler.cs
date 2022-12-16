using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Models;
using Libellus.Application.Queries.Posts.GetAllPosts;
using Libellus.Application.Queries.Posts.GetAllPostsPaginated;
using Libellus.Application.Queries.Posts.GetPostById;
using Libellus.Application.Queries.Posts.GetPostByIdWithComments;
using Libellus.Application.Queries.Posts.GetPostByIdWithCommentsPaginated;
using Libellus.Application.Queries.Posts.GetPostsByTitle;
using Libellus.Application.Queries.Posts.GetPostsByTitlePaginated;
using Libellus.Application.Queries.Posts.PostExistById;
using Libellus.Application.Queries.Posts.SearchPosts;
using Libellus.Application.Queries.Posts.SearchPostsPaginated;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Queries.Posts;

public sealed class PostsQueryHandler :
    IQueryHandler<PostExistByIdQuery, bool>,
    IQueryHandler<GetPostByIdQuery, Post>,
    IQueryHandler<GetPostByIdWithCommentsQuery, Post>,
    IQueryHandler<GetPostByIdWithCommentsPaginatedQuery, PaginationDetail<Post>>,
    IQueryHandler<GetAllPostsQuery, ICollection<Post>>,
    IQueryHandler<GetAllPostsPaginatedQuery, PaginationDetail<ICollection<Post>>>,
    IQueryHandler<GetPostsByTitleQuery, ICollection<Post>>,
    IQueryHandler<GetPostsByTitlePaginatedQuery, PaginationDetail<ICollection<Post>>>,
    IQueryHandler<SearchPostsQuery, ICollection<Post>>,
    IQueryHandler<SearchPostsPaginatedQuery, PaginationDetail<ICollection<Post>>>
{
    private readonly IPostReadOnlyRepository _repository;

    public PostsQueryHandler(IPostReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Post>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdAsync(request.PostId, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Post>>> Handle(GetAllPostsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Post>>> Handle(GetPostsByTitleQuery request,
        CancellationToken cancellationToken)
    {
        return await _repository.FindByTitleAsync(request.Title, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<ICollection<Post>>> Handle(SearchPostsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.SearchAsync(request.SearchTerm, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<Post>> Handle(GetPostByIdWithCommentsQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetByIdWithCommentsAsync(request.PostId, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<Post>>> Handle(GetPostByIdWithCommentsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<Post>>.Error(paginationResult.Errors);
        }

        return await _repository.GetByIdWithCommentsAsync(request.PostId, paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Post>>>> Handle(GetAllPostsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Post>>>.Error(paginationResult.Errors);
        }

        return await _repository.GetAllAsync(paginationResult.Value, request.SortOrder,
            cancellationToken: cancellationToken);
    }

    public async Task<Result<bool>> Handle(PostExistByIdQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ExistsAsync(request.PostId, cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Post>>>> Handle(GetPostsByTitlePaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Post>>>.Error(paginationResult.Errors);
        }

        return await _repository.FindByTitleAsync(request.Title, paginationResult.Value,
            request.SortOrder, cancellationToken: cancellationToken);
    }

    public async Task<Result<PaginationDetail<ICollection<Post>>>> Handle(SearchPostsPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var paginationResult = PaginationInfo.Create(request.PageNumber, request.ItemCount, true);
        if (paginationResult.IsError)
        {
            return Result<PaginationDetail<ICollection<Post>>>.Error(paginationResult.Errors);
        }

        return await _repository.SearchAsync(request.SearchTerm, paginationResult.Value,
            request.SortOrder, cancellationToken: cancellationToken);
    }
}