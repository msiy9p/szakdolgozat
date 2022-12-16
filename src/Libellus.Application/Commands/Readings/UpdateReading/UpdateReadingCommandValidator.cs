using FluentValidation;

namespace Libellus.Application.Commands.Readings.UpdateReading;

public sealed class UpdateReadingCommandValidator : AbstractValidator<UpdateReadingCommand>
{
    public UpdateReadingCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}