using FluentValidation;

namespace Libellus.Application.Commands.Formats.UpdateFormat;

public sealed class UpdateFormatCommandValidator : AbstractValidator<UpdateFormatCommand>
{
    public UpdateFormatCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}