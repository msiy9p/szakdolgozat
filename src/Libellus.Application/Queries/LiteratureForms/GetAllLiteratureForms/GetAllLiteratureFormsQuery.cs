using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Enums;
using Libellus.Domain.Entities;

namespace Libellus.Application.Queries.LiteratureForms.GetAllLiteratureForms;

public sealed record GetAllLiteratureFormsQuery(SortOrder SortOrder) : IQuery<ICollection<LiteratureForm>>
{
    public static readonly GetAllLiteratureFormsQuery DefaultInstance = new(SortOrder.Ascending);
}