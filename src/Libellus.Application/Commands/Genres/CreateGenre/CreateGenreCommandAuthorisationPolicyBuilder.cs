using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Genres.CreateGenre;

public sealed class
    CreateGenreCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateGenreCommand>
{
    public override void BuildPolicy(CreateGenreCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);
    }
}