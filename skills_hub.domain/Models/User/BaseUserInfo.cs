using skills_hub.domain.Models.ManyToMany;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.User;

//1.FullName 2.Sex 3.BirthDate - Age 4.PhonesArray 5.EmailsArray 6.Refs - Roles 7.AdditionalInfo 8.RegistrationDate 9.DateLastUpdate 10.DateCreate, 
public class BaseUserInfo : LogModel<BaseUserInfo>
{

    public new bool IsDeleted { get; set; }
    [NotMapped]
    public string? FullName { get => $"{this?.LastName} {this?.FirstName} {this?.Surname}"; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Surname { get; set; }
    public string Sex { get; set; } = "Male";
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime BirthDate { get; set; } = DateTime.Now.AddDays(-1 * 365 * 20);
    public string? Phones { get; set; }
    public string? Emails { get; set; }
    public string? AdditionalInfo { get; set; }
    public List<ApplicationUserBaseUserInfo> ApplicationUsers { get; set; }
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        BaseUserInfo other = (BaseUserInfo)obj;
        return FirstName == other.FirstName &&
               LastName == other.LastName &&
               Surname == other.Surname;
        //Sex == other.Sex &&
        //BirthDate == other.BirthDate &&
        //Phones == other.Phones &&
        //Emails == other.Emails &&
        //AdditionalInfo == other.AdditionalInfo;
    }

    #region NotMapped

    [NotMapped]
    public string? Role { get; set; }
    [NotMapped]
    public Guid ApplicationUserId { get; set; }

    [NotMapped]
    public int Age { get => DateTime.Now.Year - BirthDate.Year; }

    [NotMapped]
    public string[] PhonesArray
    {
        get
        {
            if (string.IsNullOrEmpty(Phones)) return new string[0];
            else return Phones.Split(", ");
        }
        set
        {
            value = value.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToArray();
            Phones = string.Join(", ", value);
        }
    }

    [NotMapped]
    public string[] EmailsArray
    {
        get
        {
            if (string.IsNullOrEmpty(Emails)) return new string[0];
            else return Emails.Split(", ");
        }
        set
        {
            value = value.Distinct().Where(x => !string.IsNullOrEmpty(x)).ToArray();
            Emails = string.Join(", ", value);
        }
    }
    [NotMapped]
    public string? Email
    {
        get
        {
            if (EmailsArray != null && EmailsArray.Length > 0)
                return EmailsArray[0];
            else
                return null;
        }
        set
        {
            if (EmailsArray == null || EmailsArray.Length == 0)
                EmailsArray = new string[1];

            EmailsArray[0] = value;
        }
    }
    [NotMapped]
    public string? Phone
    {
        get
        {
            if (PhonesArray != null && PhonesArray.Length > 0)
                return PhonesArray[0];
            else
                return null;
        }
        set
        {
            if (PhonesArray == null || PhonesArray.Length == 0)
                PhonesArray = new string[1];

            PhonesArray[0] = value;
        }
    }
    /*
    public string? Emails { get; set; }
    { 
            get 
            {
                if (string.IsNullOrEmpty(this.Phones)) return string.Join(", ", Phones); 
                else return this.Phones;
            }
            set { this.Phones = value; this.PhonesArray = value.Split(", "); }
        } */
    #endregion





}