using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Enums;
using Libellus.Domain.Errors;
using Libellus.Domain.Events;
using Libellus.Domain.Models;
using Libellus.Domain.Utilities;
using Libellus.Domain.ValueObjects;
using NodaTime;

namespace Libellus.Domain.Entities;

public sealed class Group : BaseStampedEntity<GroupId>, IComparable, IComparable<Group>, IEquatable<Group>
{
    public const bool IsPrivateDefault = true;

    private readonly EntityTrackingContainer<Shelf, ShelfId> _shelvesTracked = new();
    private readonly EntityTrackingContainer<Label, LabelId> _labelsTracked = new();
    private readonly EntityTrackingContainer<Post, PostId> _postsTracked = new();
    private readonly EntityTrackingContainer<Invitation, InvitationId> _invitationsTracked = new();

    public GroupFriendlyId FriendlyId { get; init; }

    public Name Name { get; init; }
    public DescriptionText? Description { get; private set; }
    public bool IsPrivate { get; private set; }

    public int ShelfCount => _shelvesTracked.Count;
    public int LabelCount => _labelsTracked.Count;
    public int PostCount => _postsTracked.Count;
    public int InvitationCount => _invitationsTracked.Count;

    public IReadOnlyCollection<Shelf> Shelves => _shelvesTracked.GetItems();
    public IReadOnlyCollection<Label> Labels => _labelsTracked.GetItems();
    public IReadOnlyCollection<Post> Posts => _postsTracked.GetItems();
    public IReadOnlyCollection<Invitation> Invitations => _invitationsTracked.GetItems();

    internal Group(GroupId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, GroupFriendlyId friendlyId,
        Name name, DescriptionText? description, bool isPrivate) : base(id, createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        Name = Guard.Against.Null(name);
        Description = description;
        IsPrivate = isPrivate;
    }

