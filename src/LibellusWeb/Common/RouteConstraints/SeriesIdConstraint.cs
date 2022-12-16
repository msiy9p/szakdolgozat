using Libellus.Domain.Common.Types.Ids;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class SeriesIdConstraint : IRouteConstraint
{
    public const string Key = "seriesid";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        return SeriesFriendlyId.IsValid((string?)values[routeKey]);
    }
}