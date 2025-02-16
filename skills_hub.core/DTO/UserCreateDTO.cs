using System.ComponentModel.DataAnnotations;

namespace skills_hub.core.DTO;


public class UserCreateDTO
{
    public Guid Id { get; set; }
    public Guid BaseUserInfoId { get; set; }
    public string? PasswordChanged { get; set; }
    public string? PasswordChangedConfirm { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Surname { get; set; } = "Default Surname";
    [Required]
    public string UserName { get; set; }
    public string Sex { get; set; } = "Male";

    //[DataType(DataType.Date)]
    //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime BirthDate { get; set; }
    public string[] PhonesArray { get; set; } = new string[5];
    public string[] EmailsArray { get; set; } = new string[5];
    public string? SourceFindCompany { get; set; }
    public string Password { get; set; } //?

    public bool IsTeacher { get; set; }
    public bool IsStudent { get; set; }

    public List<Group> Groups { get; set; }

    public string? EnglishLevel { get; set; }

    public Guid EnglishLevelId { get; set; }
    public bool EnglishLevelConfirmed { get; set; } = false;

    public bool IsVerified { get; set; } = false;
    public bool IsBase { get; set; }
    public bool IsDeleted { get; set; }

}


