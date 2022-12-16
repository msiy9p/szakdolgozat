using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.CoverImages.GetCoverImageByObjectName;

[Authorise]
public sealed record GetCoverImageByObjectNameQuery(string ObjectName) : IQuery<CoverImage>;