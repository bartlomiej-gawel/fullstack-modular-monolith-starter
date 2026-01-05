using Common.Abstractions.Domain;
using Common.Abstractions.Domain.ValueObjects;
using ErrorOr;

namespace Modules.Communications.Domain.Recipients;

public sealed class Recipient : AggregateRoot<RecipientId>
{
    private const int MinFirstNameLength = 2;
    private const int MaxFirstNameLength = 100;
    private const int MinLastNameLength = 2;
    private const int MaxLastNameLength = 100;

    private Recipient()
    {
    }

    private Recipient(
        RecipientId id,
        string firstName,
        string lastName,
        Email email,
        Phone phone,
        RecipientStatus status,
        RecipientType type) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
        Preferences = Preferences.CreateDefault();
        Status = status;
        Type = type;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Phone Phone { get; private set; } = null!;
    public Preferences Preferences { get; private set; } = null!;
    public RecipientStatus Status { get; private set; }
    public RecipientType Type { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public static ErrorOr<Recipient> CreateBackofficeRecipient(
        Guid userId,
        string firstName,
        string lastName,
        Email email,
        Phone phone)
    {
        var firstNameValidationResult = ValidateFirstName(firstName);
        if (firstNameValidationResult.IsError)
            return firstNameValidationResult.Errors;

        var lastNameValidationResult = ValidateLastName(lastName);
        if (lastNameValidationResult.IsError)
            return lastNameValidationResult.Errors;

        var recipient = new Recipient(
            RecipientId.CreateFrom(userId),
            firstName,
            lastName,
            email,
            phone,
            RecipientStatus.Inactive,
            RecipientType.Backoffice);

        return recipient;
    }

    private static ErrorOr<Success> ValidateFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Errors.InvalidFirstName;

        if (firstName.Length is < MinFirstNameLength or > MaxFirstNameLength)
            return Errors.InvalidFirstNameLength;

        return Result.Success;
    }

    private static ErrorOr<Success> ValidateLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return Errors.InvalidLastName;

        if (lastName.Length is < MinLastNameLength or > MaxLastNameLength)
            return Errors.InvalidLastNameLength;

        return Result.Success;
    }

    private static class Errors
    {
        public static readonly Error InvalidFirstName = Error.Validation(
            "Communications.Recipient.InvalidFirstName",
            "Recipient first name cannot be null or empty.");

        public static readonly Error InvalidFirstNameLength = Error.Validation(
            "Communications.Recipient.InvalidFirstNameLength",
            "Recipient first name must be between 2 and 100 characters.");

        public static readonly Error InvalidLastName = Error.Validation(
            "Communications.Recipient.InvalidLastName",
            "Recipient last name cannot be null or empty.");

        public static readonly Error InvalidLastNameLength = Error.Validation(
            "Communications.Recipient.InvalidLastNameLength",
            "Recipient last name must be between 2 and 100 characters.");
    }
}