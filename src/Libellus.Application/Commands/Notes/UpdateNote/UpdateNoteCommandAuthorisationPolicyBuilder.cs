using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Notes.UpdateNote;

public sealed class
    UpdateNoteCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateNoteCommand>
{
    public override void BuildPolicy(UpdateNoteCommand instance)
    {
        UseRequirement(new SameCreatorRequirement(instance.Item.CreatorId));
    }
}