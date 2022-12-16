using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;
using DomainNote = Libellus.Domain.Entities.Note;
using PersistenceNote = Libellus.Infrastructure.Persistence.DataModels.Note;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct NoteMapper : IMapFrom<PersistenceNote, UserPicturedVm?, Result<DomainNote>>,
    IMapFrom<DomainNote, PersistenceNote>, IMapFrom<DomainNote, GroupId, PersistenceNote>
{
    public static Result<DomainNote> Map(PersistenceNote item1, UserPicturedVm? item2)
    {
        return DomainNote.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            item2,
            (CommentText)item1.Text);
    }

    public static PersistenceNote Map(DomainNote item)
    {
        return new PersistenceNote()
        {
            Id = item.Id,
            CreatorId = item.CreatorId,
            Text = item.Text,
            CreatedOnUtc = item.CreatedOnUtc,
            ModifiedOnUtc = item.ModifiedOnUtc
        };
    }

    public static PersistenceNote Map(DomainNote item1, GroupId item2)
    {
        var note = Map(item1);
        note.GroupId = item2;

        return note;
    }
}