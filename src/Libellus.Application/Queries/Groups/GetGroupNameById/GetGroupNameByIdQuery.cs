using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Application.ViewModels;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Queries.Groups.GetGroupNameById;

[Authorise]
public sealed record GetGroupNameByIdQuery(GroupId GroupId) : IQuery<GroupNameVm>;