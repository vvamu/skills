namespace skills_hub.Helpers.SearchModels
{
    public class GroupFilterModel
    {
        public string? Name { get; set; } //filter-name
        public string? IsDeleted { get; set; } //filter-isDeleted
        public string? IsLateDateStart { get; set; } //filter-time-start

        public string? IsPermanentStaffGroup { get; set; } //filter-permanent-staff

        public Guid? LessonTypeId { get; set; } //filter-lesson-type-id

        public Guid? CourseNameId { get; set; } //filter-course-name-id
    }
}