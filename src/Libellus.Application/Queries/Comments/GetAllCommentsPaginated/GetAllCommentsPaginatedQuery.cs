using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Application.Models;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Comments.GetAllCommentsPaginated;

public sealed record GetAllCommentsPaginatedQuery
    (int PageNumber, int ItemCount, SortOrder SortOrder) : IQuery<PaginationDetail<ICollection<Comment>>>;