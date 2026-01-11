using FluentValidation;

namespace Modules.Identity.Endpoints.BackofficeUsers.Create;

internal sealed class CreateBackofficeUserRequestValidator : AbstractValidator<CreateBackofficeUserRequest>
{
    public CreateBackofficeUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(254)
            .EmailAddress();

        RuleFor(x => x.PhonePrefix)
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(4);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MinimumLength(4)
            .MaximumLength(15);
    }
}
