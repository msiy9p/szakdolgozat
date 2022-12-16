using FluentValidation;

namespace Libellus.Application.Commands.Shelves.CreateShelf;

public sealed class CreateShelfCommandValidator : AbstractValidator<CreateShelfCommand>
{
    public CreateShelfCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}