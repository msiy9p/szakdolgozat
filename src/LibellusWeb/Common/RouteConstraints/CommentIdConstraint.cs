using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class CommentIdConstraint : IRouteConstraint
{
    public const string Key = "commentid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return CommentFriendlyId.IsValid((string?)values[routeKey]);
    }
}