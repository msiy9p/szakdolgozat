using FluentValidation;

namespace Libellus.Application.Commands.Readings.DeleteReading;

public sealed class DeleteReadingCommandValidator : AbstractValidator<DeleteReadingCommand>
{
    public DeleteReadingCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}