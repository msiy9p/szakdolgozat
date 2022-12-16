using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Entities;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Queries.Formats.GetFormatByName;

public sealed record GetFormatByNameQuery(ShortName Name) : IQuery<Format>;