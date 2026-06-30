using FluentValidation;
using TaskManagerAPI.Models.Identity;

namespace TaskManagerAPI.Data.Configurations.Validators.IdentityValidators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(lr => lr.Email)
                .NotEmpty().
                EmailAddress();

            RuleFor(lr => lr.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}
