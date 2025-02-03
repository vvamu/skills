namespace skills_hub.core.Validators;

public class WorkingDayValidator : AbstractValidator<GroupWorkingDay>
{
    public WorkingDayValidator()
    {
        RuleFor(x => x).Must(x => x.WorkingEndTime >= x.WorkingStartTime).WithMessage("The end time cannot be greater than the start time.");

        //RuleFor(x => x.CourceId && x.CourseName).NotEmpty().WithMessage("Cource is required");
        //RuleFor(x => x.LessonType).NotEmpty().WithMessage("LessonType is required");
    }
}

