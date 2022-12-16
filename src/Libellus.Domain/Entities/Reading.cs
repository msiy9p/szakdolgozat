using Ardalis.GuardClauses;
using Libellus.Domain.Common.Interfaces.Services;
using Libellus.Domain.Common.Models;
using Libellus.Domain.Common.Types;
using Libellus.Domain.Common.Types.Ids;
using Libellus.Domain.Exceptions;
using Libellus.Domain.Models;
using Libellus.Domain.ViewModels;
using NodaTime;
using static Libellus.Domain.Errors.DomainErrors;

namespace Libellus.Domain.Entities;

public sealed class Reading : BaseStampedEntity<ReadingId>, IComparable, IComparable<Reading>, IEquatable<Reading>
{
    public static readonly Duration ExpireAfterDays = Duration.FromDays(31);

    private const int MinimumDaysRead = 1;
    private const decimal ScoreExtra = 3.5m;
    private const int AverageWordsPerPage = 300;

    private readonly PageCount? _pageCount;
    private readonly WordCount? _wordCount;
    private readonly ScoreMultiplier? _scoreMultiplier;

    public ReadingFriendlyId FriendlyId { get; init; }
    public UserId CreatorId => Creator.UserId;
    public UserPicturedVm Creator { get; private set; }

    public BookEditionId BookEditionId => BookEdition.BookEditionId;
    public BookEditionCompactVm BookEdition { get; init; }
    public Note? Note { get; private set; }

    public bool IsDnf { get; private set; }
    public bool IsReread { get; private set; }
    public double? Score { get; private set; }

    public ZonedDateTime? StartedOnUtc { get; private set; }
    public ZonedDateTime? FinishedOnUtc { get; private set; }

    internal Reading(ReadingId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        ReadingFriendlyId friendlyId, UserPicturedVm creator, BookEdition bookEdition, LiteratureForm? literatureForm,
        Note? note, bool isDnf, bool isReread, ZonedDateTime? startedOnUtc, ZonedDateTime? finishedOnUtc) : base(id,
        createdOnUtc, modifiedOnUtc)
    {
        Guard.Against.Null(bookEdition);
        Guard.Against.Null(literatureForm);

        FriendlyId = friendlyId;
        Creator = Guard.Against.Null(creator);
        Note = note;
        IsDnf = isDnf;
        IsReread = isReread;

        BookEdition = BookEditionCompactVm.Convert(bookEdition);

        if (startedOnUtc.HasValue && finishedOnUtc.HasValue &&
            !Utilities.Utilities.DoesPrecede(startedOnUtc.Value, finishedOnUtc.Value))
        {
            throw new CreationModificationDateTimeException("Started date is bigger then finished date.");
        }

        if (!startedOnUtc.HasValue)
        {
            StartedOnUtc = null;
            FinishedOnUtc = null;
        }
        else
        {
            StartedOnUtc = startedOnUtc;
            FinishedOnUtc = finishedOnUtc;
        }

        _pageCount = bookEdition.PageCount;
        _wordCount = bookEdition.WordCount;
        _scoreMultiplier = literatureForm?.ScoreMultiplier;

        RefreshScore();
    }

    internal Reading(ReadingId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        ReadingFriendlyId friendlyId, UserPicturedVm creator, BookEditionCompactVm bookEdition, PageCount? pageCount,
        ScoreMultiplier? scoreMultiplier, Note? note, bool isDnf, bool isReread, ZonedDateTime? startedOnUtc,
        ZonedDateTime? finishedOnUtc) : base(id, createdOnUtc, modifiedOnUtc)
    {
        FriendlyId = friendlyId;
        Creator = Guard.Against.Null(creator);
        Note = note;
        IsDnf = isDnf;
        IsReread = isReread;

        BookEdition = Guard.Against.Null(bookEdition);

        if (startedOnUtc.HasValue && finishedOnUtc.HasValue &&
            !Utilities.Utilities.DoesPrecede(startedOnUtc.Value, finishedOnUtc.Value))
        {
            throw new CreationModificationDateTimeException("Started date is bigger then finished date.");
        }

        if (!startedOnUtc.HasValue)
        {
            StartedOnUtc = null;
            FinishedOnUtc = null;
        }
        else
        {
            StartedOnUtc = startedOnUtc;
            FinishedOnUtc = finishedOnUtc;
        }

        _pageCount = pageCount;
        _wordCount = null;
        _scoreMultiplier = scoreMultiplier;

        RefreshScore();
    }

