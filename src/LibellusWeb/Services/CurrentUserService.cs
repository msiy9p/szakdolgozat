using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using System.Security.Claims;

namespace LibellusWeb.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private UserId? _userId = null;
    private bool _run = false;

    public UserId? UserId => GetUserId();

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private UserId? GetUserId()
    {
        if (_run)
        {
            return _userId;
        }

        _run = true;
        var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        if (Guid.TryParse(id, out var guid))
        {
            _userId = Libellus.Domain.Common.Types.Ids.UserId.Convert(guid);
            return _userId;
        }

        return null;
    }
}