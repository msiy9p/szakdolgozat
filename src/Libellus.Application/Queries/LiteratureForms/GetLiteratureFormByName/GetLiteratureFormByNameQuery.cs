using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.LiteratureForms.GetLiteratureFormByName;

public sealed record GetLiteratureFormByNameQuery(ShortName Name) : IQuery<LiteratureForm>;