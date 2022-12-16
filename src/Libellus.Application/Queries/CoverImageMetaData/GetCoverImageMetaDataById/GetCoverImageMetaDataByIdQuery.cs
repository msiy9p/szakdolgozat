using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.CoverImageMetaData.GetCoverImageMetaDataById;

[Authorise]
public sealed record GetCoverImageMetaDataByIdQuery(CoverImageId CoverImageId) : IQuery<CoverImageMetaDataContainer>;