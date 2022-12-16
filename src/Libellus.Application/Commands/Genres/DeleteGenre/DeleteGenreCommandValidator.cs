using FluentValidation;

namespace Libellus.Application.Commands.Genres.DeleteGenre;

public sealed class DeleteGenreCommandValidator : AbstractValidator<DeleteGenreCommand>
{
    public DeleteGenreCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}