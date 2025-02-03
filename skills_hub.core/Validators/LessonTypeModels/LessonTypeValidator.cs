namespace SkillsHub.Application.Validators.LessonTypeModels;


public class LessonTypeValidator : AbstractValidator<LessonType>
{
    public LessonTypeValidator()
    {
        //RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");

        /*RuleFor(x => x.CourseId).NotEmpty().WithMessage("Course is required");
        RuleFor(x => x.AgeTypeId).NotEmpty().WithMessage("Age type is required");
        RuleFor(x => x.GroupTypeId).NotEmpty().WithMessage("Group type is required");
        RuleFor(x => x.LocationId).NotEmpty().WithMessage("Location is required");
        RuleFor(x => x.LessonTimeInMinutes).NotEmpty().WithMessage("Lesson time is required");
        RuleFor(x => x.LessonTimeInMinutes).GreaterThan(0).WithMessage("Minimum LessonTimeInMinutes is 1");
        RuleFor(x => x.LessonTimeInMinutes).LessThan(100).WithMessage("Maximum LessonTimeInMinutes is 100");*/
        //RuleFor(x => x.PaymentCategories).NotEmpty().WithMessage("At least one paymentCategory must be chosen");

        RuleFor(x => x.CourseId).NotEmpty().WithMessage("Необходимо указать курс");
        RuleFor(x => x.AgeTypeId).NotEmpty().WithMessage("Необходимо указать тип возраста");
        RuleFor(x => x.GroupTypeId).NotEmpty().WithMessage("Необходимо указать тип группы");
        RuleFor(x => x.LocationId).NotEmpty().WithMessage("Необходимо указать местоположение");
        RuleFor(x => x.LessonTimeInMinutes).NotEmpty().WithMessage("Необходимо указать время урока");
        RuleFor(x => x.LessonTimeInMinutes).GreaterThan(0).WithMessage("Минимальное время урока 1 минута");
        RuleFor(x => x.LessonTimeInMinutes).LessThan(100).WithMessage("Максимальное время урока 100 минут");
        RuleFor(x => x.DurationTypeValue).GreaterThan(0).WithMessage("Минимальное количество единиц для длительности курса - 1.");
        RuleFor(x => x.DurationTypeName).NotEmpty().WithMessage("Тип длительности курса не выбран");
        RuleFor(x => x).Must(x => x.LessonTimeInMinutes < 150 || x.DurationTypeName == "hours").WithMessage("Выбрано слишком большое количество единиц для длительности курса.");


    }
}

