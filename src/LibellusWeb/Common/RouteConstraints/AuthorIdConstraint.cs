using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class AuthorIdConstraint : IRouteConstraint
{
    public const string Key = "authorid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return AuthorFriendlyId.IsValid((string?)values[routeKey]);
    }
}