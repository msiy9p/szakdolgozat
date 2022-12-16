using Libellus.Application.Commands.WarningTags.CreateWarningTag;
using Libellus.Application.Commands.WarningTags.DeleteWarningTag;
using Libellus.Application.Commands.WarningTags.DeleteWarningTagById;
using Libellus.Application.Commands.WarningTags.UpdateWarningTag;
using Libellus.Application.Commands.WarningTags.UpdateWarningTagById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.WarningTags;

public sealed class WarningTagCommandHandler :
    ICommandHandler<CreateWarningTagCommand, WarningTagId>,
    ICommandHandler<DeleteWarningTagCommand>,
    ICommandHandler<DeleteWarningTagByIdCommand>,
    ICommandHandler<UpdateWarningTagCommand>,
    ICommandHandler<UpdateWarningTagByIdCommand>
{
    private readonly IWarningTagRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public WarningTagCommandHandler(IWarningTagRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WarningTagId>> Handle(CreateWarningTagCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = WarningTagId.Create();
        var item = WarningTag.Create(id, dateTime, dateTime, _currentUserService.UserId, request.Name);

        if (item.IsError)
        {
            return await Task.FromResult(Result<WarningTagId>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<WarningTagId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<WarningTagId>.Error(saveResult.Errors);
        }

        return Result<WarningTagId>.Success(id);
    }

    public async Task<Result> Handle(DeleteWarningTagCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteWarningTagByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.WarningTagId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateWarningTagCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateWarningTagByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.WarningTagId, cancellationToken);
        if (exists.IsError)
        {
            return exists;
        }

        if (exists.Value.Name.Value != request.Name.Value)
        {
            exists.Value.ChangeName(request.Name, _dateTimeProvider);
        }

        var result = await _repository.UpdateAsync(exists.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}