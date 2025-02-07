using System.ComponentModel.DataAnnotations;


namespace skills_hub.core.Validators;

public class BaseUserInfoValidator : AbstractValidator<BaseUserInfo>
{
    public BaseUserInfoValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required.");
        RuleFor(x => x.MiddleName).NotEmpty().WithMessage("MiddleName is required.");
        RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required.");
        RuleFor(x => x.BirthDate.Year).LessThan((DateTime.Now).Year - 2).GreaterThan(1900).NotEmpty().WithMessage("Birthday not correct.");

        //RuleFor(x => x.BirthDate.Year).LessThan((DateTime.Now).Year - 3).GreaterThan(1900).NotEmpty().WithMessage("Birthday not correct.");


        //if (item.EnglishLevelId != Guid.Empty) item.EnglishLevel = await _context.EnglishLevels.FirstOrDefaultAsync(x => x.Id == item.EnglishLevelId);
        //if (_context.ApplicationUsers.FirstOrDefault(x => x.Email == user.Email) != null || user.Email == null) throw new Exception("User with such email alredy exists");
        //if (_context.ApplicationUsers.FirstOrDefault(x => x.Phone == user.Phone) != null) throw new Exception("User with such phone alredy exists");


    }
}

