using FluentValidation;
using TaskManagerAPI.Models.Dtos.TaskItemDtos;

namespace TaskManagerAPI.Data.Configurations.Validators.TaskItemDtoValidators
{
    public class CreateTaskItemDtoValidator : AbstractValidator<CreateTaskItemDto>
    {
        public CreateTaskItemDtoValidator()
        {
            RuleFor(ti => ti.Title)
                .NotEmpty().
                MaximumLength(50);

            RuleFor(ti => ti.Description)
                .NotEmpty().
                MaximumLength(255);

            RuleFor(ti => ti.DueDate)
                .Must(ti => !ti.HasValue || ti.Value >= DateTime.UtcNow)
                .WithMessage("DueDate cannot be in past");

            RuleFor(ti => ti.StatusId)
                .GreaterThan(0);
        }
    }
}
