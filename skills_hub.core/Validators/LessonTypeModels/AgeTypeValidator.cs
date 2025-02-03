namespace skills_hub.core.Validators;

public class AgeTypeValidator : AbstractValidator<AgeType>
{
    public AgeTypeValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.MaximumAge).GreaterThan(0).WithMessage("Maximum Age must be at least is 1");
        RuleFor(x => x.MinimumAge).GreaterThan(0).WithMessage("Minimum Students can`t be less than 1");
        RuleFor(x => x.MaximumAge).LessThan(101).WithMessage("Maximum Students can`t be more than 100");
        RuleFor(x => x).Must(x => x.MinimumAge <= x.MaximumAge).WithMessage("Maximum Students can`t be  less than minimum");
    }
}

