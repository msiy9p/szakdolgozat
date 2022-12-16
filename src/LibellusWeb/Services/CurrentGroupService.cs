using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Microsoft.AspNetCore.Http.Extensions;

namespace LibellusWeb.Services;

public sealed class CurrentGroupService : ICurrentGroupService
{
    private const string Group = "/Group/";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public GroupId? CurrentGroupId => GetCurrentGroupId();

    public CurrentGroupService(IHttpContextAccessor httpContextAccessor,
        IFriendlyIdLookupRepository friendlyIdLookupRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    private GroupId? GetCurrentGroupId()
    {
        var url = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        return TryGroup(url);
    }

    private GroupId? TryGroup(string url)
    {
        var index = url.IndexOf(Group);
        if (index < 1)
        {
            return null;
        }

        var rawId = url.Substring(index + Group.Length, GroupFriendlyId.Length);

        var friendlyId = GroupFriendlyId.Convert(rawId);
        if (!friendlyId.HasValue)
        {
            return null;
        }

        var id = _friendlyIdLookupRepository.Lookup(friendlyId.Value);
        if (id.IsError)
        {
            return null;
        }

        return id.Value;
    }
}