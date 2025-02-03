using skills_hub.domain.Base;

namespace skills_hub.domain.Models.Groups;

public class GroupWorkingDay : BaseEntity
{
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }
    public DayOfWeek? DayName { get; set; }
    public string? DayNameString { get => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((DayOfWeek)DayName); }
    public TimeSpan? WorkingStartTime { get; set; }
    public TimeSpan? WorkingEndTime { get; set; }

    public TimeSpan? StartDate { get; set; }
    public TimeSpan? EndDate { get; set; }
    public string? RepeatIntervalName { get; set; } //День, Неделя, Месяц
    public string? RepeatIntervalValue { get; set; }
}
