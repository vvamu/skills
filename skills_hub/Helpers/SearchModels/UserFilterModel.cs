namespace skills_hub.Helpers.SearchModels;

public class UserFilterModel
{

    public string? FIO { get; set; }
    public string? TeacherWorkingDay { get; set; }
    public string? StudentWorkingDay { get; set; }

    public string? TeacherPossibleCource { get; set; }
    public string? StudentPossibleCource { get; set; }

    public string? UserRole { get; set; }

    public string IsDeleted { get; set; }

}