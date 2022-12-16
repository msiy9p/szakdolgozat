using Libellus.Application.Common.Interfaces.Messaging;
using MediatR;

namespace Libellus.Application.Utilities;

internal static class Utilities
{
    public static string GetRequestType<TRequest>(TRequest request) where TRequest : IBaseRequest
    {
        if (request is null)
        {
            return string.Empty;
        }

        var genericInterfaces = typeof(TRequest).GetInterfaces().ToArray();
        string requestType;

        if (genericInterfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IQuery<>)))
        {
            requestType = "Query";
        }
        else if (genericInterfaces.Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommand<>)))
        {
            requestType = "Command";
        }
        else if (genericInterfaces.Any(x => !x.IsGenericType && x == typeof(ICommand)))
        {
            requestType = "Command";
        }
        else
        {
            requestType = "Request";
        }

        return requestType;
    }
}