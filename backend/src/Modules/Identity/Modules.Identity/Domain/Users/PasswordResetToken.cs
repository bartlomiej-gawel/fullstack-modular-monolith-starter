using System.Security.Cryptography;
using Common.Abstractions.Domain;
using ErrorOr;

namespace Modules.Identity.Domain.Users;

public sealed class PasswordResetToken : ValueObject
{
    private const int TokenLength = 32;
    private const int TokenLifetimeInHours = 1;

    private PasswordResetToken()
    {
    }

    private PasswordResetToken(
        string value,
        DateTime createdAt,
        DateTime expiresAt)
    {
        Value = value;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public string Value { get; } = null!;
    public DateTime CreatedAt { get; }
    public DateTime ExpiresAt { get; }

    public static PasswordResetToken Generate()
    {
        var currentDateTime = DateTime.UtcNow;
        var tokenBytes = RandomNumberGenerator.GetBytes(TokenLength);
        var tokenValue = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        return new PasswordResetToken(
            tokenValue,
            currentDateTime,
            currentDateTime.AddHours(TokenLifetimeInHours));
    }

    public ErrorOr<Success> Validate(string token)
    {
        if (!string.Equals(Value, token, StringComparison.Ordinal))
            return Errors.Invalid;

        if (IsExpired())
            return Errors.Expired;

        return Result.Success;
    }

    public bool IsExpired() => DateTime.UtcNow > ExpiresAt;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
        yield return CreatedAt;
        yield return ExpiresAt;
    }

    private static class Errors
    {
        public static readonly Error Expired = Error.Failure(
            "Identity.PasswordResetToken.Expired",
            "Password reset token has expired.");

        public static readonly Error Invalid = Error.Failure(
            "Identity.PasswordResetToken.Invalid",
            "Password reset token is invalid.");
    }
}