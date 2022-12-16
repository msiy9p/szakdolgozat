using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models.DTOs;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Entities;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Books.CreateBook;

public sealed record CreateBookCommand(Title Title, DescriptionText? DescriptionText,
    LiteratureFormDto? LiteratureForm, SeriesDto? SeriesDto,
    CoverImageMetaDataContainer? CoverImageMetaDataContainer,
    IReadOnlyCollection<AuthorDto> Authors, IReadOnlyCollection<GenreDto> Genres,
    IReadOnlyCollection<TagDto> Tags, IReadOnlyCollection<WarningTagDto> WarningTags) : ICommand<BookIds>;