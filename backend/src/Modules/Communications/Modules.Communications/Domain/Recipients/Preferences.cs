using Common.Abstractions.Domain;
using ErrorOr;

namespace Modules.Communications.Domain.Recipients;

public sealed class Preferences : ValueObject
{
    private Preferences()
    {
    }

    private Preferences(
        bool emailEnabled,
        bool smsEnabled,
        bool inAppEnabled)
    {
        EmailEnabled = emailEnabled;
        SmsEnabled = smsEnabled;
        InAppEnabled = inAppEnabled;
    }

    public bool EmailEnabled { get; }
    public bool SmsEnabled { get; }
    public bool InAppEnabled { get; }

    public static Preferences CreateDefault()
    {
        return new Preferences(
            emailEnabled: true,
            smsEnabled: true,
            inAppEnabled: true);
    }

    public static ErrorOr<Preferences> Create(
        bool emailEnabled,
        bool smsEnabled,
        bool inAppEnabled)
    {
        if (!emailEnabled && !smsEnabled && !inAppEnabled)
            return Errors.AtLeastOneChannelRequired;

        return new Preferences(
            emailEnabled,
            smsEnabled,
            inAppEnabled);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return EmailEnabled;
        yield return SmsEnabled;
        yield return InAppEnabled;
    }

    private static class Errors
    {
        public static readonly Error AtLeastOneChannelRequired = Error.Validation(
            "Communications.Preferences.AtLeastOneChannelRequired",
            "At least one communication channel must be enabled in recipient preferences.");
    }
}