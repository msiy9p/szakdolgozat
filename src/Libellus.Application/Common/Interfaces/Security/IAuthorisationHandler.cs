using Libellus.Domain.Models;

namespace Libellus.Application.Common.Interfaces.Security;

public interface IAuthorisationHandler<in TRequirement> where TRequirement : IAuthorisationRequirement
{
    Task<Result> Handle(TRequirement requirement, CancellationToken cancellationToken = default);
}