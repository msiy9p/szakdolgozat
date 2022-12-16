using FluentValidation;

namespace Libellus.Application.Commands.Publishers.UpdatePublisherById;

public sealed class UpdatePublisherByIdCommandValidator : AbstractValidator<UpdatePublisherByIdCommand>
{
    public UpdatePublisherByIdCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}