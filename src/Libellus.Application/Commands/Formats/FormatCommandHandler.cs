using Libellus.Application.Commands.Formats.CreateFormat;
using Libellus.Application.Commands.Formats.DeleteFormat;
using Libellus.Application.Commands.Formats.DeleteFormatById;
using Libellus.Application.Commands.Formats.UpdateFormat;
using Libellus.Application.Commands.Formats.UpdateFormatById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Formats;

public sealed class FormatCommandHandler :
    ICommandHandler<CreateFormatCommand, FormatId>,
    ICommandHandler<DeleteFormatCommand>,
    ICommandHandler<DeleteFormatByIdCommand>,
    ICommandHandler<UpdateFormatCommand>,
    ICommandHandler<UpdateFormatByIdCommand>
{
    private readonly IFormatRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public FormatCommandHandler(IFormatRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<FormatId>> Handle(CreateFormatCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = FormatId.Create();
        var item = Format.Create(id, dateTime, dateTime, _currentUserService.UserId, request.Name, request.IsDigital);

        if (item.IsError)
        {
            return await Task.FromResult(Result<FormatId>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<FormatId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<FormatId>.Error(saveResult.Errors);
        }

        return Result<FormatId>.Success(id);
    }

    public async Task<Result> Handle(DeleteFormatCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteFormatByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.FormatId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateFormatCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateFormatByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.FormatId, cancellationToken);
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