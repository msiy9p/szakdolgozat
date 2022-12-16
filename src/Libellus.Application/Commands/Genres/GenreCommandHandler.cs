using Libellus.Application.Commands.Genres.CreateGenre;
using Libellus.Application.Commands.Genres.DeleteGenre;
using Libellus.Application.Commands.Genres.DeleteGenreById;
using Libellus.Application.Commands.Genres.UpdateGenre;
using Libellus.Application.Commands.Genres.UpdateGenreById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Genres;

public sealed class GenreCommandHandler :
    ICommandHandler<CreateGenreCommand, GenreId>,
    ICommandHandler<DeleteGenreCommand>,
    ICommandHandler<DeleteGenreByIdCommand>,
    ICommandHandler<UpdateGenreCommand>,
    ICommandHandler<UpdateGenreByIdCommand>
{
    private readonly IGenreRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public GenreCommandHandler(IGenreRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GenreId>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = GenreId.Create();
        var item = Genre.Create(id, dateTime, dateTime, _currentUserService.UserId, request.Name, request.IsFiction);

        if (item.IsError)
        {
            return Result<GenreId>.Error(item.Errors);
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<GenreId>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<GenreId>.Error(saveResult.Errors);
        }

        return Result<GenreId>.Success(id);
    }

    public async Task<Result> Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteGenreByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.GenreId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateGenreByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.GenreId, cancellationToken);
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