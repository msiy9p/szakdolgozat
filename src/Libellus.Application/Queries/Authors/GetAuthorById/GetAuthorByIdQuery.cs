using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Authors.GetAuthorById;

public sealed record GetAuthorByIdQuery(AuthorId AuthorId) : IQuery<Author>;