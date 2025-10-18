using FluentValidation;
using TaskManagement.API.DTOs;
using System.Linq;

namespace TaskManagement.API.Validators
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskDto>
    {
        private readonly string[] ValidStatuses = { "To Do", "In Progress", "Done" };

        public UpdateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(x => ValidStatuses.Contains(x)).WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

            RuleFor(x => x.AssignedTo)
                .GreaterThan(0).When(x => x.AssignedTo.HasValue)
                .WithMessage("AssignedTo must be a valid user ID when specified");
        }
    }
}


