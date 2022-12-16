using FluentValidation;

namespace Libellus.Application.Commands.Series.CreateSeries;

public sealed class CreateSeriesCommandValidator : AbstractValidator<CreateSeriesCommand>
{
    public CreateSeriesCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotNull();
    }
}