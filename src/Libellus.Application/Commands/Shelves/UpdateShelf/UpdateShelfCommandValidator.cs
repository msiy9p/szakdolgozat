using FluentValidation;

namespace Libellus.Application.Commands.Shelves.UpdateShelf;

public sealed class UpdateShelfCommandValidator : AbstractValidator<UpdateShelfCommand>
{
    public UpdateShelfCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}