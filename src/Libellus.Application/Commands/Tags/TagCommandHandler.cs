using Libellus.Application.Commands.Tags.CreateTag;
using Libellus.Application.Commands.Tags.DeleteTag;
using Libellus.Application.Commands.Tags.DeleteTagById;
using Libellus.Application.Commands.Tags.UpdateTag;
using Libellus.Application.Commands.Tags.UpdateTagById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Tags;

public sealed class TagCommandHandler :
    ICommandHandler<CreateTagCommand, TagId>,
    ICommandHandler<DeleteTagCommand>,
    ICommandHandler<DeleteTagByIdCommand>,
    ICommandHandler<UpdateTagCommand>,
    ICommandHandler<UpdateTagByIdCommand>
{
    private readonly ITagRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public TagCommandHandler(ITagRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TagId>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = TagId.Create();
        var item = Tag.Create(id, dateTime, dateTime, _currentUserService.UserId, request.Name);

        if (item.IsError)
        {
            return await Task.FromResult(Result<TagId>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<TagId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<TagId>.Error(saveResult.Errors);
        }

        return Result<TagId>.Success(id);
    }

    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteTagByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.TagId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateTagByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.TagId, cancellationToken);
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