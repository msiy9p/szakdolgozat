using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Genres.DeleteGenreById;

public sealed class
    DeleteGenreByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteGenreByIdCommand>
{
    public override void BuildPolicy(DeleteGenreByIdCommand instance)
    {
        UseRequirement(CurrentUserAtLeastGroupModeratorRequirement.Instance);
    }
}