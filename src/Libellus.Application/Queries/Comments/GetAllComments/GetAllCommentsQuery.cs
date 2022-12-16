using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Comments.GetAllComments;

public sealed record GetAllCommentsQuery(SortOrder SortOrder) : IQuery<ICollection<Comment>>
{
    public static readonly GetAllCommentsQuery DefaultInstance = new(SortOrder.Ascending);
}