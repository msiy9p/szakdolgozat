using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Microsoft.AspNetCore.Http.Extensions;

namespace LibellusWeb.Services;

public sealed class CurrentPostService : ICurrentPostService
{
    private const string Post = "/Post/";

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IFriendlyIdLookupRepository _friendlyIdLookupRepository;

    public PostId? CurrentPostId => GetCurrentPostId();

    public CurrentPostService(IHttpContextAccessor httpContextAccessor,
        IFriendlyIdLookupRepository friendlyIdLookupRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _friendlyIdLookupRepository = friendlyIdLookupRepository;
    }

    private PostId? GetCurrentPostId()
    {
        var url = _httpContextAccessor.HttpContext?.Request?.GetDisplayUrl();
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var index = url.IndexOf(Post);
        if (index < 1)
        {
            return null;
        }

        var rawId = url.Substring(index + Post.Length, GroupFriendlyId.Length);

        var friendlyId = PostFriendlyId.Convert(rawId);
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