using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Readings.UpdateReadingById;

public sealed class UpdateReadingByIdCommandAuthorisationPolicyBuilder :
    BaseAuthorisationPolicyBuilder<UpdateReadingByIdCommand>
{
    private readonly IReadingReadOnlyRepository _repository;

    public UpdateReadingByIdCommandAuthorisationPolicyBuilder(IReadingReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(UpdateReadingByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.ReadingId);
        if (result.IsError)
        {
            UseRequirement(new SameCreatorRequirement(result.Value));
        }

        UseRequirement(new SameCreatorRequirement(result.Value));
    }
}