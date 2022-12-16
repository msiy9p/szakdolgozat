namespace Libellus.Infrastructure.Persistence.DataModels.Common.Interfaces;

internal interface IId<TId> where TId : IEquatable<TId>
{
    public TId Id { get; set; }
}