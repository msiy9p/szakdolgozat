using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Genres.DeleteGenre;

public sealed class
    DeleteGenreCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteGenreCommand>
{
    public override void BuildPolicy(DeleteGenreCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}