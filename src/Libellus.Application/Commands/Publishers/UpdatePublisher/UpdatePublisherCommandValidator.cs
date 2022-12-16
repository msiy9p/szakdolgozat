using FluentValidation;

namespace Libellus.Application.Commands.Publishers.UpdatePublisher;

public sealed class UpdatePublisherCommandValidator : AbstractValidator<UpdatePublisherCommand>
{
    public UpdatePublisherCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}