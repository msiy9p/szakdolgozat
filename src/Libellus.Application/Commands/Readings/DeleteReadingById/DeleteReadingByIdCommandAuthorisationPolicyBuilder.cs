using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Models;
using Libellus.Application.Models.Requirements;

namespace Libellus.Application.Commands.Readings.DeleteReadingById;

public sealed class
    DeleteReadingByIdCommandAuthorisationPolicyBuilder : BaseAuthorisationPolicyBuilder<
        DeleteReadingByIdCommand>
{
    private readonly IReadingReadOnlyRepository _repository;

    public DeleteReadingByIdCommandAuthorisationPolicyBuilder(IReadingReadOnlyRepository repository)
    {
        _repository = repository;
    }

    public override void BuildPolicy(DeleteReadingByIdCommand instance)
    {
        var result = _repository.GetCreatorId(instance.ReadingId);
        if (result.IsError)
        {
            UseRequirement(new SameCreatorRequirement(result.Value));
        }

        UseRequirement(new SameCreatorRequirement(result.Value));
    }
}