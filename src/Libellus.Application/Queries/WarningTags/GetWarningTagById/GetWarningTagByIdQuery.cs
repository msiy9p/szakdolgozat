using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.WarningTags.GetWarningTagById;

public sealed record GetWarningTagByIdQuery(WarningTagId WarningTagId) : IQuery<WarningTag>;