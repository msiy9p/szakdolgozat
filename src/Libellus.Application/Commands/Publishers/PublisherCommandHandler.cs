using Libellus.Application.Commands.Publishers.CreatePublisher;
using Libellus.Application.Commands.Publishers.DeletePublisher;
using Libellus.Application.Commands.Publishers.DeletePublisherById;
using Libellus.Application.Commands.Publishers.UpdatePublisher;
using Libellus.Application.Commands.Publishers.UpdatePublisherById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Publishers;

public sealed class PublisherCommandHandler :
    ICommandHandler<CreatePublisherCommand, PublisherId>,
    ICommandHandler<DeletePublisherCommand>,
    ICommandHandler<DeletePublisherByIdCommand>,
    ICommandHandler<UpdatePublisherCommand>,
    ICommandHandler<UpdatePublisherByIdCommand>
{
    private readonly IPublisherRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public PublisherCommandHandler(IPublisherRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PublisherId>> Handle(CreatePublisherCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = PublisherId.Create();
        var item = Publisher.Create(id, dateTime, dateTime, _currentUserService.UserId, request.Name);

        if (item.IsError)
        {
            return await Task.FromResult(Result<PublisherId>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<PublisherId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<PublisherId>.Error(saveResult.Errors);
        }

        return Result<PublisherId>.Success(id);
    }

    public async Task<Result> Handle(DeletePublisherCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeletePublisherByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.PublisherId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdatePublisherCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdatePublisherByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.PublisherId, cancellationToken);
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