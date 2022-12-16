using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.WarningTags.GetWarningTagByName;

public sealed record GetWarningTagByNameQuery(ShortName Name) : IQuery<WarningTag>;