


namespace skills_hub.core.Validators;

public class GroupValidator : AbstractValidator<Group>
{
    public GroupValidator()
    {

        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.LessonTypeId).NotNull().NotEmpty().WithMessage("Lesson type can`t be empty");
        RuleFor(x => x.PaymentCategoryId).NotNull().NotEmpty().WithMessage("Payment Category start can`t be empty");
        RuleFor(x => x.TeacherId).NotNull().NotEmpty().WithMessage("Teacher can`t be empty");
        RuleFor(x => x.DateStart).NotNull().NotEmpty().WithMessage("Date start can`t be empty");

        /*
        RuleFor(x => x)
            .Must(x => (x.IsUnlimitedLessonsCount && x.LessonsCount <= 0) || (!x.IsUnlimitedLessonsCount && x.LessonsCount > 1))
            .WithMessage("Invalid lessons сount value");
        */
        RuleFor(x => x)
            .Must(x => (!x.IsLateDateStart && (x.DateStart.Year >= DateTime.Now.Year - 1 && x.DateStart.Year <= DateTime.Now.Year + 1))
            || (x.IsLateDateStart))
            .WithMessage("Not valid date start");

        RuleFor(x => x).Must(x => (!x.IsVerified && x.DateStart > DateTime.Now) || (x.IsVerified)).WithMessage("Group can`t be started if not verified. Choose a later start time.");



        //RuleFor(x => x.CourceId && x.CourseName).NotEmpty().WithMessage("Cource is required");
        //RuleFor(x => x.LessonType).NotEmpty().WithMessage("LessonType is required");
    }
}

