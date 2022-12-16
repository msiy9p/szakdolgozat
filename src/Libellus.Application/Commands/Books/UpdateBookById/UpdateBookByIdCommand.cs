using Libellus.Application.Common.Interfaces.Messaging;
using Libellus.Application.Models.DTOs;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Models;
using Libellus.Domain.ValueObjects;

namespace Libellus.Application.Commands.Books.UpdateBookById;

public record UpdateBookByIdCommand(BookId BookId, Title Title, DescriptionText? DescriptionText,
    LiteratureFormDto? LiteratureForm, SeriesDto? SeriesDto,
    IReadOnlyCollection<AuthorDto> Authors, IReadOnlyCollection<GenreDto> Genres,
    IReadOnlyCollection<TagDto> Tags, IReadOnlyCollection<WarningTagDto> WarningTags) : ICommand;