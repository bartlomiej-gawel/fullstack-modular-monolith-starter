using Common.Abstractions.Domain.ValueObjects;

namespace Common.Abstractions.UnitTests.Domain.ValueObjects;

public sealed class EmailTests
{
    [Test]
    [Arguments("test@example.com")]
    [Arguments("user.name@domain.org")]
    [Arguments("user+tag@example.co.uk")]
    [Arguments("a@b.c")]
    [Arguments("test123@test-domain.com")]
    public async Task Create_WithValidEmail_ShouldSucceed(string value)
    {
        var result = Email.Create(value);

        await Assert.That(result.IsError).IsFalse();
        await Assert.That(result.Value.Value).IsEqualTo(value);
    }

    [Test]
    [Arguments("ab")]
    [Arguments("a")]
    [Arguments("")]
    public async Task Create_WithTooShortEmail_ShouldReturnInvalidLengthError(string value)
    {
        var result = Email.Create(value);

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Email.InvalidLength");
    }

    [Test]
    public async Task Create_WithTooLongEmail_ShouldReturnInvalidLengthError()
    {
        var longEmail = new string('a', 251) + "@b.c";
        var result = Email.Create(longEmail);

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Email.InvalidLength");
    }

    [Test]
    [Arguments("notanemail")]
    [Arguments("@nodomain.com")]
    [Arguments("spaces in@email.com")]
    [Arguments("double@@at.com")]
    [Arguments("trailing.dot.@example.com")]
    [Arguments(".leading.dot@example.com")]
    public async Task Create_WithInvalidFormat_ShouldReturnInvalidError(string value)
    {
        var result = Email.Create(value);

        await Assert.That(result.IsError).IsTrue();
        await Assert.That(result.FirstError.Code).IsEqualTo("Common.Email.Invalid");
    }

    [Test]
    public async Task TwoEmails_WithSameValue_ShouldBeEqual()
    {
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        await Assert.That(email1.Value).IsEqualTo(email2.Value);
    }

    [Test]
    public async Task TwoEmails_WithDifferentValues_ShouldNotBeEqual()
    {
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        await Assert.That(email1.Value).IsNotEqualTo(email2.Value);
    }
}
