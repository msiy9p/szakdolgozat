using Libellus.Infrastructure.Persistence.Mapping.Interfaces;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;
using Libellus.Domain.ViewModels;
using DomainBook = Libellus.Domain.Entities.Book;
using DomainSeries = Libellus.Domain.Entities.Series;
using PersistenceSeries = Libellus.Infrastructure.Persistence.DataModels.Series;

namespace Libellus.Infrastructure.Persistence.Mapping;

internal readonly struct SeriesMapper : IMapFrom<PersistenceSeries, UserVm?, Result<DomainSeries>>,
    IMapFrom<PersistenceSeries, UserVm?, ICollection<DomainBook>, Result<DomainSeries>>,
    IMapFrom<DomainSeries, PersistenceSeries>, IMapFrom<DomainSeries, GroupId, PersistenceSeries>
{
    public static Result<DomainSeries> Map(PersistenceSeries item1, UserVm? item2)
    {
        return DomainSeries.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new SeriesFriendlyId(item1.FriendlyId),
            item2,
            (Title)item1.Title);
    }

    public static Result<DomainSeries> Map(PersistenceSeries item1, UserVm? item2, ICollection<DomainBook> item3)
    {
        return DomainSeries.Create(
            item1.Id,
            item1.CreatedOnUtc,
            item1.ModifiedOnUtc,
            new SeriesFriendlyId(item1.FriendlyId),
            item2,
            (Title)item1.Title,
            item3);
    }

    public static PersistenceSeries Map(DomainSeries item1)
    {
        return new PersistenceSeries()
        {
            Id = item1.Id,
            FriendlyId = item1.FriendlyId.Value,
            CreatorId = item1.CreatorId,
            Title = item1.Title.Value,
            TitleNormalized = item1.Title.ValueNormalized,
            CreatedOnUtc = item1.CreatedOnUtc,
            ModifiedOnUtc = item1.ModifiedOnUtc
        };
    }

    public static PersistenceSeries Map(DomainSeries item1, GroupId item2)
    {
        var series = Map(item1);
        series.GroupId = item2;

        return series;
    }
}