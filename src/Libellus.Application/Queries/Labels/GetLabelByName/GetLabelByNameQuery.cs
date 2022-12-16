using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Labels.GetLabelByName;

public sealed record GetLabelByNameQuery(ShortName Name) : IQuery<Label>;