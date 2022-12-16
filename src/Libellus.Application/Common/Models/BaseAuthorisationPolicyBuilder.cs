using Libellus.Application.Common.Interfaces.Security;

namespace Libellus.Application.Common.Models;

public abstract class BaseAuthorisationPolicyBuilder<TRequest> : IAuthorisationPolicyBuilder<TRequest>
{
    private readonly List<IAuthorisationRequirement> _requirements = new();

    public IReadOnlyCollection<IAuthorisationRequirement> Requirements => _requirements;

    protected void UseRequirement(IAuthorisationRequirement requirement)
    {
        _requirements.Add(requirement);
    }

    public abstract void BuildPolicy(TRequest instance);
}