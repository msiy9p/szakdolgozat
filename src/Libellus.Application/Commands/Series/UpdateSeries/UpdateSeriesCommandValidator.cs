using FluentValidation;

namespace Libellus.Application.Commands.Series.UpdateSeries;

public sealed class UpdateSeriesCommandValidator : AbstractValidator<UpdateSeriesCommand>
{
    public UpdateSeriesCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}