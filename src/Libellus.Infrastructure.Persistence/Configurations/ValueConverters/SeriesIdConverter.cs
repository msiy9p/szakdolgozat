﻿using Libellus.Domain.Common.Types.Ids;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Libellus.Infrastructure.Persistence.Configurations.ValueConverters;

public sealed class SeriesIdConverter : ValueConverter<SeriesId, Guid>
{
    public SeriesIdConverter() :
        base(id => id.Value,
            guid => new SeriesId(guid))
    {
    }
}