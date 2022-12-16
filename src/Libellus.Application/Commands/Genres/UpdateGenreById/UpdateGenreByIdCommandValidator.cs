using FluentValidation;

namespace Libellus.Application.Commands.Genres.UpdateGenreById;

public sealed class UpdateGenreByIdCommandValidator : AbstractValidator<UpdateGenreByIdCommand>
{
    public UpdateGenreByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}