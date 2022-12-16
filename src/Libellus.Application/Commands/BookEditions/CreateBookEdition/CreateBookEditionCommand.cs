using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models.DTOs;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.BookEditions.CreateBookEdition;

public sealed record CreateBookEditionCommand(BookId BookId, Title Title, DescriptionText? DescriptionText,
    FormatDto? Format, LanguageDto? Language, PublisherDto? Publisher,
    PartialDate? PublishedOn, bool IsTranslation, PageCount? PageCount, WordCount? WordCount,
    Isbn? Isbn, CoverImageMetaDataContainer? CoverImageMetaDataContainer) : ICommand<BookEditionIds>;