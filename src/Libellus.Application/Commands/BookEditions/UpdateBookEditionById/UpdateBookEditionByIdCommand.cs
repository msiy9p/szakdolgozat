using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models.DTOs;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.BookEditions.UpdateBookEditionById;

public sealed record UpdateBookEditionByIdCommand(BookEditionId BookEditionId, Title Title,
    DescriptionText? DescriptionText,
    FormatDto? Format, LanguageDto? Language, PublisherDto? Publisher,
    PartialDate? PublishedOn, bool IsTranslation, PageCount? PageCount, WordCount? WordCount,
    Isbn? Isbn) : ICommand;