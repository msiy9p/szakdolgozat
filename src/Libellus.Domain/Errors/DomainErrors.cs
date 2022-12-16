using Libellus.Domain.Common.Errors;
using Libellus.Domain.Common.Types.Ids;

namespace Libellus.Domain.Errors;

public static class DomainErrors
{
    public static class GeneralErrors
    {
        public static readonly Error InputIsNull =
            new(nameof(InputIsNull), "Provided input is null.");

        public static readonly Error InputIsNegative =
            new(nameof(InputIsNegative), "Provided input is negative.");

        public static readonly Error InputIsNegativeOrZero =
            new(nameof(InputIsNegative), "Provided input is negative or zero.");

        public static readonly Error StringNullOrWhiteSpace =
            new(nameof(StringNullOrWhiteSpace), "Provided string is null or empty or whitespace.");

        public static readonly Error OperationCancelled =
            new(nameof(OperationCancelled), "Operation was cancelled.");
    }

    public static class JsonErrors
    {
        public static readonly Error JsonConversionError =
            new(nameof(JsonConversionError), "Error occurred during json conversion.");
    }

    public static class ImageErrors
    {
        public static readonly Error ImageSizeNotSupported =
            new(nameof(ImageSizeNotSupported), "Specified image size is not supported.");

        public static readonly Error ImageSizeWidthNotSupported =
            new(nameof(ImageSizeWidthNotSupported), "Specified image width is not supported.");

        public static readonly Error ImageSizeHeightNotSupported =
            new(nameof(ImageSizeHeightNotSupported), "Specified image height is not supported.");

        public static readonly Error ImageAspectRatioNotSupported =
            new(nameof(ImageAspectRatioNotSupported), "Specified image with given aspect ratio is not supported.");

        public static readonly Error ImageFormatNotSupported =
            new(nameof(ImageFormatNotSupported), "Specified image format is not supported.");

        public static readonly Error MimeTypeNotRecognized =
            new(nameof(MimeTypeNotRecognized), "Specified MIME type it not recognized.");

        public static readonly Error ImageDataNotValid =
            new(nameof(ImageDataNotValid), "Provided image is not valid.");

        public static readonly Error ImageObjectNameNotValid =
            new(nameof(ImageObjectNameNotValid), "Provided image object name us not valid.");
    }

    public static class ShortNameErrors
    {
        public static readonly Error InvalidShortName =
            new(nameof(InvalidShortName), "Specified value for short name is invalid.");

        public static readonly Error ShortNameTooLong =
            new(nameof(ShortNameTooLong), "Specified value for short name is too long.");
    }

    public static class NameErrors
    {
        public static readonly Error InvalidName =
            new(nameof(InvalidName), "Specified value for name is invalid.");

        public static readonly Error NameTooLong =
            new(nameof(NameTooLong), "Specified value for name is too long.");
    }

    public static class UserNameErrors
    {
        public static readonly Error InvalidUserName =
            new(nameof(InvalidUserName), "Specified value for user name is invalid.");

        public static readonly Error UserNameTooLong =
            new(nameof(UserNameTooLong), "Specified value for user name is too long.");
    }

    public static class EmailErrors
    {
        public static readonly Error InvalidEmail =
            new(nameof(InvalidEmail), "Specified value for email is invalid.");

        public static readonly Error EmailTooLong =
            new(nameof(EmailTooLong), "Specified value for email is too long.");
    }

    public static class PasswordErrors
    {
        public static readonly Error InvalidPassword =
            new(nameof(InvalidPassword), "Specified value for password is invalid.");

        public static readonly Error PasswordTooLong =
            new(nameof(PasswordTooLong), "Specified value for password is too long.");
    }

    public static class ObjectNameErrors
    {
        public static readonly Error InvalidObjectName =
            new(nameof(InvalidObjectName), "Specified value for object name is invalid.");

        public static readonly Error ObjectNameTooLong =
            new(nameof(ObjectNameTooLong), "Specified value for object name is too long.");
    }

    public static class TitleErrors
    {
        public static readonly Error InvalidTitle =
            new(nameof(InvalidTitle), "Specified value for title is invalid.");

        public static readonly Error TitleTooLong =
            new(nameof(TitleTooLong), "Specified value for title is too long.");
    }

