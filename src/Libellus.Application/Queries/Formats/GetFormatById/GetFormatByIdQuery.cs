using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Formats.GetFormatById;

public sealed record GetFormatByIdQuery(FormatId FormatId) : IQuery<Format>;