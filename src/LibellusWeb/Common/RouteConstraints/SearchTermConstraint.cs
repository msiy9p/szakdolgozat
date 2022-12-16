using Libellus.Domain.ValueObjects;

namespace LibellusWeb.Common.RouteConstraints;

public sealed class SearchTermConstraint : IRouteConstraint
{
    public const string Key = "term";

    public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values,
        RouteDirection routeDirection)
    {
        var term = (string?)values[routeKey];

        if (string.IsNullOrWhiteSpace(term))
        {
            return false;
        }

        return term.Length >= 1 && term.Length <= SearchTerm.MaxLength;
    }
}