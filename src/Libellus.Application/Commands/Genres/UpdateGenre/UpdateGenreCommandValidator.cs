using FluentValidation;

namespace Libellus.Application.Commands.Genres.UpdateGenre;

public sealed class UpdateGenreCommandValidator : AbstractValidator<UpdateGenreCommand>
{
    public UpdateGenreCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}