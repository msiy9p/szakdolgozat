using FluentValidation;
using Libellus.Domain.Common.Errors;
using Libellus.Domain.Models;
using MediatR;

namespace Libellus.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task
            .WhenAll(_validators
                .Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .Select(x => new Error(x.PropertyName, x.PropertyName))
            .Distinct()
            .ToList();

        if (failures.Any())
        {
            return CreateResult<TResponse>(failures);
        }

        return await next();
    }

    private static TResult CreateResult<TResult>(IEnumerable<Error> errors) where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (Result.Invalid(errors) as TResult)!;
        }

        var result = typeof(Result<>)
            .GetGenericTypeDefinition()
            .MakeGenericType(typeof(TResult).GenericTypeArguments[0])
            .GetMethod(nameof(Result.Invalid))!
            .Invoke(null, new object?[] { errors })!;

        return (TResult)result;
    }
}