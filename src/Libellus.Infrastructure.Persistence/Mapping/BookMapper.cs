using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using DomainAuthor = Libellus.Domain.Entities.Author;
using DomainBook = Libellus.Domain.Entities.Book;
using DomainBookEdition = Libellus.Domain.Entities.BookEdition;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using DomainGenre = Libellus.Domain.Entities.Genre;
using DomainLiteratureForm = Libellus.Domain.Entities.LiteratureForm;
using DomainSeries = Libellus.Domain.Entities.Series;
using DomainTag = Libellus.Domain.Entities.Tag;
using DomainWarningTag = Libellus.Domain.Entities.WarningTag;
using PersistenceBook = Libellus.Infrastructure.Persistence.DataModels.Book;
using PersistenceCoverImageMetaData = Libellus.Infrastructure.Persistence.DataModels.CoverImageMetaData;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct BookMapper :
    IMapFrom<PersistenceBook, UserVm?, UserVm?, Result<DomainBook>>,
    IMapFrom<PersistenceBook, UserVm?, UserVm?, ICollection<DomainCoverImageMetaData>, Result<DomainBook>>,
    IMapFrom<PersistenceBook, UserVm?, UserVm?, ICollection<DomainCoverImageMetaData>, ICollection<DomainAuthor>,
        ICollection<DomainGenre>, ICollection<DomainTag>, ICollection<DomainWarningTag>, Result<DomainBook>>,
    IMapFrom<PersistenceBook, UserVm?, UserVm?, ICollection<PersistenceCoverImageMetaData>, ICollection<DomainAuthor>,
        ICollection<DomainGenre>, ICollection<DomainTag>, ICollection<DomainWarningTag>, Result<DomainBook>>,
    IMapFrom<DomainBook, PersistenceBook>, IMapFrom<DomainBook, GroupId, PersistenceBook>
{
    public static Result<DomainBook> Map(PersistenceBook item1, UserVm? item2, UserVm? item3)
    {
        var literatureForm =
            item1.LiteratureForm is null ? null : LiteratureFormMapper.Map(item1.LiteratureForm!).Value;
        var series = item1.BookSeriesConnector?.Series is null
            ? null
            : SeriesMapper.Map(item1.BookSeriesConnector?.Series!, item3).Value;

        return DomainBook.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookFriendlyId(item1.FriendlyId),
            item2,
            literatureForm,
            series,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.BookSeriesConnector?.NumberInSeries,
            null);
    }

    public static Result<DomainBook> Map(PersistenceBook item1, UserVm? item2, UserVm? item3,
        ICollection<DomainCoverImageMetaData> item4)
    {
        var literatureForm =
            item1.LiteratureForm is null ? null : LiteratureFormMapper.Map(item1.LiteratureForm!).Value;
        var series = item1.BookSeriesConnector?.Series is null
            ? null
            : SeriesMapper.Map(item1.BookSeriesConnector?.Series!, item3).Value;

        if (item1.CoverImageId.HasValue)
        {
            return DomainBook.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new BookFriendlyId(item1.FriendlyId),
                item2,
                literatureForm,
                series,
                (Title)item1.Title,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                item1.BookSeriesConnector?.NumberInSeries,
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, item4));
        }

        return DomainBook.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookFriendlyId(item1.FriendlyId),
            item2,
            literatureForm,
            series,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.BookSeriesConnector?.NumberInSeries,
            null);
    }

    public static Result<DomainBook> Map(PersistenceBook item1, UserVm? item2, UserVm? item3,
        ICollection<DomainCoverImageMetaData> item4,
        ICollection<DomainAuthor> item5, ICollection<DomainGenre> item6, ICollection<DomainTag> item7,
        ICollection<DomainWarningTag> item8)
    {
        var literatureForm =
            item1.LiteratureForm is null ? null : LiteratureFormMapper.Map(item1.LiteratureForm!).Value;
        var series = item1.BookSeriesConnector?.Series is null
            ? null
            : SeriesMapper.Map(item1.BookSeriesConnector?.Series!, item3).Value;

        if (item1.CoverImageId.HasValue)
        {
            return DomainBook.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new BookFriendlyId(item1.FriendlyId),
                item2,
                literatureForm,
                series,
                (Title)item1.Title,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                item1.BookSeriesConnector?.NumberInSeries,
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, item4),
                item5,
                item6,
                item7,
                item8,
                Array.Empty<DomainBookEdition>());
        }

        return DomainBook.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookFriendlyId(item1.FriendlyId),
            item2,
            literatureForm,
            series,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.BookSeriesConnector?.NumberInSeries,
            null,
            item5,
            item6,
            item7,
            item8,
            Array.Empty<DomainBookEdition>());
    }

    public static Result<DomainBook> Map(PersistenceBook item1,
        UserVm? item2,
        UserVm? item3,
        ICollection<DomainCoverImageMetaData> item4,
        ICollection<DomainAuthor> item5,
        ICollection<DomainGenre> item6,
        ICollection<DomainTag> item7,
        ICollection<DomainWarningTag> item8,
        ICollection<DomainBookEdition> item9)
    {
        var literatureForm =
            item1.LiteratureForm is null ? null : LiteratureFormMapper.Map(item1.LiteratureForm!).Value;
        var series = item1.BookSeriesConnector?.Series is null
            ? null
            : SeriesMapper.Map(item1.BookSeriesConnector?.Series!, item3).Value;

        if (item1.CoverImageId.HasValue)
        {
            return DomainBook.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new BookFriendlyId(item1.FriendlyId),
                item2,
                literatureForm,
                series,
                (Title)item1.Title,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                item1.BookSeriesConnector?.NumberInSeries,
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, item4),
                item5,
                item6,
                item7,
                item8,
                item9);
        }

        return DomainBook.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookFriendlyId(item1.FriendlyId),
            item2,
            literatureForm,
            series,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.BookSeriesConnector?.NumberInSeries,
            null,
            item5,
            item6,
            item7,
            item8,
            item9);
    }

    public static Result<DomainBook> Map(PersistenceBook item1, UserVm? item2, UserVm? item3,
        ICollection<PersistenceCoverImageMetaData> item4,
        ICollection<DomainAuthor> item5, ICollection<DomainGenre> item6, ICollection<DomainTag> item7,
        ICollection<DomainWarningTag> item8)
    {
        var literatureForm =
            item1.LiteratureForm is null ? null : LiteratureFormMapper.Map(item1.LiteratureForm!).Value;
        var series = item1.BookSeriesConnector?.Series is null
            ? null
            : SeriesMapper.Map(item1.BookSeriesConnector?.Series!, item3).Value;

        if (item1.CoverImageId.HasValue)
        {
            var imgMeta = new List<CoverImageMetaData>(item4.Count);
            foreach (var item in item4)
            {
                var temp = CoverImageMetaDataMapper.Map(item);

                if (temp.IsSuccess)
                {
                    imgMeta.Add(temp.Value!);
                }
            }

            return DomainBook.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new BookFriendlyId(item1.FriendlyId),
                item2,
                literatureForm,
                series,
                (Title)item1.Title,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                item1.BookSeriesConnector?.NumberInSeries,
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, imgMeta),
                item5,
                item6,
                item7,
                item8,
                Array.Empty<DomainBookEdition>());
        }

        return DomainBook.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookFriendlyId(item1.FriendlyId),
            item2,
            literatureForm,
            series,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            item1.BookSeriesConnector?.NumberInSeries,
            null,
            item5,
            item6,
            item7,
            item8,
            Array.Empty<DomainBookEdition>());
    }

    public static PersistenceBook Map(DomainBook item1)
    {
        return new PersistenceBook()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            CoverImageId = item1.AvailableCovers?.Id,
            LiteratureFormId = item1.LiteratureForm?.Id,
            Title = item1.Title.Value,
            TitleNormalized = item1.Title.ValueNormalized,
            Description = item1.Description?.Value,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc,
        };
    }

    public static PersistenceBook Map(DomainBook item1, GroupId item2)
    {
        var book = Map(item1);
        book.GroupId = item2;

        return book;
    }
}