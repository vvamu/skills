namespace SkillsHub.Application.Validators.LessonTypeModels;

public class PaymentCategoryValidator : AbstractValidator<PaymentCategory>
{
    public PaymentCategoryValidator()
    {
        /*RuleFor(x => x.TeacherPrice).GreaterThan(0).WithMessage("Minimum TeacherPrice is 1");
        RuleFor(x => x.StudentPrice).GreaterThan(0).WithMessage("Minimum StudentPrice is 1");
        RuleFor(x => x.MinCountLessonsToPay).GreaterThan(0).WithMessage("Minumum Lessons To Pay is 1");
        RuleFor(x => x.DurationTypeTeacherName).NotEmpty().WithMessage("Duration value type for teacher is required");
        RuleFor(x => x.DurationTypeTeacherValue).NotEmpty().WithMessage("Duration type for teacher is required");
        RuleFor(x => x.DurationTypeStudentName).NotEmpty().WithMessage("Duration value type for student is required");
        RuleFor(x => x.DurationTypeStudentValue).NotEmpty().WithMessage("Duration type for student is required");*/

        RuleFor(x => x.TeacherPrice).GreaterThan(0).WithMessage("Минимальная цена для учителя 1");
        RuleFor(x => x.StudentPrice).GreaterThan(0).WithMessage("Минимальная цена для студента 1");
        RuleFor(x => x.MinCountLessonsToPay).GreaterThan(0).WithMessage("Минимальное количество уроков для оплаты 1");
        RuleFor(x => x.DurationTypeTeacherName).NotEmpty().WithMessage("Необходимо указать тип значения продолжительности для учителя");
        RuleFor(x => x.DurationTypeTeacherValue).NotEmpty().WithMessage("Необходимо указать тип продолжительности для учителя");
        RuleFor(x => x.DurationTypeStudentName).NotEmpty().WithMessage("Необходимо указать тип значения продолжительности для студента");
        RuleFor(x => x.DurationTypeStudentValue).NotEmpty().WithMessage("Необходимо указать тип продолжительности для студента");
    }
}

