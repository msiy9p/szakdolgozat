using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.Languages.GetAllLanguages;

public sealed record GetAllLanguagesQuery(SortOrder SortOrder) : IQuery<ICollection<Language>>
{
    public static readonly GetAllLanguagesQuery DefaultInstance = new(SortOrder.Ascending);
}