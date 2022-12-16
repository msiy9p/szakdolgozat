using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Models.DTOs;

public sealed class LiteratureFormDto
{
    public ShortName ShortName { get; init; }
    public ScoreMultiplier ScoreMultiplier { get; init; }
    public LiteratureFormId? LiteratureFormId { get; init; }

    public LiteratureFormDto(ShortName shortName, ScoreMultiplier scoreMultiplier, LiteratureFormId? literatureFormId)
    {
        ShortName = shortName;
        ScoreMultiplier = scoreMultiplier;
        LiteratureFormId = literatureFormId;
    }
}