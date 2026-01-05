using Common.Abstractions.Domain.ValueObjects;

namespace Common.Abstractions.UnitTests.Domain.ValueObjects;

public sealed class PhoneTests
{
    [Test]
    [Arguments("48", "601234567")]
    [Arguments("+48", "601234567")]
    [Arguments("1", "2025551234")]
    [Arguments("44", "7911123456")]
    public async Task Create_WithValidPhone_ShouldSucceed(string prefix, string number)
    {
        var result = Phone.Create(prefix, number);

        await Assert.That(result.IsError).IsFalse();
        await Assert.That(result.Value.Prefix).IsEqualTo(prefix);
        await Assert.That(result.Value.Number).IsEqualTo(number);
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task Create_WithEmptyPrefix_ShouldReturnInvalidPrefixError(string prefix)
    {
        var result = Phone.Create(prefix, "601234567");

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Phone.InvalidPrefix");
    }

    [Test]
    public async Task Create_WithTooLongPrefix_ShouldReturnInvalidPrefixLengthError()
    {
        var result = Phone.Create("12345", "601234567");

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Phone.InvalidPrefixLength");
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    public async Task Create_WithEmptyNumber_ShouldReturnInvalidNumberError(string number)
    {
        var result = Phone.Create("48", number);

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Phone.InvalidNumber");
    }

    [Test]
    [Arguments("123")]
    [Arguments("12")]
    [Arguments("1")]
    public async Task Create_WithTooShortNumber_ShouldReturnInvalidNumberLengthError(string number)
    {
        var result = Phone.Create("48", number);

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Phone.InvalidNumberLength");
    }

    [Test]
    public async Task Create_WithTooLongNumber_ShouldReturnInvalidNumberLengthError()
    {
        var result = Phone.Create("48", "1234567890123456");

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Phone.InvalidNumberLength");
    }

    [Test]
    public async Task Create_WithMismatchedCountryCode_ShouldReturnError()
    {
        var result = Phone.Create("48", "2025551234");

        await Assert.That(result.IsError).IsTrue();
    }

    [Test]
    public async Task Create_WithInvalidPhoneNumber_ShouldReturnError()
    {
        var result = Phone.Create("48", "0000000000");

        await Assert.That(result.IsError).IsTrue();
    }

    [Test]
    public async Task TwoPhones_WithSameValues_ShouldBeEqual()
    {
        var phone1 = Phone.Create("48", "601234567");
        var phone2 = Phone.Create("48", "601234567");

        await Assert.That(phone1.Value).IsEqualTo(phone2.Value);
    }

    [Test]
    public async Task TwoPhones_WithDifferentPrefixes_ShouldNotBeEqual()
    {
        var phone1 = Phone.Create("48", "601234567");
        var phone2 = Phone.Create("+48", "601234567");

        await Assert.That(phone1.Value).IsNotEqualTo(phone2.Value);
    }

    [Test]
    public async Task TwoPhones_WithDifferentNumbers_ShouldNotBeEqual()
    {
        var phone1 = Phone.Create("48", "601234567");
        var phone2 = Phone.Create("48", "601234568");

        await Assert.That(phone1.Value).IsNotEqualTo(phone2.Value);
    }
}
