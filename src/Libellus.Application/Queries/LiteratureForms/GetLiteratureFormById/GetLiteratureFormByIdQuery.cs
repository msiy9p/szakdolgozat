using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.LiteratureForms.GetLiteratureFormById;

public sealed record GetLiteratureFormByIdQuery(LiteratureFormId LiteratureFormId) : IQuery<LiteratureForm>;