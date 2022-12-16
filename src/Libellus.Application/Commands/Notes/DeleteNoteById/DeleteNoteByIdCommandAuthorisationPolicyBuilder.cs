using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Notes.DeleteNoteById;

public sealed class
    DeleteNoteByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<DeleteNoteByIdCommand>
{
    private readonly INoteReadOnlyRepository _repository;

    public DeleteNoteByIdCommandAuthorisationPolicyBuilder(INoteReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(DeleteNoteByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.NoteId);
        if (result.IsError)
        {
            UseRequirement(new SameCreatorOrAboveRequirement(result.Value));
        }

        UseRequirement(new SameCreatorOrAboveRequirement(result.Value));
    }
}