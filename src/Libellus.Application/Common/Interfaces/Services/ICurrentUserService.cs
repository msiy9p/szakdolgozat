using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Common.Interfaces.Services;

public interface ICurrentUserService
{
    UserId? UserId { get; }
}