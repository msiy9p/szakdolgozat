using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Readings.DeleteReading;

public sealed class
    DeleteReadingCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteReadingCommand>
{
    public override void BuildPolicy(DeleteReadingCommand instance)
    {
        UseRequirement(new SameCreatorRequirement(instance.Item.CreatorId));
    }
}