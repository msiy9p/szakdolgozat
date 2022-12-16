using FluentValidation;

namespace Libellus.Application.Commands.Notes.DeleteNote;

public sealed class DeleteNoteCommandValidator : AbstractValidator<DeleteNoteCommand>
{
    public DeleteNoteCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}