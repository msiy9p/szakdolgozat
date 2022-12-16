using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class ShelfIdConstraint : IRouteConstraint
{
    public const string Key = "shelfid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return ShelfFriendlyId.IsValid((string?)values[routeKey]);
    }
}