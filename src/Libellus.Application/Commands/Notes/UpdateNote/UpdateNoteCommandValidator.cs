using FluentValidation;

namespace Libellus.Application.Commands.Notes.UpdateNote;

public sealed class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
{
    public UpdateNoteCommandValidator()
    {
        RuleFor(x => x.Item)
            .NotNull();
    }
}