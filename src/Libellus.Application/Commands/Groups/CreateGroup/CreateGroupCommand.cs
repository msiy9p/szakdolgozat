using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Security;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Groups.CreateGroup;

[Authorise]
public sealed record CreateGroupCommand(Name Name, DescriptionText? DescriptionText, bool IsPrivate,
    bool CreateWithDefaults) : ICommand<GroupIds>;