    public static class SearchTermErrors
    {
        public static readonly Error InvalidSearchTerm =
            new(nameof(InvalidSearchTerm), "Specified value for search term is invalid.");

        public static readonly Error SearchTermTooLong =
            new(nameof(SearchTermTooLong), "Specified value for search term is too long.");
    }

    public static class DescriptionTextErrors
    {
        public static readonly Error InvalidDescriptionText =
            new(nameof(InvalidDescriptionText), "Specified value for description text is invalid.");

        public static readonly Error DescriptionTextTooLong =
            new(nameof(DescriptionTextTooLong), "Specified value for description text is too long.");
    }

    public static class CommentTextErrors
    {
        public static readonly Error InvalidCommentText =
            new(nameof(InvalidCommentText), "Specified value for comment text is invalid.");

        public static readonly Error CommentTextTooLong =
            new(nameof(CommentTextTooLong), "Specified value for comment text is too long.");
    }

    public static class WarningTagErrors
    {
        public static readonly Error InvalidWarningTagName =
            new(nameof(InvalidWarningTagName), "Specified name for WarningTag is invalid.");

        public static readonly Error WarningTagNotFound =
            new(nameof(WarningTagNotFound), "Could not find warningTag with given id.");

        public static readonly Error WarningTagAlreadyExists =
            new(nameof(WarningTagAlreadyExists), "A warning tag already exists with given name.");

        public static Error WarningTagNotFoundWithId(WarningTagId warningTagId) =>
            new(nameof(WarningTagNotFoundWithId), $"Could not find warningTag with given id of {warningTagId}.");
    }

    public static class TagErrors
    {
        public static readonly Error InvalidTagName =
            new(nameof(InvalidTagName), "Specified name for Tag is invalid.");

        public static readonly Error TagNotFound =
            new(nameof(TagNotFound), "Could not find tag with given id.");

        public static readonly Error TagAlreadyExists =
            new(nameof(TagAlreadyExists), "A tag already exists with given name.");

        public static Error TagNotFoundWithId(TagId tagId) =>
            new(nameof(TagNotFoundWithId), $"Could not find tag with given id of {tagId}.");
    }

    public static class PublisherErrors
    {
        public static readonly Error InvalidPublisherName =
            new(nameof(InvalidPublisherName), "Specified name for Publisher is invalid.");

        public static readonly Error PublisherNotFound =
            new(nameof(PublisherNotFound), "Could not find publisher with given id.");

        public static readonly Error PublisherAlreadyExists =
            new(nameof(PublisherAlreadyExists), "A publisher already exists with given name.");

        public static Error PublisherNotFoundWithId(PublisherId publisherId) =>
            new(nameof(PublisherNotFoundWithId), $"Could not find publisher with given id of {publisherId}.");
    }

    public static class LanguageErrors
    {
        public static readonly Error InvalidLanguageName =
            new(nameof(InvalidLanguageName), "Specified name for Language is invalid.");

        public static readonly Error LanguageNotFound =
            new(nameof(LanguageNotFound), "Could not find language with given id.");

        public static readonly Error LanguageAlreadyExists =
            new(nameof(LanguageAlreadyExists), "A language already exists with given name.");

        public static Error LanguageNotFoundWithId(LanguageId languageId) =>
            new(nameof(LanguageNotFoundWithId), $"Could not find language with given id of {languageId}.");
    }

    public static class GenreErrors
    {
        public static readonly Error InvalidGenreName =
            new(nameof(InvalidGenreName), "Specified name for Genre is invalid.");

        public static readonly Error GenreNotFound =
            new(nameof(GenreNotFound), "Could not find genre with given id.");

        public static readonly Error GenreAlreadyExists =
            new(nameof(GenreAlreadyExists), "A genre already exists with given name.");

        public static Error GenreNotFoundWithId(GenreId genreId) =>
            new(nameof(GenreNotFoundWithId), $"Could not find genre with given id of {genreId}.");
    }

    public static class LiteratureFormErrors
    {
        public static readonly Error InvalidLiteratureFormName =
            new(nameof(InvalidLiteratureFormName), "Specified name for LiteratureForm is invalid.");

        public static readonly Error InvalidScoreMultiplier =
            new(nameof(InvalidScoreMultiplier), "Specified score multiplier is outside of acceptable range.");

