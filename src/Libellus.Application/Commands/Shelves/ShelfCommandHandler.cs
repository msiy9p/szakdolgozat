using Libellus.Application.Commands.Shelves.AddBookToShelfById;
using Libellus.Application.Commands.Shelves.CreateShelf;
using Libellus.Application.Commands.Shelves.DeleteShelf;
using Libellus.Application.Commands.Shelves.DeleteShelfById;
using Libellus.Application.Commands.Shelves.RemoveBookFromShelfById;
using Libellus.Application.Commands.Shelves.UpdateShelf;
using Libellus.Application.Commands.Shelves.UpdateShelfById;
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

namespace Libellus.Application.Commands.Shelves;

public sealed class ShelfCommandHandler :
    ICommandHandler<CreateShelfCommand, ShelfIds>,
    ICommandHandler<DeleteShelfCommand>,
    ICommandHandler<DeleteShelfByIdCommand>,
    ICommandHandler<UpdateShelfCommand>,
    ICommandHandler<UpdateShelfByIdCommand>,
    ICommandHandler<AddBookToShelfByIdCommand>,
    ICommandHandler<RemoveBookFromShelfByIdCommand>
{
    private readonly IShelfRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBookReadOnlyRepository _bookReadOnlyRepository;

    public ShelfCommandHandler(IShelfRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork, IBookReadOnlyRepository bookReadOnlyRepository)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
        _bookReadOnlyRepository = bookReadOnlyRepository;
    }

    public async Task<Result<ShelfIds>> Handle(CreateShelfCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = ShelfId.Create();
        var fid = ShelfFriendlyId.Create();
        var item = Shelf.Create(id, dateTime, dateTime, fid, (UserVm?)_currentUserService.UserId,
            request.Name, request.DescriptionText, request.IsLocked);

        if (item.IsError)
        {
            return await Task.FromResult(Result<ShelfIds>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<ShelfIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<ShelfIds>.Error(saveResult.Errors);
        }

        return new ShelfIds(id, fid).ToResult();
    }

    public async Task<Result> Handle(DeleteShelfCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteShelfByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.ShelfId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateShelfCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateShelfByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.ShelfId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (request.DescriptionText is not null)
        {
            result.Value.ChangeDescription(request.DescriptionText, _dateTimeProvider);
        }
        else
        {
            result.Value.RemoveDescription(_dateTimeProvider);
        }

        result.Value.ChangeLockStatus(request.IsLocked, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return updateResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(AddBookToShelfByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.ShelfId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var bookResult = await _bookReadOnlyRepository.GetByIdAsync(request.BookId, cancellationToken);
        if (bookResult.IsError)
        {
            return bookResult;
        }

        result.Value.AddBook(bookResult.Value, _dateTimeProvider);

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return updateResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(RemoveBookFromShelfByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetByIdAsync(request.ShelfId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        var bookResult = await _bookReadOnlyRepository.ExistsAsync(request.BookId, cancellationToken);
        if (bookResult.IsError)
        {
            return bookResult;
        }

        if (bookResult.Value)
        {
            result.Value.RemoveBookById(request.BookId, _dateTimeProvider);
        }

        var updateResult = await _repository.UpdateAsync(result.Value, cancellationToken);
        if (updateResult.IsError)
        {
            return updateResult;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}