using FluentValidation;

namespace Libellus.Application.Commands.Notes.CreateNote;

public sealed class CreateNoteCommandValidator : AbstractValidator<CreateNoteCommand>
{
    public CreateNoteCommandValidator()
    {
        RuleFor(x => x.CommentText)
            .NotNull();
    }
}