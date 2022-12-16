using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Readings.UpdateReading;

public sealed class
    UpdateReadingCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateReadingCommand>
{
    public override void BuildPolicy(UpdateReadingCommand instance)
    {
        UseRequirement(new SameCreatorRequirement(instance.Item.CreatorId));
    }
}