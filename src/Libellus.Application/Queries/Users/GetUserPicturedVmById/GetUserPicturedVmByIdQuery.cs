using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Queries.Users.GetUserPicturedVmById;

[Authorise]
public sealed record GetUserPicturedVmByIdQuery(UserId UserId) : IQuery<UserPicturedVm>;