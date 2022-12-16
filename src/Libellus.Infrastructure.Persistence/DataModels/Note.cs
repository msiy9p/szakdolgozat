#pragma warning disable CS8618

using Libellus.Domain.Common.Types.Ids;
using Libellus.Infrastructure.Persistence.DataModels.Common;
using NodaTime;

namespace Libellus.Infrastructure.Persistence.DataModels;

internal class Note : BaseStampedModel<NoteId, GroupId>
{
    public UserId? CreatorId { get; set; }

    public string Text { get; set; }

    public Reading Reading { get; set; }

    public Note()
    {
    }

    public Note(NoteId id, GroupId groupId, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc, UserId? creatorId,
        string text) : base(id, groupId, createdOnUtc, modifiedOnUtc)
    {
        CreatorId = creatorId;
        Text = text;
        CreatedOnUtc = createdOnUtc;
        ModifiedOnUtc = modifiedOnUtc;
    }
}