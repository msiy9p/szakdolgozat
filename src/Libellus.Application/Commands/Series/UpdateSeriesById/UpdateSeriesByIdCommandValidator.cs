using FluentValidation;

namespace Libellus.Application.Commands.Series.UpdateSeriesById;

public sealed class UpdateSeriesByIdCommandValidator : AbstractValidator<UpdateSeriesByIdCommand>
{
    public UpdateSeriesByIdCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}