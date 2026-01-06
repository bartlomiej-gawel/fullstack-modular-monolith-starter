using Common.Abstractions.Domain;
using Common.Abstractions.Domain.ValueObjects;
using ErrorOr;
using Modules.Identity.Domain.Users.Events;

namespace Modules.Identity.Domain.Users;

public sealed class User : AggregateRoot<UserId>
{
    private const int MinFirstNameLength = 2;
    private const int MaxFirstNameLength = 100;
    private const int MinLastNameLength = 2;
    private const int MaxLastNameLength = 100;

    private User()
    {
    }

    private User(
        UserId id,
        string firstName,
        string lastName,
        Email email,
        Phone phone,
        UserStatus status,
        UserRole role,
        EmailVerificationToken? emailVerificationToken = null,
        Password? password = null) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        EmailVerificationToken = emailVerificationToken;
        Password = password;
        PasswordResetToken = null;
        Phone = phone;
        Status = status;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public EmailVerificationToken? EmailVerificationToken { get; private set; }
    public Password? Password { get; private set; }
    public PasswordResetToken? PasswordResetToken { get; private set; }
    public Phone Phone { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    public static ErrorOr<User> CreateBackofficeUser(
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

        var user = new User(
            UserId.Create(),
            firstName,
            lastName,
            email,
            phone,
            UserStatus.Inactive,
            UserRole.Backoffice,
            EmailVerificationToken.Generate());

        user.AddDomainEvent(new UserCreatedDomainEvent(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.Status,
            user.Role,
            user.EmailVerificationToken));

        return user;
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
            "Identity.User.InvalidFirstName",
            "User first name cannot be null or empty.");

        public static readonly Error InvalidFirstNameLength = Error.Validation(
            "Identity.User.InvalidFirstNameLength",
            "User first name must be between 2 and 100 characters.");

        public static readonly Error InvalidLastName = Error.Validation(
            "Identity.User.InvalidLastName",
            "User last name cannot be null or empty.");

        public static readonly Error InvalidLastNameLength = Error.Validation(
            "Identity.User.InvalidLastNameLength",
            "User last name must be between 2 and 100 characters.");
    }
}