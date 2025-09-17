using FluentValidation;
using TaskServiceApi.DTOs;

namespace TaskServiceApi.Validation
{
    public sealed class TaskUpdateDtoValidator : AbstractValidator<TaskUpdateDto>
    {
        public TaskUpdateDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).MaximumLength(100);
            RuleFor(x => x.DueDate)
                .Must(d => d >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("DueDate не может быть в прошлом.");
        }
    }
}
