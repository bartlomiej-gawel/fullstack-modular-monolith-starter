using System.Text.RegularExpressions;
using ErrorOr;

namespace Common.Abstractions.Domain.ValueObjects;

public sealed partial class Email : ValueObject
{
    private const int MinLength = 3;
    private const int MaxLength = 254;

    [GeneratedRegex(@"^[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+(?:\.[a-zA-Z0-9!#$%&'*+\-/=?^_`{|}~]+)*@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    private static bool IsValid(string value) => EmailRegex().IsMatch(value);

    private Email(string value) => Value = value;

    public string Value { get; }

    public static ErrorOr<Email> Create(string value)
    {
        if (value.Length is < MinLength or > MaxLength)
            return Errors.InvalidLength;

        if (!IsValid(value))
            return Errors.Invalid;

        return new Email(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static class Errors
    {
        public static readonly Error Invalid = Error.Failure(
            "Common.Email.Invalid",
            "Email is invalid.");

        public static readonly Error InvalidLength = Error.Failure(
            "Common.Email.InvalidLength",
            "Email must be between 3 and 254 characters.");
    }
}
