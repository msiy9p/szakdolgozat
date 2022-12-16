using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class BookEditionIdConstraint : IRouteConstraint
{
    public const string Key = "bookeditionid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return BookEditionFriendlyId.IsValid((string?)values[routeKey]);
    }
}