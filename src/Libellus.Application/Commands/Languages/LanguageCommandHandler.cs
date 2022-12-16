using Libellus.Application.Commands.Languages.CreateLanguage;
using Libellus.Application.Commands.Languages.DeleteLanguage;
using Libellus.Application.Commands.Languages.DeleteLanguageById;
using Libellus.Application.Commands.Languages.UpdateLanguage;
using Libellus.Application.Commands.Languages.UpdateLanguageById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Languages;

public sealed class LanguageCommandHandler :
    ICommandHandler<CreateLanguageCommand, LanguageId>,
    ICommandHandler<DeleteLanguageCommand>,
    ICommandHandler<DeleteLanguageByIdCommand>,
    ICommandHandler<UpdateLanguageCommand>,
    ICommandHandler<UpdateLanguageByIdCommand>
{
    private readonly ILanguageRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LanguageCommandHandler(ILanguageRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LanguageId>> Handle(CreateLanguageCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = LanguageId.Create();
        var item = Language.Create(id, dateTime, dateTime, _currentUserService.UserId, request.Name);

        if (item.IsError)
        {
            return Result<LanguageId>.Error(item.Errors);
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<LanguageId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<LanguageId>.Error(saveResult.Errors);
        }

        return Result<LanguageId>.Success(id);
    }

    public async Task<Result> Handle(DeleteLanguageCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteLanguageByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.LanguageId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateLanguageByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.LanguageId, cancellationToken);
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