using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Common.Abstractions.Domain;
using ErrorOr;

namespace Modules.Identity.Domain.Users;

public sealed partial class Password : ValueObject
{
    private const int MinLength = 8;
    private const int MaxLength = 128;

    [GeneratedRegex(@"^(?=.*?[0-9])(?=.*?[A-Z])(?=.*\W)(?!.* ).{8,}$", RegexOptions.Compiled)]
    private static partial Regex PasswordRegex();

    private static bool IsValid(string value) => PasswordRegex().IsMatch(value);

    private Password(string value) => Value = value;

    public string Value { get; }

    public static ErrorOr<Password> Create(string value)
    {
        if (value.Length is < MinLength or > MaxLength)
            return Errors.InvalidLength;

        if (!IsValid(value))
            return Errors.Invalid;

        return new Password(Hasher.Hash(value));
    }

    public static Password CreateFromHash(string hash) => new(hash);

    public bool Verify(string input) => Hasher.Verify(input, Value);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    private static class Hasher
    {
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public static bool Verify(string password, string hashedPassword)
        {
            var parts = hashedPassword.Split('-');
            var hash = Convert.FromHexString(parts[0]);
            var salt = Convert.FromHexString(parts[1]);
            var inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }

    private static class Errors
    {
        public static readonly Error InvalidLength = Error.Failure(
            "Identity.Password.InvalidLength",
            "Password must be between 8 and 128 characters.");

        public static readonly Error Invalid = Error.Failure(
            "Identity.Password.Invalid",
            "Password must contain at least one digit, one uppercase letter, one special character, and no spaces.");
    }
}