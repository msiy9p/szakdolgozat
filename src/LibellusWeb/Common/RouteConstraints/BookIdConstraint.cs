using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class BookIdConstraint : IRouteConstraint
{
    public const string Key = "bookid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return BookFriendlyId.IsValid((string?)values[routeKey]);
    }
}