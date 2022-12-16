using System.Text;
using Libellus.Domain.Common.Interfaces.Models;
using Libellus.Domain.Common.Models;

namespace LibellusWeb.Common;

public abstract class BaseImgSrcSetWriter<TImageDataContainer, TEntity, TKey> where TImageDataContainer :
    BaseImageMetaDataContainer<TEntity, TKey>
    where TEntity : IImageMetaData, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly StringBuilder _stringBuilder = new();
    private readonly string _src;

    public bool HasItems { get; init; }

    protected BaseImgSrcSetWriter(string linkBase, TImageDataContainer? metaDataContainer)
    {
        if (metaDataContainer is null || metaDataContainer.Count < 1)
        {
            HasItems = false;
            _src = string.Empty;
            return;
        }

        _stringBuilder.AppendJoin(", ", metaDataContainer.AvailableImageMetaData
            .Select(x => $"{linkBase}/{x.ToString()} {x.Width}w"));

        _src = metaDataContainer.AvailableImageMetaData.MaxBy(x => x.Width)?.ToString() ?? string.Empty;
        _src = $"{linkBase}/{_src}";

        HasItems = true;
    }

    public abstract string GetAltText();

    public string GetSrc() => _src;

    public string GetSrcSet() => _stringBuilder.ToString();

    public abstract string GetSizes();
}