using FluentValidation;
using TaskManagement.API.DTOs;
using System.Linq;

namespace TaskManagement.API.Validators
{
    public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
    {
        private readonly string[] ValidStatuses = { "To Do", "In Progress", "Done" };

        public CreateTaskValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(x => ValidStatuses.Contains(x)).WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

            RuleFor(x => x.CreatedBy)
                .GreaterThan(0).WithMessage("CreatedBy must be a valid user ID");

            RuleFor(x => x.AssignedTo)
                .GreaterThan(0).When(x => x.AssignedTo.HasValue)
                .WithMessage("AssignedTo must be a valid user ID when specified");
        }
    }
}


