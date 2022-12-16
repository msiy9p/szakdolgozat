using FluentValidation;

namespace Libellus.Application.Commands.Series.DeleteSeries;

public sealed class DeleteSeriesCommandValidator : AbstractValidator<DeleteSeriesCommand>
{
    public DeleteSeriesCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}