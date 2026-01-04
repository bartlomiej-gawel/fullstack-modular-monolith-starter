using ErrorOr;
using PhoneNumbers;

namespace Common.Abstractions.Domain.ValueObjects;

public sealed class Phone : ValueObject
{
    private const int MinPrefixLength = 1;
    private const int MaxPrefixLength = 4;
    private const int MinNumberLength = 4;
    private const int MaxNumberLength = 15;

    private Phone()
    {
    }

    private Phone(string prefix, string number)
    {
        Prefix = prefix;
        Number = number;
    }

    public string Prefix { get; } = null!;
    public string Number { get; } = null!;

    public static ErrorOr<Phone> Create(string prefix, string number)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            return Errors.InvalidPrefix;

        if (string.IsNullOrWhiteSpace(number))
            return Errors.InvalidNumber;

        var cleanPrefix = prefix.TrimStart('+');
        if (cleanPrefix.Length is < MinPrefixLength or > MaxPrefixLength)
            return Errors.InvalidPrefixLength;

        if (number.Length is < MinNumberLength or > MaxNumberLength)
            return Errors.InvalidNumberLength;

        try
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            var fullNumber = $"+{cleanPrefix}{number}";
            var phoneNumber = phoneNumberUtil.Parse(fullNumber, null);

            if (!cleanPrefix.Equals(phoneNumber.CountryCode.ToString()))
                return Errors.InvalidCountryCode;

            if (!phoneNumberUtil.IsValidNumber(phoneNumber))
                return Errors.InvalidPhone;

            return new Phone(prefix, number);
        }
        catch (Exception)
        {
            return Errors.InvalidNumber;
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Prefix;
        yield return Number;
    }

    private static class Errors
    {
        public static readonly Error InvalidPrefix = Error.Failure(
            "Common.Phone.InvalidPrefix",
            "Phone prefix cannot be null or empty.");

        public static readonly Error InvalidPrefixLength = Error.Failure(
            "Common.Phone.InvalidPrefixLength",
            "Phone prefix must be between 1 and 4 characters.");

        public static readonly Error InvalidNumber = Error.Failure(
            "Common.Phone.InvalidNumber",
            "Phone number cannot be null or empty.");

        public static readonly Error InvalidNumberLength = Error.Failure(
            "Common.Phone.InvalidNumberLength",
            "Phone number must be between 4 and 15 characters.");

        public static readonly Error InvalidCountryCode = Error.Failure(
            "Common.Phone.InvalidCountryCode",
            "Phone prefix does not match the country code");

        public static readonly Error InvalidPhone = Error.Failure(
            "Common.Phone.InvalidPhone",
            "Phone is invalid.");
    }
}
