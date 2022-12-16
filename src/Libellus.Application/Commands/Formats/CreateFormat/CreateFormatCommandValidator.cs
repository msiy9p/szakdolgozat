using FluentValidation;

namespace Libellus.Application.Commands.Formats.CreateFormat;

public sealed class CreateFormatCommandValidator : AbstractValidator<CreateFormatCommand>
{
    public CreateFormatCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}