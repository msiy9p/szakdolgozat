using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Labels.GetLabelById;

public sealed record GetLabelByIdQuery(LabelId LabelId) : IQuery<Label>;