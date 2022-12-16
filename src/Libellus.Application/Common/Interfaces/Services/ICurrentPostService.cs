using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Application.Common.Interfaces.Services;

public interface ICurrentPostService
{
    PostId? CurrentPostId { get; }
}