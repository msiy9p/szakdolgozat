using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Common.Interfaces.Services;

public interface ICurrentGroupService
{
    GroupId? CurrentGroupId { get; }
}