using skills_hub.core.DTO;

namespace skills_hub.core.Validators;

public class UserLoginValidator : AbstractValidator<UserLoginDTO>
{
    public UserLoginValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("Login is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }

}


