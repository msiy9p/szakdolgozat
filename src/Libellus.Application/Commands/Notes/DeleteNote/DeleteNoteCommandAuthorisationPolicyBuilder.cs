using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Notes.DeleteNote;

public sealed class
    DeleteNoteCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteNoteCommand>
{
    public override void BuildPolicy(DeleteNoteCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}