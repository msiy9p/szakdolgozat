using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Languages.GetLanguageByName;

public sealed record GetLanguageByNameQuery(ShortName Name) : IQuery<Language>;