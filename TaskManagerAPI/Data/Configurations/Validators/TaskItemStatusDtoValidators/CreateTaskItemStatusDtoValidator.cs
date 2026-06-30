using FluentValidation;
using TaskManagerAPI.Models.Dtos;

namespace TaskManagerAPI.Data.Configurations.Validators.TaskItemStatusDtoValidators
{
    public class CreateTaskItemStatusDtoValidator : AbstractValidator<CreateTaskItemStatusDto>
    {
        public CreateTaskItemStatusDtoValidator()
        {
            RuleFor(tis => tis.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
