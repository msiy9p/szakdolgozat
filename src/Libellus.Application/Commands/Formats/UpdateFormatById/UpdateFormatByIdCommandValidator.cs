using FluentValidation;

namespace Libellus.Application.Commands.Formats.UpdateFormatById;

public sealed class UpdateFormatByIdCommandValidator : AbstractValidator<UpdateFormatByIdCommand>
{
    public UpdateFormatByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}