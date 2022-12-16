using FluentValidation;

namespace Libellus.Application.Commands.Publishers.CreatePublisher;

public sealed class CreatePublisherCommandValidator : AbstractValidator<CreatePublisherCommand>
{
    public CreatePublisherCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}