using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;
using DomainNote = Libellus.Domain.Entities.Note;
using DomainReading = Libellus.Domain.Entities.Reading;
using PersistenceBookEdition = Libellus.Infrastructure.Persistence.DataModels.BookEdition;
using PersistenceLiteratureForm = Libellus.Infrastructure.Persistence.DataModels.LiteratureForm;
using PersistenceReading = Libellus.Infrastructure.Persistence.DataModels.Reading;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct ReadingMapper :
    IMapFrom<PersistenceReading, UserPicturedVm, UserPicturedVm?, PersistenceBookEdition, PersistenceLiteratureForm?,
        BookEditionCompactVm, Result<DomainReading>>,
    IMapFrom<PersistenceReading, UserPicturedVm, UserPicturedVm?, PersistenceLiteratureForm?, BookEditionCompactVm,
        Result<DomainReading>>,
    IMapFrom<DomainReading, PersistenceReading>, IMapFrom<DomainReading, GroupId, PersistenceReading>
{
    public static Result<DomainReading> Map(PersistenceReading item1, UserPicturedVm item2, UserPicturedVm? item3,
        PersistenceBookEdition item4, PersistenceLiteratureForm? item5, BookEditionCompactVm item6)
    {
        var note = item1.Note is null ? null : NoteMapper.Map(item1.Note!, item3).Value;

        return DomainReading.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new ReadingFriendlyId(item1.FriendlyId),
            item2,
            item6,
            PageCount.Convert(item4.PageCount),
            ScoreMultiplier.Convert(item5?.ScoreMultiplier),
            note,
            item1.IsDnf,
            item1.IsReread,
            item1.StartedOnUtc,
            item1.FinishedOnUtc);
    }

    public static Result<DomainReading> Map(PersistenceReading item1, UserPicturedVm item2, UserPicturedVm? item3,
        PersistenceLiteratureForm? item4, BookEditionCompactVm item6)
    {
        var note = item1.Note is null ? null : NoteMapper.Map(item1.Note!, item3).Value;

        return DomainReading.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new ReadingFriendlyId(item1.FriendlyId),
            item2,
            item6,
            PageCount.Convert(item1.BookEdition?.PageCount),
            ScoreMultiplier.Convert(item4?.ScoreMultiplier),
            note,
            item1.IsDnf,
            item1.IsReread,
            item1.StartedOnUtc,
            item1.FinishedOnUtc);
    }

    public static PersistenceReading Map(DomainReading item1)
    {
        return new PersistenceReading()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            BookEditionId = item1.BookEditionId,
            NoteId = item1.Note?.Id,
            IsDnf = item1.IsDnf,
            IsReread = item1.IsReread,
            Score = item1.Score,
            StartedOnUtc = item1.StartedOnUtc,
            FinishedOnUtc = item1.FinishedOnUtc,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc
        };
    }

    public static PersistenceReading Map(DomainReading item1, GroupId item2)
    {
        var reading = Map(item1);
        reading.GroupId = item2;

        return reading;
    }
}