#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Reading : BaseStampedModel<ReadingId, GroupId>
{
    public string FriendlyId { get; set; }
    public UserId CreatorId { get; set; }
    public BookEditionId BookEditionId { get; set; }
    public NoteId? NoteId { get; set; }

    public bool IsDnf { get; set; } = false;
    public bool IsReread { get; set; } = false;
    public double? Score { get; set; }
    public ZonedDateTime? StartedOnUtc { get; set; }
    public ZonedDateTime? FinishedOnUtc { get; set; }

    public BookEdition BookEdition { get; set; }
    public Note? Note { get; set; }

    public Reading()
    {
    }

    public Reading(ReadingId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        string friendlyId, UserId creatorId, BookEditionId bookEditionId, NoteId? noteId, bool isDnf, bool isReread,
        double? score, ZonedDateTime? startedOnUtc, ZonedDateTime? finishedOnUtc) : base(id, groupId, createdOnUtc,
        modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        CreatorId = creatorId;
        BookEditionId = bookEditionId;
        NoteId = noteId;
        IsDnf = isDnf;
        IsReread = isReread;
        Score = score;
        StartedOnUtc = startedOnUtc;
        FinishedOnUtc = finishedOnUtc;
    }
}