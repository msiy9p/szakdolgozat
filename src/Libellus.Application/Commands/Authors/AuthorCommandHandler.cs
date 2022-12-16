using Libellus.Application.Commands.Authors.CreateAuthor;
using Libellus.Application.Commands.Authors.DeleteAuthor;
using Libellus.Application.Commands.Authors.DeleteAuthorById;
using Libellus.Application.Commands.Authors.DeleteAuthorCoverImageById;
using Libellus.Application.Commands.Authors.UpdateAuthor;
using Libellus.Application.Commands.Authors.UpdateAuthorById;
using Libellus.Application.Commands.Authors.UpdateAuthorCoverImageById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Commands.Authors;

public sealed class AuthorCommandHandler :
    ICommandHandler<CreateAuthorCommand, AuthorIds>,
    ICommandHandler<DeleteAuthorCommand>,
    ICommandHandler<DeleteAuthorByIdCommand>,
    ICommandHandler<UpdateAuthorCommand>,
    ICommandHandler<UpdateAuthorByIdCommand>,
    ICommandHandler<UpdateAuthorCoverImageByIdCommand>,
    ICommandHandler<DeleteAuthorCoverImageByIdCommand>
{
    private readonly IAuthorRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICoverImageRepository _coverImageRepository;

    public AuthorCommandHandler(IAuthorRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, ICoverImageRepository coverImageRepository)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _coverImageRepository = coverImageRepository;
    }

    public async Task<Result<AuthorIds>> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = AuthorId.Create();
        var fid = AuthorFriendlyId.Create();
        var item = Author.Create(id, dateTime, dateTime, fid, (UserVm?)_currentUserService.UserId,
            request.Name, request.CoverImageMetaDataContainer);

        if (item.IsError)
        {
            return await Task.FromResult(Result<AuthorIds>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<AuthorIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<AuthorIds>.Error(saveResult.Errors);
        }

        return new AuthorIds(id, fid).ToResult();
    }

    public async Task<Result> Handle(DeleteAuthorCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteAuthorByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.AuthorId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateAuthorByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (result.Value.Name.Value != request.Name.Value)
        {
            result.Value.ChangeName(request.Name, _dateTimeProvider);
        }

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateAuthorCoverImageByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        result.Value.ChangeCovers(request.CoverImageMetaDataContainer, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteAuthorCoverImageByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.AuthorId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (result.Value.AvailableCovers is null)
        {
            return Result.Succeeded;
        }

        var deleteResult = await _coverImageRepository
            .DeleteByIdAsync(result.Value.AvailableCovers.Id, cancellationToken);
        if (deleteResult.IsError)
        {
            return deleteResult;
        }

        result.Value.RemoveCovers(_dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}