using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Readings.CreateReading;

public sealed class
    CreateReadingCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateReadingCommand>
{
    public override void BuildPolicy(CreateReadingCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}