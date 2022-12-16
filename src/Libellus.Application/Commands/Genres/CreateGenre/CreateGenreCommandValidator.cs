using FluentValidation;

namespace Libellus.Application.Commands.Genres.CreateGenre;

public sealed class CreateGenreCommandValidator : AbstractValidator<CreateGenreCommand>
{
    public CreateGenreCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}