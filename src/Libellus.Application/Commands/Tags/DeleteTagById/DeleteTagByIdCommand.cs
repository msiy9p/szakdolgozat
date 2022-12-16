using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Commands.Tags.DeleteTagById;

public sealed record DeleteTagByIdCommand(TagId TagId) : ICommand;