        public static readonly Error LiteratureFormNotFound =
            new(nameof(LiteratureFormNotFound), "Could not find literature form with given id.");

        public static readonly Error LiteratureFormAlreadyExists =
            new(nameof(LiteratureFormAlreadyExists), "A literature form already exists with given name.");

        public static Error LiteratureFormNotFoundWithId(LiteratureFormId literatureFormId) =>
            new(nameof(LiteratureFormNotFoundWithId),
                $"Could not find literature form with given id of {literatureFormId}.");
    }

    public static class FormatErrors
    {
        public static readonly Error InvalidFormatName =
            new(nameof(InvalidFormatName), "Specified name for Format is invalid.");

        public static readonly Error FormatNotFound =
            new(nameof(FormatNotFound), "Could not find format with given id.");

        public static readonly Error FormatAlreadyExists =
            new(nameof(FormatAlreadyExists), "A format already exists with given name.");

        public static Error FormatNotFoundWithId(FormatId formatId) =>
            new(nameof(FormatNotFoundWithId), $"Could not find format with given id of {formatId}.");
    }

    public static class LabelErrors
    {
        public static readonly Error InvalidLabelName =
            new(nameof(InvalidLabelName), "Specified name for Label is invalid.");

        public static readonly Error LabelNotFound =
            new(nameof(LabelNotFound), "Could not find label with given id.");

        public static readonly Error LabelAlreadyExists =
            new(nameof(LabelAlreadyExists), "A label already exists with given name.");

        public static Error LabelNotFoundWithId(LabelId labelId) =>
            new(nameof(LabelNotFoundWithId), $"Could not find label with given id of {labelId}.");
    }

    public static class NoteErrors
    {
        public static readonly Error InvalidNoteText =
            new(nameof(InvalidNoteText), "Specified text for Note is invalid.");

        public static readonly Error NoteNotFound =
            new(nameof(NoteNotFound), "Could not find note with given id.");

        public static Error NoteNotFoundWithId(NoteId noteId) =>
            new(nameof(NoteNotFoundWithId), $"Could not find note with given id of {noteId}.");
    }

    public static class CommentErrors
    {
        public static readonly Error InvalidCommentText =
            new(nameof(InvalidCommentText), "Specified text for Comment is invalid.");

        public static readonly Error CommentNotFound =
            new(nameof(CommentNotFound), "Could not find comment with given id.");

        public static Error CommentNotFoundWithId(CommentId commentId) =>
            new(nameof(CommentNotFoundWithId), $"Could not find comment with given id of {commentId}.");
    }

    public static class PostErrors
    {
        public static readonly Error InvalidPostText =
            new(nameof(InvalidPostText), "Specified text for Post is invalid.");

        public static readonly Error InvalidPostTitle =
            new(nameof(InvalidPostTitle), "Specified title for Post is invalid.");

        public static readonly Error InvalidPostId =
            new(nameof(InvalidPostId), "Specified id for Post is invalid.");

        public static readonly Error ProvidedCommentsNull =
            new(nameof(ProvidedCommentsNull), "Provided comments is null.");

        public static readonly Error ProvidedTitleNull =
            new(nameof(ProvidedTitleNull), "Provided title is null.");

        public static readonly Error PostNotFound =
            new(nameof(PostNotFound), "Could not find post with given id.");

        public static Error PostNotFoundWithId(PostId postId) =>
            new(nameof(PostNotFoundWithId), $"Could not find post with given id of {postId}.");
    }

    public static class ShelfErrors
    {
        public static readonly Error InvalidShelfName =
            new(nameof(InvalidShelfName), "Specified name for shelf is invalid.");

        public static readonly Error ProvidedBooksNull =
            new(nameof(ProvidedBooksNull), "Provided books is null.");

        public static readonly Error ShelfNotFound =
            new(nameof(ShelfNotFound), "Could not find shelf with given id.");

        public static readonly Error ShelfAlreadyExists =
            new(nameof(ShelfAlreadyExists), "A shelf already exists with given name.");

        public static Error ShelfNotFoundWithId(ShelfId shelfId) =>
            new(nameof(ShelfNotFoundWithId), $"Could not find shelf with given id of {shelfId}.");
    }

