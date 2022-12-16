using Libellus.Application.Commands.LiteratureForms.CreateLiteratureForm;
using Libellus.Application.Commands.LiteratureForms.DeleteLiteratureForm;
using Libellus.Application.Commands.LiteratureForms.DeleteLiteratureFormById;
using Libellus.Application.Commands.LiteratureForms.UpdateLiteratureForm;
using Libellus.Application.Commands.LiteratureForms.UpdateLiteratureFormById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.LiteratureForms;

public sealed class LiteratureFormCommandHandler :
    ICommandHandler<CreateLiteratureFormCommand, LiteratureFormId>,
    ICommandHandler<DeleteLiteratureFormCommand>,
    ICommandHandler<DeleteLiteratureFormByIdCommand>,
    ICommandHandler<UpdateLiteratureFormCommand>,
    ICommandHandler<UpdateLiteratureFormByIdCommand>
{
    private readonly ILiteratureFormRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LiteratureFormCommandHandler(ILiteratureFormRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LiteratureFormId>> Handle(CreateLiteratureFormCommand request,
        CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = LiteratureFormId.Create();
        var item = LiteratureForm.Create(id, dateTime, dateTime, _currentUserService.UserId, request.Name,
            request.ScoreMultiplier);

        if (item.IsError)
        {
            return Result<LiteratureFormId>.Error(item.Errors);
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<LiteratureFormId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<LiteratureFormId>.Error(saveResult.Errors);
        }

        return Result<LiteratureFormId>.Success(id);
    }

    public async Task<Result> Handle(DeleteLiteratureFormCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteLiteratureFormByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.LiteratureFormId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateLiteratureFormCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateLiteratureFormByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.LiteratureFormId, cancellationToken);
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