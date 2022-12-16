using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class ReadingIdConstraint : IRouteConstraint
{
    public const string Key = "readingid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return ReadingFriendlyId.IsValid((string?)values[routeKey]);
    }
}