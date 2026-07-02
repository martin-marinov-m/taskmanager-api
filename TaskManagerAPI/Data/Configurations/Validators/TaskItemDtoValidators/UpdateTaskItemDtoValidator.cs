using FluentValidation;
using TaskManagerAPI.Models.Dtos.TaskItemDtos;

namespace TaskManagerAPI.Data.Configurations.Validators.TaskItemDtoValidators
{
    public class UpdateTaskItemDtoValidator : AbstractValidator<UpdateTaskItemDto>
    {
        public UpdateTaskItemDtoValidator()
        {
            RuleFor(ti => ti.Id)
                .GreaterThan(0);

            RuleFor(ti => ti.Title)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(ti => ti.Description)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(ti => ti.DueDate)
                .Must(ti => !ti.HasValue || ti.Value >= DateTime.UtcNow)
                .WithMessage("DueDate cannot be in past");

            RuleFor(ti => ti.StatusId)
                .GreaterThan(0);
        }
    }
}