    public static Result<Reading> Create(ReadingId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        ReadingFriendlyId friendlyId, UserPicturedVm creator, BookEdition bookEdition, LiteratureForm? literatureForm,
        Note? noteId, bool isDnf, bool isReread, ZonedDateTime? startedOnUtc, ZonedDateTime? finishedOnUtc)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Reading>.Invalid(result.Errors);
        }

        if (creator is null)
        {
            return Result<Reading>.Invalid(GeneralErrors.InputIsNull);
        }

        if (startedOnUtc.HasValue && finishedOnUtc.HasValue &&
            !Utilities.Utilities.DoesPrecede(startedOnUtc!.Value, finishedOnUtc!.Value))
        {
            return Result<Reading>.Invalid(ReadingErrors.StartFinishDateTimeError);
        }

        if (bookEdition is null)
        {
            return Result<Reading>.Invalid(ReadingErrors.ProvidedBookEditionNull);
        }

        if (literatureForm is null)
        {
            return Result<Reading>.Invalid(ReadingErrors.ProvidedLiteratureFormNull);
        }

        return Result<Reading>.Success(new Reading(id, createdOnUtc, modifiedOnUtc, friendlyId, creator, bookEdition,
            literatureForm, noteId, isDnf, isReread, startedOnUtc, finishedOnUtc));
    }

    public static Result<Reading> Create(ReadingId id, ZonedDateTime createdOnUtc, ZonedDateTime modifiedOnUtc,
        ReadingFriendlyId friendlyId, UserPicturedVm creator, BookEditionCompactVm bookEdition, PageCount? pageCount,
        ScoreMultiplier? scoreMultiplier, Note? noteId, bool isDnf, bool isReread, ZonedDateTime? startedOnUtc,
        ZonedDateTime? finishedOnUtc)
    {
        var result = Create(id, createdOnUtc, modifiedOnUtc);
        if (result.IsError)
        {
            return Result<Reading>.Invalid(result.Errors);
        }

        if (creator is null)
        {
            return Result<Reading>.Invalid(GeneralErrors.InputIsNull);
        }

        if (startedOnUtc.HasValue && finishedOnUtc.HasValue &&
            !Utilities.Utilities.DoesPrecede(startedOnUtc!.Value, finishedOnUtc!.Value))
        {
            return Result<Reading>.Invalid(ReadingErrors.StartFinishDateTimeError);
        }

        return Result<Reading>.Success(new Reading(id, createdOnUtc, modifiedOnUtc, friendlyId, creator,
            bookEdition, pageCount, scoreMultiplier, noteId, isDnf, isReread, startedOnUtc, finishedOnUtc));
    }

    private void RefreshScore()
    {
        var temp = _wordCount.HasValue
            ? CalculateReadingScoreWithWordCount(GetDaysRead(), _wordCount, _scoreMultiplier)
            : CalculateReadingScoreWithPageCount(GetDaysRead(), _pageCount, _scoreMultiplier);

        Score = temp.HasValue ? Math.Round(temp.Value, 3, MidpointRounding.AwayFromZero) : temp;
    }

    public int? GetDaysRead()
    {
        if (!StartedOnUtc.HasValue)
        {
            return null;
        }

        if (!FinishedOnUtc.HasValue)
        {
            return null;
        }

        var duration = FinishedOnUtc.Value - StartedOnUtc.Value;
        return duration.Days;
    }

    public bool MarkAsDnf(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (IsDnf)
        {
            return false;
        }

        IsDnf = true;
        FinishedOnUtc = null;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsNotDnf(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsDnf)
        {
            return false;
        }

        IsDnf = false;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeDnf(bool value, IDateTimeProvider dateTimeProvider)
        => value ? MarkAsDnf(dateTimeProvider) : MarkAsNotDnf(dateTimeProvider);

    public bool MarkAsReread(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (IsReread)
        {
            return false;
        }

        IsReread = true;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool MarkAsNotReread(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (!IsReread)
        {
            return false;
        }

        IsReread = false;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool ChangeReread(bool value, IDateTimeProvider dateTimeProvider)
        => value ? MarkAsReread(dateTimeProvider) : MarkAsNotReread(dateTimeProvider);

    public bool RemoveStartedOnUtc(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        StartedOnUtc = null;
        FinishedOnUtc = null;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool StartReading(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        StartedOnUtc = dateTimeProvider.UtcNow;
        FinishedOnUtc = null;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool StartReading(ZonedDateTime zonedDateTime, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        StartedOnUtc = zonedDateTime;
        FinishedOnUtc = null;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveFinishedOnUtc(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        FinishedOnUtc = null;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool FinishReading(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        var datetime = dateTimeProvider.UtcNow;

        if (StartedOnUtc.HasValue && !Utilities.Utilities.DoesPrecede(StartedOnUtc.Value, datetime))
        {
            return false;
        }

        FinishedOnUtc = datetime;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool FinishReading(ZonedDateTime zonedDateTime, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (StartedOnUtc.HasValue && !Utilities.Utilities.DoesPrecede(StartedOnUtc.Value, zonedDateTime))
        {
            return false;
        }

        FinishedOnUtc = zonedDateTime;
        RefreshScore();

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool RemoveNote(IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        Note = null;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public bool AddNote(Note note, IDateTimeProvider dateTimeProvider)
    {
        if (dateTimeProvider is null)
        {
            return false;
        }

        if (note is null)
        {
            return false;
        }

        Note = note;

        UpdateModifiedOnUtc(dateTimeProvider);
        return true;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public bool Equals(Reading? other)
    {
        if (other is null)
        {
            return false;
        }

        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        return obj is Reading value && Equals(value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is Reading author)
        {
            return CompareTo(author);
        }

        return 1;
    }

    public int CompareTo(Reading? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        return Id.CompareTo(other.Id);
    }

    public static bool operator ==(Reading left, Reading right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Reading left, Reading right) => !(left == right);

    public static bool operator <(Reading left, Reading right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    public static bool operator <=(Reading left, Reading right) => left is null || left.CompareTo(right) <= 0;

    public static bool operator >(Reading left, Reading right) => left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(Reading left, Reading right) =>
        left is null ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;

    public static double? CalculateReadingScoreWithPageCount(int? daysRead, PageCount? pageCount,
        ScoreMultiplier? scoreMultiplier)
    {
        if (daysRead is null || pageCount is null || !scoreMultiplier.HasValue)
        {
            return null;
        }

        if (daysRead < MinimumDaysRead)
        {
            return null;
        }

        return CalculateReadingScoreWithWordCount(daysRead, new WordCount(pageCount.Value * AverageWordsPerPage),
            scoreMultiplier);
    }

    public static double? CalculateReadingScoreWithWordCount(int? daysRead, WordCount? wordCount,
        ScoreMultiplier? scoreMultiplier)
    {
        if (daysRead is null || wordCount is null || !scoreMultiplier.HasValue)
        {
            return null;
        }

        if (daysRead < MinimumDaysRead)
        {
            return null;
        }

        return (double)wordCount!.Value /
               (Math.Pow((double)daysRead!.Value, (double)scoreMultiplier.Value.Value!) + (double)ScoreExtra);
    }

    public class CompareByStartedOnUtc : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.StartedOnUtc.HasValue && !y!.StartedOnUtc.HasValue)
            {
                result = 0;
            }
            else if (x!.StartedOnUtc.HasValue && !y!.StartedOnUtc.HasValue)
            {
                result = 1;
            }
            else if (!x!.StartedOnUtc.HasValue && y!.StartedOnUtc.HasValue)
            {
                result = -1;
            }
            else
            {
                result = ZonedDateTime.Comparer.Instant.Compare(x!.StartedOnUtc!.Value, y!.StartedOnUtc!.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return new CompareByCreatedOnUtc().Compare(x, y);
        }
    }

    public class CompareByStartedOnUtcDesc : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.StartedOnUtc.HasValue && !y!.StartedOnUtc.HasValue)
            {
                result = 0;
            }
            else if (x!.StartedOnUtc.HasValue && !y!.StartedOnUtc.HasValue)
            {
                result = 1;
            }
            else if (!x!.StartedOnUtc.HasValue && y!.StartedOnUtc.HasValue)
            {
                result = -1;
            }
            else
            {
                result = ZonedDateTime.Comparer.Instant.Compare(x!.StartedOnUtc!.Value, y!.StartedOnUtc!.Value);
            }

            if (result != 0)
            {
                return result * -1;
            }

            return new CompareByCreatedOnUtcDesc().Compare(x, y);
        }
    }

    public class CompareByFinishedOnUtc : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.FinishedOnUtc.HasValue && !y!.FinishedOnUtc.HasValue)
            {
                result = 0;
            }
            else if (x!.FinishedOnUtc.HasValue && !y!.FinishedOnUtc.HasValue)
            {
                result = 1;
            }
            else if (!x!.FinishedOnUtc.HasValue && y!.FinishedOnUtc.HasValue)
            {
                result = -1;
            }
            else
            {
                result = ZonedDateTime.Comparer.Instant.Compare(x!.FinishedOnUtc!.Value, y!.FinishedOnUtc!.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return new CompareByStartedOnUtc().Compare(x, y);
        }
    }

    public class CompareByFinishedOnUtcDesc : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.FinishedOnUtc.HasValue && !y!.FinishedOnUtc.HasValue)
            {
                result = 0;
            }
            else if (x!.FinishedOnUtc.HasValue && !y!.FinishedOnUtc.HasValue)
            {
                result = 1;
            }
            else if (!x!.FinishedOnUtc.HasValue && y!.FinishedOnUtc.HasValue)
            {
                result = -1;
            }
            else
            {
                result = ZonedDateTime.Comparer.Instant.Compare(x!.FinishedOnUtc!.Value, y!.FinishedOnUtc!.Value);
            }

            if (result != 0)
            {
                return result * -1;
            }

            return new CompareByStartedOnUtcDesc().Compare(x, y);
        }
    }

    public class CompareByScore : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.Score.HasValue && !y!.Score.HasValue)
            {
                result = 0;
            }
            else if (x!.Score.HasValue && !y!.Score.HasValue)
            {
                result = 1;
            }
            else if (!x!.Score.HasValue && y!.Score.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.Score!.Value.CompareTo(y!.Score!.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return new CompareByStartedOnUtc().Compare(x, y);
        }
    }

    public class CompareByScoreDesc : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int result;
            if (!x!.Score.HasValue && !y!.Score.HasValue)
            {
                result = 0;
            }
            else if (x!.Score.HasValue && !y!.Score.HasValue)
            {
                result = 1;
            }
            else if (!x!.Score.HasValue && y!.Score.HasValue)
            {
                result = -1;
            }
            else
            {
                result = x.Score!.Value.CompareTo(y!.Score!.Value);
            }

            if (result != 0)
            {
                return result * -1;
            }

            return new CompareByStartedOnUtcDesc().Compare(x, y);
        }
    }

    public class CompareByDaysRead : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int? xDaysRead = x!.GetDaysRead();
            int? yDaysRead = y!.GetDaysRead();

            int result;
            if (!xDaysRead.HasValue && !yDaysRead.HasValue)
            {
                result = 0;
            }
            else if (xDaysRead.HasValue && !yDaysRead.HasValue)
            {
                result = 1;
            }
            else if (!xDaysRead.HasValue && yDaysRead.HasValue)
            {
                result = -1;
            }
            else
            {
                result = xDaysRead!.Value.CompareTo(yDaysRead!.Value);
            }

            if (result != 0)
            {
                return result;
            }

            return new CompareByStartedOnUtc().Compare(x, y);
        }
    }

    public class CompareByDaysReadDesc : Comparer<Reading>
    {
        public override int Compare(Reading? x, Reading? y)
        {
            if (x is null && y is null)
            {
                return 0;
            }

            if (x is not null && y is null)
            {
                return 1;
            }

            if (x is null && y is not null)
            {
                return -1;
            }

            int? xDaysRead = x!.GetDaysRead();
            int? yDaysRead = y!.GetDaysRead();

            int result;
            if (!xDaysRead.HasValue && !yDaysRead.HasValue)
            {
                result = 0;
            }
            else if (xDaysRead.HasValue && !yDaysRead.HasValue)
            {
                result = 1;
            }
            else if (!xDaysRead.HasValue && yDaysRead.HasValue)
            {
                result = -1;
            }
            else
            {
                result = xDaysRead!.Value.CompareTo(yDaysRead!.Value);
            }

            if (result != 0)
            {
                return result * -1;
            }

            return new CompareByStartedOnUtcDesc().Compare(x, y);
        }
    }
}