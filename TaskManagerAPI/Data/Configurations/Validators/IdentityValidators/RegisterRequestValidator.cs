using FluentValidation;
using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Data.Configurations.Validators.IdentityValidators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(rr => rr.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(rr => rr.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(rr => rr.Role)
                .NotEmpty();
        }
    }
}