    public static class SeriesErrors
    {
        public static readonly Error InvalidSeriesTitle =
            new(nameof(InvalidSeriesTitle), "Specified title for Series is invalid.");

        public static readonly Error ProvidedBooksNull =
            new(nameof(ProvidedBooksNull), "Provided books is null.");

        public static readonly Error SeriesNotFound =
            new(nameof(SeriesNotFound), "Could not find series with given id.");

        public static Error SeriesNotFoundWithId(SeriesId seriesId) =>
            new(nameof(SeriesNotFoundWithId), $"Could not find series with given id of {seriesId}.");
    }

    public static class BookErrors
    {
        public static readonly Error InvalidBookTitle =
            new(nameof(InvalidBookTitle), "Specified title for Book is invalid.");

        public static readonly Error InvalidNumberInSeries =
            new(nameof(InvalidNumberInSeries), "Specified number in series for Book is invalid.");

        public static readonly Error ProvidedAuthorsNull =
            new(nameof(ProvidedAuthorsNull), "Provided authors is null.");

        public static readonly Error ProvidedGenresNull =
            new(nameof(ProvidedGenresNull), "Provided genres is null.");

        public static readonly Error ProvidedTagsNull =
            new(nameof(ProvidedTagsNull), "Provided tags is null.");

        public static readonly Error ProvidedWarningTagsNull =
            new(nameof(ProvidedWarningTagsNull), "Provided warning tags is null.");

        public static readonly Error ProvidedBookEditionsNull =
            new(nameof(ProvidedBookEditionsNull), "Provided book editions is null.");

        public static readonly Error BookNotFound =
            new(nameof(BookNotFound), "Could not find book with given id.");

        public static Error BookNotFoundWithId(BookId bookId) =>
            new(nameof(BookNotFoundWithId), $"Could not find book with given id of {bookId}.");
    }

    public static class BookEditionErrors
    {
        public static readonly Error InvalidBookEditionTitle =
            new(nameof(InvalidBookEditionTitle), "Specified title for BookEdition is invalid.");

        public static readonly Error ProvidedReadingsNull =
            new(nameof(ProvidedReadingsNull), "Provided readings is null.");

        public static readonly Error BookEditionNotFound =
            new(nameof(BookEditionNotFound), "Could not find bookEdition with given id.");

        public static Error BookEditionNotFoundWithId(BookEditionId bookEditionId) =>
            new(nameof(BookEditionNotFoundWithId), $"Could not find bookEdition with given id of {bookEditionId}.");
    }

    public static class PageCountErrors
    {
        public static readonly Error InvalidPageCount =
            new(nameof(InvalidPageCount), "Specified page count is invalid.");
    }

    public static class WordCountErrors
    {
        public static readonly Error InvalidWordCount =
            new(nameof(InvalidWordCount), "Specified word count is invalid.");
    }

    public static class ScoreMultiplierErrors
    {
        public static readonly Error InvalidScoreMultiplier =
            new(nameof(InvalidScoreMultiplier), "Specified score multiplier is invalid.");
    }

    public static class SortOrderErrors
    {
        public static readonly Error InvalidSortOrder =
            new(nameof(InvalidSortOrder), "Specified sort order is invalid.");
    }

    public static class ReadingErrors
    {
        public static readonly Error StartFinishDateTimeError =
            new(nameof(StartFinishDateTimeError), "Specified startedOnUtc is greater than finishedOnUtc");

        public static readonly Error ProvidedBookEditionNull =
            new(nameof(ProvidedBookEditionNull), "Provided book edition is null.");

        public static readonly Error ProvidedLiteratureFormNull =
            new(nameof(ProvidedLiteratureFormNull), "Provided literature form is null.");

        public static readonly Error ReadingNotFound =
            new(nameof(ReadingNotFound), "Could not find reading with given id.");

        public static Error ReadingNotFoundWithId(ReadingId readingId) =>
            new(nameof(ReadingNotFoundWithId), $"Could not find reading with given id of {readingId}.");
    }

    public static class AuthorErrors
    {
        public static readonly Error InvalidAuthorName =
            new(nameof(InvalidAuthorName), "Specified name for Author is invalid.");

        public static readonly Error AuthorNotFound =
            new(nameof(AuthorNotFound), "Could not find author with given id.");

