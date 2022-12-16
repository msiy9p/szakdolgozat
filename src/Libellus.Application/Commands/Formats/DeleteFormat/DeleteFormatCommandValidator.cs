using FluentValidation;

namespace Libellus.Application.Commands.Formats.DeleteFormat;

public sealed class DeleteFormatCommandValidator : AbstractValidator<DeleteFormatCommand>
{
    public DeleteFormatCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}