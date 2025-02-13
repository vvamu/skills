namespace skills_hub.Helpers.SearchModels;

public class StudentFilterModel
{
    public Guid GroupId { get; set; }
    public string? WorkingDay { get; set; }

    //----------------------------------
    public Guid ApplicationUserId { get; set; }
    public string? ParentName { get; set; }
    public string? ParentPhone { get; set; }
    public int MinCountPayedLessons { get; set; }
    public int MinCountGroups { get; set; }

    public DateTime? MinDateCreated { get; set; }


    public string? PossibleCourse { get; set; }
    public int IsDeleted { get; set; } = -100;
}

public class StudentOrderModel
{
    public int DateCreated { get; set; } = -100;
    public int CountPayedLessons { get; set; } = -100;
    public int CountGroups { get; set; } = -100;
    public int CountCources { get; set; } = -100;

    // public int CountLessons { get; set; } = -100;

}