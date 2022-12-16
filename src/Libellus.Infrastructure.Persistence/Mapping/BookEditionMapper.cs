using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using DomainBookEdition = Libellus.Domain.Entities.BookEdition;
using DomainCoverImageMetaData = Libellus.Domain.Entities.CoverImageMetaData;
using DomainFormat = Libellus.Domain.Entities.Format;
using DomainLanguage = Libellus.Domain.Entities.Language;
using DomainPublisher = Libellus.Domain.Entities.Publisher;
using DomainReading = Libellus.Domain.Entities.Reading;
using PersistenceBookEdition = Libellus.Infrastructure.Persistence.DataModels.BookEdition;
using PersistenceCoverImage = Libellus.Infrastructure.Persistence.DataModels.CoverImageMetaData;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct BookEditionMapper :
    IMapFrom<PersistenceBookEdition, BookCompactVm, UserVm?, Result<DomainBookEdition>>,
    IMapFrom<PersistenceBookEdition, BookCompactVm, UserVm?, ICollection<PersistenceCoverImage>,
        Result<DomainBookEdition>>,
    IMapFrom<PersistenceBookEdition, BookCompactVm, UserVm?, ICollection<DomainCoverImageMetaData>,
        Result<DomainBookEdition>>,
    IMapFrom<PersistenceBookEdition, BookCompactVm, UserVm?, ICollection<DomainCoverImageMetaData>,
        ICollection<DomainReading>,
        Result<DomainBookEdition>>, IMapFrom<DomainBookEdition, PersistenceBookEdition>,
    IMapFrom<DomainBookEdition, GroupId, PersistenceBookEdition>
{
    public static Result<DomainBookEdition> Map(PersistenceBookEdition item1, BookCompactVm item2, UserVm? item3)
    {
        var format = item1.Format is null ? null : FormatMapper.Map(item1.Format!).Value;
        var language = item1.Language is null ? null : LanguageMapper.Map(item1.Language!).Value;
        var publisher = item1.Publisher is null ? null : PublisherMapper.Map(item1.Publisher!).Value;

        return DomainBookEdition.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookEditionFriendlyId(item1.FriendlyId),
            item3,
            item2,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            format,
            language,
            publisher,
            PartialDate.Create(item1.PublishedOnYear, item1.PublishedOnMonth, item1.PublishedOnDay),
            item1.IsTranslation,
            PageCount.Convert(item1.PageCount),
            WordCount.Convert(item1.WordCount),
            Isbn.Convert(item1.Isbn),
            null);
    }

    public static Result<DomainBookEdition> Map(PersistenceBookEdition item1, BookCompactVm item2, UserVm? item3,
        ICollection<PersistenceCoverImage> item4)
    {
        var format = item1.Format is null ? null : FormatMapper.Map(item1.Format!).Value;
        var language = item1.Language is null ? null : LanguageMapper.Map(item1.Language!).Value;
        var publisher = item1.Publisher is null ? null : PublisherMapper.Map(item1.Publisher!).Value;

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

            return DomainBookEdition.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new BookEditionFriendlyId(item1.FriendlyId),
                item3,
                item2,
                (Title)item1.Title,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                format,
                language,
                publisher,
                PartialDate.Create(item1.PublishedOnYear, item1.PublishedOnMonth, item1.PublishedOnDay),
                item1.IsTranslation,
                PageCount.Convert(item1.PageCount),
                WordCount.Convert(item1.WordCount),
                Isbn.Convert(item1.Isbn),
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, imgMeta));
        }

        return DomainBookEdition.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookEditionFriendlyId(item1.FriendlyId),
            item3,
            item2,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            format,
            language,
            publisher,
            PartialDate.Create(item1.PublishedOnYear, item1.PublishedOnMonth, item1.PublishedOnDay),
            item1.IsTranslation,
            PageCount.Convert(item1.PageCount),
            WordCount.Convert(item1.WordCount),
            Isbn.Convert(item1.Isbn),
            null);
    }

    public static Result<DomainBookEdition> Map(PersistenceBookEdition item1, BookCompactVm item2, UserVm? item3,
        ICollection<DomainCoverImageMetaData> item4)
    {
        var format = item1.Format is null ? null : FormatMapper.Map(item1.Format!).Value;
        var language = item1.Language is null ? null : LanguageMapper.Map(item1.Language!).Value;
        var publisher = item1.Publisher is null ? null : PublisherMapper.Map(item1.Publisher!).Value;

        if (item1.CoverImageId.HasValue)
        {
            return DomainBookEdition.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new BookEditionFriendlyId(item1.FriendlyId),
                item3,
                item2,
                (Title)item1.Title,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                format,
                language,
                publisher,
                PartialDate.Create(item1.PublishedOnYear, item1.PublishedOnMonth, item1.PublishedOnDay),
                item1.IsTranslation,
                PageCount.Convert(item1.PageCount),
                WordCount.Convert(item1.WordCount),
                Isbn.Convert(item1.Isbn),
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, item4));
        }

        return DomainBookEdition.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookEditionFriendlyId(item1.FriendlyId),
            item3,
            item2,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            format,
            language,
            publisher,
            PartialDate.Create(item1.PublishedOnYear, item1.PublishedOnMonth, item1.PublishedOnDay),
            item1.IsTranslation,
            PageCount.Convert(item1.PageCount),
            WordCount.Convert(item1.WordCount),
            Isbn.Convert(item1.Isbn),
            null);
    }

    public static Result<DomainBookEdition> Map(PersistenceBookEdition item1, BookCompactVm item2, UserVm? item3,
        ICollection<DomainCoverImageMetaData> item4, ICollection<DomainReading> item5)
    {
        var format = item1.Format is null ? null : FormatMapper.Map(item1.Format!).Value;
        var language = item1.Language is null ? null : LanguageMapper.Map(item1.Language!).Value;
        var publisher = item1.Publisher is null ? null : PublisherMapper.Map(item1.Publisher!).Value;

        if (item1.CoverImageId.HasValue)
        {
            return DomainBookEdition.Create(
                item1.Id,
                item1.CreatedOnUtc,
                item1.ModifiedOnUtc,
                new BookEditionFriendlyId(item1.FriendlyId),
                item3,
                item2,
                (Title)item1.Title,
                (item1.Description is null ? null : (DescriptionText)item1.Description),
                format,
                language,
                publisher,
                PartialDate.Create(item1.PublishedOnYear, item1.PublishedOnMonth, item1.PublishedOnDay),
                item1.IsTranslation,
                PageCount.Convert(item1.PageCount),
                WordCount.Convert(item1.WordCount),
                Isbn.Convert(item1.Isbn),
                new CoverImageMetaDataContainer(item1.CoverImageId!.Value, item4),
                item5);
        }

        return DomainBookEdition.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new BookEditionFriendlyId(item1.FriendlyId),
            item3,
            item2,
            (Title)item1.Title,
            (item1.Description is null ? null : (DescriptionText)item1.Description),
            format,
            language,
            publisher,
            PartialDate.Create(item1.PublishedOnYear, item1.PublishedOnMonth, item1.PublishedOnDay),
            item1.IsTranslation,
            PageCount.Convert(item1.PageCount),
            WordCount.Convert(item1.WordCount),
            Isbn.Convert(item1.Isbn),
            null,
            item5);
    }

    public static PersistenceBookEdition Map(DomainBookEdition item1)
    {
        return new PersistenceBookEdition()
        {
            Id = item1.Id,
            BookId = item1.BookId,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            CoverImageId = item1.AvailableCovers?.Id,
            Title = item1.Title.Value,
            TitleNormalized = item1.Title.ValueNormalized,
            Description = item1.Description?.Value,
            FormatId = item1.Format?.Id,
            LanguageId = item1.Language?.Id,
            PublisherId = item1.Publisher?.Id,
            PublishedOnYear = item1.PublishedOn?.Year,
            PublishedOnMonth = item1.PublishedOn?.Month,
            PublishedOnDay = item1.PublishedOn?.Day,
            IsTranslation = item1.IsTranslation,
            PageCount = item1.PageCount,
            WordCount = item1.WordCount,
            Isbn = item1.Isbn,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc,
        };
    }

    public static PersistenceBookEdition Map(DomainBookEdition item1, GroupId item2)
    {
        var bookEdition = Map(item1);
        bookEdition.GroupId = item2;

        return bookEdition;
    }
}