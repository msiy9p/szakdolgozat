using Libellus.Application.Commands.Labels.CreateLabel;
using Libellus.Application.Commands.Labels.DeleteLabel;
using Libellus.Application.Commands.Labels.DeleteLabelById;
using Libellus.Application.Commands.Labels.UpdateLabel;
using Libellus.Application.Commands.Labels.UpdateLabelById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Labels;

public sealed class LabelCommandHandler :
    ICommandHandler<CreateLabelCommand, LabelId>,
    ICommandHandler<DeleteLabelCommand>,
    ICommandHandler<DeleteLabelByIdCommand>,
    ICommandHandler<UpdateLabelCommand>,
    ICommandHandler<UpdateLabelByIdCommand>
{
    private readonly ILabelRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LabelCommandHandler(ILabelRepository repository, IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LabelId>> Handle(CreateLabelCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = LabelId.Create();
        var item = Label.Create(id, dateTime, dateTime, request.Name);

        if (item.IsError)
        {
            return Result<LabelId>.Error(item.Errors);
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<LabelId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<LabelId>.Error(saveResult.Errors);
        }

        return Result<LabelId>.Success(id);
    }

    public async Task<Result> Handle(DeleteLabelCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteLabelByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.LabelId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateLabelCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateLabelByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.LabelId, cancellationToken);
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