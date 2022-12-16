using NpgsqlTypes;

namespace Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;

internal interface ISearchable
{
    NpgsqlTsVector SearchVectorOne { get; set; }
    NpgsqlTsVector SearchVectorTwo { get; set; }
}