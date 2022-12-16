using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.GroupMemberships.KickOutMember;

public sealed record KickOutMemberCommand(UserId UserId) : ICommand;