using Libellus.Application.Commands.Series.CreateSeries;
using Libellus.Application.Commands.Series.DeleteSeries;
using Libellus.Application.Commands.Series.DeleteSeriesById;
using Libellus.Application.Commands.Series.UpdateSeries;
using Libellus.Application.Commands.Series.UpdateSeriesById;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ViewModels;

namespace Libellus.Application.Commands.Series;

public sealed class SeriesCommandHandler :
    ICommandHandler<CreateSeriesCommand, SeriesIds>,
    ICommandHandler<DeleteSeriesCommand>,
    ICommandHandler<DeleteSeriesByIdCommand>,
    ICommandHandler<UpdateSeriesCommand>,
    ICommandHandler<UpdateSeriesByIdCommand>
{
    private readonly ISeriesRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public SeriesCommandHandler(ISeriesRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SeriesIds>> Handle(CreateSeriesCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = SeriesId.Create();
        var fid = SeriesFriendlyId.Create();
        var item = Domain.Entities.Series.Create(id, dateTime, dateTime, fid,
            (UserVm?)_currentUserService.UserId, request.Title);

        if (item.IsError)
        {
            return await Task.FromResult(Result<SeriesIds>.Error(item.Errors));
        }

        var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
        if (result.IsError)
        {
            return Result<SeriesIds>.Error(result.Errors);
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<SeriesIds>.Error(saveResult.Errors);
        }

        return new SeriesIds(id, fid).ToResult();
    }

    public async Task<Result> Handle(DeleteSeriesCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteSeriesByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.SeriesId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateSeriesCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateSeriesByIdCommand request, CancellationToken cancellationToken)
    {
        var exists = await _repository.GetByIdAsync(request.SeriesId, cancellationToken);
        if (exists.IsError)
        {
            return exists;
        }

        if (exists.Value.Title.Value != request.Title.Value)
        {
            exists.Value.ChangeTitle(request.Title, _dateTimeProvider);
        }

        var result = await _repository.UpdateAsync(exists.Value, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}