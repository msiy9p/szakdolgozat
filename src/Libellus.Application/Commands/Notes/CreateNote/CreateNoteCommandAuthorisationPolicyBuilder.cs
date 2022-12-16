using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Notes.CreateNote;

public sealed class
    CreateNoteCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<CreateNoteCommand>
{
    private readonly IReadingReadOnlyRepository _repository;

    public CreateNoteCommandAuthorisationPolicyBuilder(IReadingReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(CreateNoteCommand instance)
    {
        UseRequirement(CurrentUserCanCreateInCurrentGroupRequirement.Instance);

        var result = _repository.GetCreatorId(instance.ReadingId);
        if (result.IsError)
        {
            UseRequirement(new SameCreatorRequirement(result.Value));
        }

        UseRequirement(new SameCreatorRequirement(result.Value));
    }
}