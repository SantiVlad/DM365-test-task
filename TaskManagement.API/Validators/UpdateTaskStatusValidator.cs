using FluentValidation;
using TaskManagement.API.DTOs;
using System.Linq;

namespace TaskManagement.API.Validators
{
    public class UpdateTaskStatusValidator : AbstractValidator<UpdateTaskStatusDto>
    {
        private readonly string[] ValidStatuses = { "To Do", "In Progress", "Done" };

        public UpdateTaskStatusValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(x => ValidStatuses.Contains(x)).WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");
        }
    }
}


