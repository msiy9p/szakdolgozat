using FluentValidation;

namespace Libellus.Application.Commands.Publishers.DeletePublisher;

public sealed class DeletePublisherCommandValidator : AbstractValidator<DeletePublisherCommand>
{
    public DeletePublisherCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}