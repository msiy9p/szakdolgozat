using System.Collections.Concurrent;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Libellus.Application.Common.Interfaces.Security;
using Libellus.Application.Common.Security;
using Libellus.Domain.Models;

namespace Libellus.Application.Behaviors;

public sealed class AuthorisationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ConcurrentDictionary<Type, Type> _requirementHandlers = new();
    private static readonly ConcurrentDictionary<Type, MethodInfo> _handlerMethodInfo = new();

    private readonly IEnumerable<IAuthorisationPolicyBuilder<TRequest>> _authorisationPolicyBuilders;
    private readonly IServiceProvider _serviceProvider;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICurrentGroupService _currentGroupService;
    private readonly IIdentityService _identityService;
    private readonly ILogger<AuthorisationBehavior<TRequest, TResponse>> _logger;

    public AuthorisationBehavior(IEnumerable<IAuthorisationPolicyBuilder<TRequest>> authorisationPolicyBuilders,
        IServiceProvider serviceProvider,
        ICurrentUserService currentUserService, ICurrentGroupService currentGroupService,
        IIdentityService identityService, ILogger<AuthorisationBehavior<TRequest, TResponse>> logger)
    {
        _authorisationPolicyBuilders = authorisationPolicyBuilders;
        _serviceProvider = serviceProvider;
        _currentUserService = currentUserService;
        _currentGroupService = currentGroupService;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        await StandardAuthorisationAsync(request, cancellationToken);

        await RequirementAuthorisationAsync(request, cancellationToken);

        return await next();
    }

    private async Task StandardAuthorisationAsync(TRequest request, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType()
            .GetCustomAttributes<AuthoriseAttribute>().ToList();

        if (!authorizeAttributes.Any())
        {
            return;
        }

        var groupId = _currentGroupService.CurrentGroupId;

        if (_currentUserService.UserId is null)
        {
            _logger.LogError("Forbidden access for {Name}: {UserId} with {GroupId}.",
                typeof(TRequest).Name, "NO_USER_ID",
                groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID");

            throw new ForbiddenAccessException();
        }

        // Role-based authorization
        var authorizeAttributesWithRoles = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .ToList();

        var continueAuthorization = true;

        if (continueAuthorization && authorizeAttributesWithRoles.Any())
        {
            var authorized = false;

            foreach (var role in authorizeAttributesWithRoles
                         .SelectMany(a => a.Roles.Split(',',
                             StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                         .Distinct())
            {
                var isInRole = await _identityService.IsInRoleAsync(
                    _currentUserService.UserId.Value, role.Trim(), cancellationToken);

                if (isInRole.IsSuccess && isInRole.Value)
                {
                    authorized = true;

                    if (role == SecurityConstants.IdentityRoles.Administrator)
                    {
                        _logger.LogInformation("Administrator access, {UserId}.",
                            _currentUserService.UserId.Value.ToString());

                        continueAuthorization = false;
                    }

                    break;
                }
            }

            // Must be a member of at least one role in roles
            if (!authorized)
            {
                _logger.LogError("Forbidden access for {Name}: {UserId} with {GroupId}.",
                    typeof(TRequest).Name, _currentUserService.UserId.Value.ToString(),
                    groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID");

                throw new ForbiddenAccessException();
            }
        }

        // Policy-based authorization
        var authorizeAttributesWithPolicies = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
            .ToList();

        if (continueAuthorization && authorizeAttributesWithPolicies.Any())
        {
            foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
            {
                var authorized =
                    await _identityService.AuthorizeWithPolicyAsync(_currentUserService.UserId.Value, policy,
                        cancellationToken);

                if (authorized.IsError || !authorized.Value)
                {
                    _logger.LogError("Forbidden access for {Name}: {UserId} with {GroupId}.",
                        typeof(TRequest).Name, _currentUserService.UserId.Value.ToString(),
                        groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID");

                    throw new ForbiddenAccessException();
                }
            }
        }

        // GroupRole-based authorization
        var authorizeAttributesWithGroupRoles = authorizeAttributes
            .Where(a => !string.IsNullOrWhiteSpace(a.GroupRoles))
            .ToList();

        if (continueAuthorization && authorizeAttributesWithGroupRoles.Any())
        {
            var authorized = false;

            if (_currentGroupService.CurrentGroupId is null)
            {
                _logger.LogError("Forbidden access for {Name}: {UserId} with {GroupId}.",
                    typeof(TRequest).Name, groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID",
                    groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID");

                throw new ForbiddenAccessException();
            }

            foreach (var groupRole in authorizeAttributesWithGroupRoles
                         .SelectMany(a => a.GroupRoles.Split(',',
                             StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                         .Distinct())
            {
                var isInRole = await _identityService.IsInGroupRoleAsync(_currentUserService.UserId.Value,
                    _currentGroupService.CurrentGroupId.Value, groupRole.Trim(), cancellationToken);

                if (isInRole.IsSuccess && isInRole.Value)
                {
                    authorized = true;
                    break;
                }
            }

            // Must be a member of at least one role in roles
            if (!authorized)
            {
                _logger.LogError("Forbidden access for {Name}: {UserId} with {GroupId}.",
                    typeof(TRequest).Name, _currentUserService.UserId.Value.ToString(),
                    groupId.HasValue ? groupId.Value.ToString() : "NO_GROUP_ID");

                throw new ForbiddenAccessException();
            }
        }
    }

    private async Task RequirementAuthorisationAsync(TRequest request, CancellationToken cancellationToken)
    {
        var requirements = new List<IAuthorisationRequirement>();

        foreach (var builder in _authorisationPolicyBuilders)
        {
            builder.BuildPolicy(request);
            requirements.AddRange(builder.Requirements);
        }

        foreach (var requirement in requirements)
        {
            var result = await ExecuteAuthorizationHandler(requirement, cancellationToken);

            if (result.IsError)
            {
                if (result.Errors.Count == 1)
                {
                    throw new ForbiddenAccessException(result.Errors.FirstOrDefault()?.Message);
                }

                throw new ForbiddenAccessException(result.Errors.Select(x => x.Message));
            }
        }
    }

    private Task<Result> ExecuteAuthorizationHandler(IAuthorisationRequirement requirement,
        CancellationToken cancellationToken)
    {
        var requirementType = requirement.GetType();
        var handlerType = FindHandlerType(requirement);

        if (handlerType is null)
        {
            throw new InvalidOperationException(
                $"Could not find an authorization handler type for requirement type \"{requirementType.Name}\"");
        }

        var temp = _serviceProvider.GetService(typeof(IEnumerable<>)
            .MakeGenericType(handlerType)) as IEnumerable<object>;

        if (temp is null)
        {
            throw new InvalidOperationException(
                $"Could not find an authorization handler implementation for requirement type \"{requirementType.Name}\"");
        }

        var handlers = temp.ToList();

        if (!handlers.Any())
        {
            throw new InvalidOperationException(
                $"Could not find an authorization handler implementation for requirement type \"{requirementType.Name}\"");
        }

        if (handlers.Count > 1)
        {
            throw new InvalidOperationException(
                $"Multiple authorization handler implementations were found for requirement type \"{requirementType.Name}\"");
        }

        var serviceHandler = handlers.First();
        var serviceHandlerType = serviceHandler.GetType();

        var methodInfo = _handlerMethodInfo.GetOrAdd(serviceHandlerType,
            handlerMethodKey =>
            {
                return serviceHandlerType.GetMethods()
                    .First(x => x.Name == nameof(IAuthorisationHandler<IAuthorisationRequirement>.Handle));
            });

        return ((Task<Result>)methodInfo.Invoke(serviceHandler,
            new object[] { requirement, cancellationToken })!)!;
    }

    private static Type? FindHandlerType(IAuthorisationRequirement requirement)
    {
        var requirementType = requirement.GetType();
        var handlerType = _requirementHandlers.GetOrAdd(requirementType,
            requirementTypeKey => typeof(IAuthorisationHandler<>).MakeGenericType(requirementTypeKey));

        return handlerType;
    }
}