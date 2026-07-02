using FluentValidation;
using TaskManagerAPI.Models.Dtos.TaskItemStatusDtos;

namespace TaskManagerAPI.Data.Configurations.Validators.TaskItemStatusDtoValidators
{
    public class TaskItemStatusDtoValidator : AbstractValidator<TaskItemStatusDto>
    {
        public TaskItemStatusDtoValidator()
        {
            RuleFor(tis => tis.Id)
                .GreaterThan(0);

            RuleFor(tis => tis.Name)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}