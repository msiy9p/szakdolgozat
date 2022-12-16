namespace Libellus.Application.Common.Interfaces.Security;

public interface IAuthorisationPolicyBuilder<in T>
{
    IReadOnlyCollection<IAuthorisationRequirement> Requirements { get; }
    void BuildPolicy(T instance);
}