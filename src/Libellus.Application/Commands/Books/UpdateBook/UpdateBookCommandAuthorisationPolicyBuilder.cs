using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Books.UpdateBook;

public sealed class
    UpdateBookCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<UpdateBookCommand>
{
    public override void BuildPolicy(UpdateBookCommand instance)
    {
        UseRequirement(new SameCreatorOrAboveRequirement(instance.Item.CreatorId));
    }
}