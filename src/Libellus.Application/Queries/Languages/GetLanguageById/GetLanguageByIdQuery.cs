using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Languages.GetLanguageById;

public sealed record GetLanguageByIdQuery(LanguageId LanguageId) : IQuery<Language>;