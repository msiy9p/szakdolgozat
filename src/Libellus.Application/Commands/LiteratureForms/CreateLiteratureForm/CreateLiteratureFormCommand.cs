using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.LiteratureForms.CreateLiteratureForm;

public sealed record CreateLiteratureFormCommand
    (ShortName Name, ScoreMultiplier ScoreMultiplier) : ICommand<LiteratureFormId>;