namespace skills_hub.Helpers.SearchModels;

public class TeacherFilterModel
{
    public Guid ApplicationUserId { get; set; }

    public decimal MinSalary { get; set; }
    public decimal MaxSalary { get; set; }
    public string PossibleCourse { get; set; }
    public Guid GroupId { get; set; }
    public string WorkingDay { get; set; }
    public int IsDeleted { get; set; } = -100;
}
public class TeacherOrderModel
{
    public int Salary { get; set; } = -100;
    public int CountGroups { get; set; } = -100;

    public int CountCources { get; set; } = -100;
    // public int CountLessons { get; set; } = -100;

}
