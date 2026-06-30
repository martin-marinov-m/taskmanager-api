using FluentValidation;
using TaskManagerAPI.Models.Filters;

namespace TaskManagerAPI.Data.Configurations.Validators.FilterValidators
{
    public class TaskItemFiltersValidator : AbstractValidator<TaskItemFilters>
    {
        public TaskItemFiltersValidator()
        {
            RuleFor(tif => tif.Page)
                .GreaterThan(0);

            RuleFor(tif => tif.Take)
                .InclusiveBetween(0, 100);
        }
    }
}
