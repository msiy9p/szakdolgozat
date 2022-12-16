using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Users.GetTwoFactorSummaryById;

public sealed record GetTwoFactorSummaryByIdQuery(UserId UserId) : IQuery<TwoFactorSummaryVm>;