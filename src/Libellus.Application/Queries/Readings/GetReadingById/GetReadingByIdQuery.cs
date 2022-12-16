using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Readings.GetReadingById;

public sealed record GetReadingByIdQuery(ReadingId ReadingId) : IQuery<Reading>;