namespace skills_hub.core.Validators;

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {

        RuleFor(x => x.Name).NotEmpty().WithMessage("Имя обязательно.");
        RuleFor(x => x).Must(x => (!string.IsNullOrEmpty(x.Description) && x.IsOffline) || !x.IsOffline).NotEmpty().WithMessage("Для очных занятий обязательно указание места проведения.");
    }
}

