namespace SkillsHub.Application.Validators.LessonTypeModels;

public class CourseValidator : AbstractValidator<Course>
{
    public CourseValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Имя обязательно");
        RuleFor(x => x.Subject).NotEmpty().WithMessage("Предмет обязателен");

    }
}