        public static Error AuthorNotFoundWithId(AuthorId authorId) =>
            new(nameof(AuthorNotFoundWithId), $"Could not find author with given id of {authorId}.");
    }

    public static class GroupErrors
    {
        public static readonly Error InvalidGroupName =
            new(nameof(InvalidGroupName), "Specified name for Group is invalid.");

        public static readonly Error ProvidedShelvesNull =
            new(nameof(ProvidedShelvesNull), "Provided shelves is null.");

        public static readonly Error ProvidedLabelsNull =
            new(nameof(ProvidedLabelsNull), "Provided labels is null.");

        public static readonly Error ProvidedPostsNull =
            new(nameof(ProvidedPostsNull), "Provided posts is null.");

        public static readonly Error GroupNotFound =
            new(nameof(GroupNotFound), "Could not find group with given id.");

        public static Error GroupNotFoundWithId(GroupId groupId) =>
            new(nameof(GroupNotFoundWithId), $"Could not find group with given id of {groupId}.");
    }

    public static class UserErrors
    {
        public static readonly Error UserNotFound =
            new(nameof(UserNotFound), "Could not find user with given id.");

        public static readonly Error UserLoginRequiresTwoFactor =
            new(nameof(UserLoginRequiresTwoFactor), "User login action requires two factor authentication.");

        public static readonly Error UserIsLockedOut =
            new(nameof(UserIsLockedOut), "User is locked out.");

        public static readonly Error UserLoginIsNotAllowed =
            new(nameof(UserLoginIsNotAllowed), "User login is not allowed.");

        public static readonly Error TwoFactorCodeNotValid =
            new(nameof(TwoFactorCodeNotValid), "Provided two factor authentication code is not valid.");

        public static readonly Error TwoFactorIsDisabled =
            new(nameof(TwoFactorIsDisabled), "Two factor authentication is disabled.");

        public static readonly Error RecoveryCodeNotValid =
            new(nameof(RecoveryCodeNotValid), "Provided recovery code is not valid.");

        public static readonly Error UserEmailNotConfirmed =
            new(nameof(UserEmailNotConfirmed), "User email is not confirmed.");

        public static readonly Error UserEmailTaken =
            new(nameof(UserEmailTaken), "User email is already taken.");

        public static Error UserNotFoundWithId(UserId userId) =>
            new(nameof(UserNotFoundWithId), $"Could not find user with given id of {userId}.");
    }

    public static class InvitationErrors
    {
        public static readonly Error InvitationNotFound =
            new(nameof(InvitationNotFound), "Could not find invitation with given id.");

        public static readonly Error InvitationAlreadyExists =
            new(nameof(InvitationAlreadyExists), "Invitation already exists.");

        public static readonly Error CountNotCreateInvitation =
            new(nameof(CountNotCreateInvitation), "Could not create invitation.");

        public static Error InvitationNotFoundWithId(InvitationId invitationId) =>
            new(nameof(InvitationNotFoundWithId), $"Could not find invitation with given id of {invitationId}.");
    }

    public static class CoverImageErrors
    {
        public static readonly Error CoverImageNotFound =
            new(nameof(CoverImageNotFound), "Could not find cover image.");
    }

    public static class ProfilePictureErrors
    {
        public static readonly Error ProfilePictureNotFound =
            new(nameof(ProfilePictureNotFound), "Could not find profile picture.");
    }

    public static class GroupRoleErrors
    {
        public static readonly Error InvalidGroupRole =
            new(nameof(InvalidGroupRole), "Specified group role is invalid.");
    }

    public static class StampedEntityErrors
    {
        public static readonly Error CreationModificationDateTimeError =
            new(nameof(CreationModificationDateTimeError), "Specified createdOnUtc is greater than modifiedOnUtc.");
    }

    public static class NamedEntityErrors
    {
        public static readonly Error InvalidName =
            new(nameof(InvalidName), "Specified name is invalid.");
    }

    public static class TitledEntityErrors
    {
        public static readonly Error InvalidTitle =
            new(nameof(InvalidTitle), "Specified title is invalid.");
    }

    public static class IsbnErrors
    {
        public static readonly Error InvalidIsbn =
            new(nameof(InvalidIsbn), "Specified ISBN is invalid.");
    }
}