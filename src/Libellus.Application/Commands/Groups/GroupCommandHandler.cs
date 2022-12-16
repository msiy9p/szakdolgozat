using Libellus.Application.Commands.Groups.CreateGroup;
using Libellus.Application.Commands.Groups.DeleteGroup;
using Libellus.Application.Commands.Groups.DeleteGroupById;
using Libellus.Application.Commands.Groups.UpdateGroup;
using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Common.Interfaces.Persistence;
using Libellus.Application.Common.Interfaces.Persistence.Repositories;
using Libellus.Application.Common.Interfaces.Services;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;

namespace Libellus.Application.Commands.Groups;

public sealed class GroupCommandHandler :
    ICommandHandler<CreateGroupCommand, GroupIds>,
    ICommandHandler<DeleteGroupCommand>,
    ICommandHandler<DeleteGroupByIdCommand>,
    ICommandHandler<UpdateGroupCommand>
{
    private readonly IGroupRepository _repository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public GroupCommandHandler(IGroupRepository repository, ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GroupIds>> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.UtcNow;
        var id = GroupId.Create();
        var fid = GroupFriendlyId.Create();
        var item = Group.Create(id, dateTime, dateTime, fid,
            request.Name, request.DescriptionText, request.IsPrivate);

        if (item.IsError)
        {
            return await Task.FromResult(Result<GroupIds>.Error(item.Errors));
        }

        if (request.CreateWithDefaults)
        {
            var result = await _repository.AddWithDefaultIfNotExistsAsync(item.Value!, cancellationToken);
            if (result.IsError)
            {
                return Result<GroupIds>.Error(result.Errors);
            }
        }
        else
        {
            var result = await _repository.AddIfNotExistsAsync(item.Value!, cancellationToken);
            if (result.IsError)
            {
                return Result<GroupIds>.Error(result.Errors);
            }
        }

        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saveResult.IsError)
        {
            return Result<GroupIds>.Error(saveResult.Errors);
        }

        return Result<GroupIds>.Success(new GroupIds(id, fid));
    }

    public async Task<Result> Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(DeleteGroupByIdCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteAsync(request.GroupId, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Result> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.UpdateAsync(request.Item, cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        return await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}