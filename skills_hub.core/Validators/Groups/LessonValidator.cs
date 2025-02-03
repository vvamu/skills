namespace skills_hub.core.Validators;

public class LessonValidator : AbstractValidator<Lesson>
{
    public LessonValidator()
    {
        RuleFor(x => x.StartTime).NotEmpty().WithMessage("StartTime is required.");
        RuleFor(x => x.EndTime).NotEmpty().WithMessage("EndTime is required.");
        //RuleFor(x => x).Must(x => x.Group != null || x.GroupId != Guid.Empty).WithMessage("Group is required.");

        RuleFor(x => x).Must(x => x.StartTime < x.EndTime).WithMessage("End time less than start time.");
        RuleFor(x => x).Must(x => (x.EndTime - x.StartTime).TotalMinutes > 5 && (x.EndTime - x.StartTime).TotalMinutes < 60 * 3).WithMessage("Not correct date. Min duration is 5 minutes and max duration is 180 min.");
        RuleFor(x => x).Must(x => x.StartTime.Year > (DateTime.Now.Year - 10) && x.StartTime.Year < (DateTime.Now.Year + 10)).WithMessage("Start time not valid");
        RuleFor(x => x).Must(x => x.EndTime.Year > (DateTime.Now.Year - 10) && x.EndTime.Year < (DateTime.Now.Year + 10)).WithMessage("End time not valid");
        //RuleFor(x => x)
        //    .Must(x => (x.IsСompleted && x.ArrivedStudents.Count() > 0) || !x.IsСompleted)
        //    .WithMessage("Group can't be started if not verified");
    }
}

