using skills_hub.core.DTO;
using System.ComponentModel.DataAnnotations;


namespace skills_hub.core.Validators;

public class UserCreateDTOValidator : AbstractValidator<UserCreateDTO>
{
    public UserCreateDTOValidator()
    {
        //&& email.EmailAddress().WithMessage("Invalid email format.")
        RuleFor(x => x.EmailsArray).NotEmpty().WithMessage("Email is required.").Must(x => x.Any(email => !string.IsNullOrEmpty(email))).WithMessage("Email is required.").Must(x => x.All(email => new EmailAddressAttribute().IsValid(email) )).WithMessage("Invalid email format.");
        RuleFor(x => x.PhonesArray).NotEmpty().WithMessage("Phone is required.").Must(x => x.Any(email => !string.IsNullOrEmpty(email))).WithMessage("Phone is required.").Must(x => x.All(email => new PhoneAttribute().IsValid(email))).WithMessage("Invalid phone format.");

        RuleFor(x => x.Login).NotEmpty().WithMessage("Login is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");


        RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required.");
        RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname is required.");
        RuleFor(x => x.BirthDate.Year).LessThan((DateTime.Now).Year - 2).GreaterThan(1900).NotEmpty().WithMessage("Birthday not correct.");

        //RuleFor(x => x.BirthDate.Year).LessThan((DateTime.Now).Year - 3).GreaterThan(1900).NotEmpty().WithMessage("Birthday not correct.");


        //if (item.EnglishLevelId != Guid.Empty) item.EnglishLevel = await _context.EnglishLevels.FirstOrDefaultAsync(x => x.Id == item.EnglishLevelId);
        //if (_context.ApplicationUsers.FirstOrDefault(x => x.Email == user.Email) != null || user.Email == null) throw new Exception("User with such email alredy exists");
        //if (_context.ApplicationUsers.FirstOrDefault(x => x.Phone == user.Phone) != null) throw new Exception("User with such phone alredy exists");


    }
}

