using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class PostIdConstraint : IRouteConstraint
{
    public const string Key = "postid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return PostFriendlyId.IsValid((string?)values[routeKey]);
    }
}