    public static Result<Group> Create(GroupId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        GroupFriendlyId friendlyId, Name name, DescriptionText? description, bool isPrivate)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Group>.Invalid(result.Errors);
        }

        if (name is null)
        {
            return DomainErrors.GeneralErrors.InputIsNull.ToErrorResult<Group>();
        }

        return Result<Group>.Success(new Group(id, createdOnUtc, modifiedOnUtc, friendlyId, name, description,
            isPrivate));
    }

    public static Result<Group> Create(GroupId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        GroupFriendlyId friendlyId, Name name, DescriptionText? description, bool isPrivate,
        IEnumerable<Shelf> shelves)
    {
        if (shelves is null)
        {
            return Result<Group>.Invalid(DomainErrors.GroupErrors.ProvidedShelvesNull);
        }

        var result = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, name, description, isPrivate);
        if (result.IsError)
        {
            return Result<Group>.Invalid(result.Errors);
        }

        var group = new Group(id, createdOnUtc, modifiedOnUtc, friendlyId, name, description, isPrivate);

        foreach (var entity in shelves)
        {
            if (group._shelvesTracked.FirstOrDefault(x => x.Name == entity.Name) is null)
            {
                group._shelvesTracked.Add(entity);
            }
        }

        group._shelvesTracked.ClearChanges();

        return Result<Group>.Success(group);
    }

    public static Result<Group> Create(GroupId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        GroupFriendlyId friendlyId, Name name, DescriptionText? description, bool isPrivate,
        IEnumerable<Label> labels)
    {
        if (labels is null)
        {
            return Result<Group>.Error(DomainErrors.GroupErrors.ProvidedLabelsNull);
        }

        var result = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, name, description, isPrivate);
        if (result.IsError)
        {
            return Result<Group>.Error(result.Errors);
        }

        var group = new Group(id, createdOnUtc, modifiedOnUtc, friendlyId, name, description, isPrivate);

        foreach (var label in labels)
        {
            if (group._labelsTracked.FirstOrDefault(x => x.Name == label.Name) is null)
            {
                group._labelsTracked.Add(label);
            }
        }

        group._labelsTracked.ClearChanges();

        return Result<Group>.Success(group);
    }

    public static Result<Group> Create(GroupId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        GroupFriendlyId friendlyId, Name name, DescriptionText? description, bool isPrivate,
        IEnumerable<Shelf> shelves, IEnumerable<Label> labels, IEnumerable<Post> posts)
    {
        if (shelves is null)
        {
            return Result<Group>.Error(DomainErrors.GroupErrors.ProvidedShelvesNull);
        }

        if (labels is null)
        {
            return Result<Group>.Error(DomainErrors.GroupErrors.ProvidedLabelsNull);
        }

        if (posts is null)
        {
            return Result<Group>.Error(DomainErrors.GroupErrors.ProvidedPostsNull);
        }

        var result = Create(id, createdOnUtc, modifiedOnUtc, friendlyId, name, description, isPrivate, shelves);
        if (result.IsError)
        {
            return Result<Group>.Error(result.Errors);
        }

        var group = new Group(id, createdOnUtc, modifiedOnUtc, friendlyId, name, description, isPrivate);

        foreach (var label in labels)
        {
            if (group._labelsTracked.FirstOrDefault(x => x.Name == label.Name) is null)
            {
                group._labelsTracked.Add(label);
            }
        }

        group._labelsTracked.ClearChanges();

        foreach (var post in posts)
        {
            group._postsTracked.Add(post);
        }

        group._postsTracked.ClearChanges();

        return Result<Group>.Success(group);
    }

    public IReadOnlyEntityTrackingContainer<Shelf, ShelfId> GetShelfTracker() => _shelvesTracked;

    public IReadOnlyEntityTrackingContainer<Label, LabelId> GetLabelTracker() => _labelsTracked;

    public IReadOnlyEntityTrackingContainer<Post, PostId> GetPostTracker() => _postsTracked;

    public IReadOnlyEntityTrackingContainer<Invitation, InvitationId> GetInvitationTracker() => _invitationsTracked;

    public bool MarkAsPrivate(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (IsPrivate)
        {
            return false;
        }

        IsPrivate = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsPublic(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsPrivate)
        {
            return false;
        }

        IsPrivate = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeVisibility(bool value, IDateTimeProvider dateTimeProvider)
        => value ? MarkAsPrivate(dateTimeProvider) : MarkAsPublic(dateTimeProvider);

    public bool RemoveDescription(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Description = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeDescription(string description, IDateTimeProvider dateTimeProvider)
    {
        if (!DescriptionText.IsValidDescriptionText(description))
        {
            return false;
        }

        return ChangeDescription(new DescriptionText(description), dateTimeProvider);
    }

    public bool ChangeDescription(DescriptionText description, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (description is null)
        {
            return false;
        }

        Description = description;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool AddShelf(Shelf shelf, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (shelf is null)
        {
            return false;
        }

        if (_shelvesTracked.FirstOrDefault(x => x.Name == shelf.Name) is not null)
        {
            return false;
        }

        if (!_shelvesTracked.Add(shelf))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveShelf(Shelf shelf, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (shelf is null)
        {
            return false;
        }

        return RemoveShelfById(shelf.Id, dateTimeProvider);
    }

    public bool RemoveShelfById(ShelfId shelfId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_shelvesTracked.Remove(shelfId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Shelf? FindShelfById(ShelfId shelfId)
    {
        if (!_shelvesTracked.Contains(shelfId))
        {
            return null;
        }

        return _shelvesTracked.Find(x => x.Id == shelfId);
    }

    public Shelf? FindShelfByName(string name)
    {
        if (!Name.IsValidName(name))
        {
            return null;
        }

        return FindShelfByTitle(new Name(name));
    }

    public Shelf? FindShelfByTitle(Name name)
    {
        if (name is null)
        {
            return null;
        }

        return _shelvesTracked.FirstOrDefault(x => x.Name == name);
    }

    public bool AddLabel(Label label, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (label is null)
        {
            return false;
        }

        if (_labelsTracked.FirstOrDefault(x => x.Name == label.Name) is not null)
        {
            return false;
        }

        if (!_labelsTracked.Add(label))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveLabel(Label label, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (label is null)
        {
            return false;
        }

        return RemoveLabelById(label.Id, dateTimeProvider);
    }

    public bool RemoveLabelById(LabelId labelId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_labelsTracked.Remove(labelId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Label? FindLabelByName(string name)
    {
        if (!ShortName.IsValidShortName(name))
        {
            return null;
        }

        return FindLabelByName(new ShortName(name));
    }

    public Label? FindLabelByName(ShortName name)
    {
        if (name is null)
        {
            return null;
        }

        return _labelsTracked.FirstOrDefault(x => x.Name == name);
    }

    public bool AddPost(Post post, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_postsTracked.Add(post))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemovePost(Post post, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (post is null)
        {
            return false;
        }

        return RemovePostById(post.Id, dateTimeProvider);
    }

    public bool RemovePostById(PostId postId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!_postsTracked.Remove(postId))
        {
            return false;
        }

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public Post? FindPostById(PostId postId)
    {
        if (!_postsTracked.Contains(postId))
        {
            return null;
        }

        return _postsTracked.Find(x => x.Id == postId);
    }

    public InvitationId? InviteUser(UserId inviterId, UserId inviteeId, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return null;
        }

        if (_invitationsTracked
                .FirstOrDefault(x => x.GroupId == Id && x.InviterId == inviterId && x.InviteeId == inviteeId) is not
            null)
        {
            return null;
        }

        var dateTime = dateTimeProvider.UtcNow;
        var id = InvitationId.Create();
        var invitation = Invitation.Create(id, dateTime, dateTime, inviterId, Id, inviteeId, InvitationStatus.Pending);
        if (invitation.IsError)
        {
            return null;
        }

        if (!AddDomainEvent(new UserInvitedEvent(dateTimeProvider.UtcNow, id)))
        {
            return null;
        }

        _invitationsTracked.Add(invitation.Value!);

        return id;
    }

    public bool RemoveInvitation(InvitationId invitationId)
    {
        if (_invitationsTracked.Remove(invitationId))
        {
            RemoveDomainEvent(x => x is UserInvitedEvent invitedEvent && invitedEvent.InvitationId == invitationId);
        }

        return false;
    }

    public override string ToString() => Name.ToString();

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Group? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        return obj is Group value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Group author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Group? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        return Id.CompareTo(other.Id);
    }

    public static bool operator ==(Group left, Group right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Group left, Group right) => !(left == right);

    public static bool operator <(Group left, Group right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Group left, Group right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Group left, Group right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Group left, Group right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;

    public class CompareByName : Comparer<Group>
    {
        public override int Compare(Group? x, Group? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            var result = new Name.CompareByName().Compare(x!.Name, y!.Name);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtc().Compare(x, y);
        }
    }

    public class CompareByNameDesc : Comparer<Group>
    {
        public override int Compare(Group? x, Group? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            var result = new Name.CompareByNameDesc().Compare(x!.Name, y!.Name);
            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtcDesc().Compare(x, y);
        }
    }
}