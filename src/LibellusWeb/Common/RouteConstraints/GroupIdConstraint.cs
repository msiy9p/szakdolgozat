using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class GroupIdConstraint : IRouteConstraint
{
    public const string Key = "groupid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return GroupFriendlyId.IsValid((string?)values[routeKey]);
    }
}