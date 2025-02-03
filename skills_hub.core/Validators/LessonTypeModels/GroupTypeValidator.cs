namespace skills_hub.core.Validators;

public class GroupTypeValidator : AbstractValidator<GroupType>
{
    public GroupTypeValidator()
    {
        /*RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
		RuleFor(x => x.MinimumStudents).GreaterThan(0).WithMessage("Maximum Age must be at least is 1");
        RuleFor(x => x.MaximumStudents).GreaterThan(0).WithMessage("Minimum Students can`t be less than 1");
        RuleFor(x => x.MaximumStudents).LessThan(30).WithMessage("Maximum Students can`t be more than 30");
        RuleFor(x => x).Must(x => x.MinimumStudents <= x.MaximumStudents).WithMessage("Maximum Students can`t be  less than minimum");*/

        RuleFor(x => x.Name).NotEmpty().WithMessage("Имя обязательно");
        RuleFor(x => x.MinimumStudents).GreaterThan(0).WithMessage("Минимальное количество студентов должно быть как минимум 1");
        RuleFor(x => x.MaximumStudents).GreaterThan(0).WithMessage("Минимальное количество студентов не может быть меньше 1");
        RuleFor(x => x.MaximumStudents).LessThan(30).WithMessage("Максимальное количество студентов не может превышать 30");
        RuleFor(x => x).Must(x => x.MinimumStudents <= x.MaximumStudents).WithMessage("Максимальное количество студентов не может быть меньше минимального");
    }
}

