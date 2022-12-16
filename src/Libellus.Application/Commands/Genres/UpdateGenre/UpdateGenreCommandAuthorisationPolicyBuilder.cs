using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Genres.UpdateGenre;

public sealed class
    UpdateGenreCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateGenreCommand>
{
    public override void BuildPolicy(UpdateGenreCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}