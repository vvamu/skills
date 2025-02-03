using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.ManyToMany;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.LessonTypes;


public class LessonType : LogModel<LessonType>
{
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int LessonTimeInMinutes { get; set; }
    public string? FrequencyName { get; set; }
    public int FrequencyValue { get; set; }
    public string? DurationTypeName { get; set; }
    public int DurationTypeValue { get; set; }
    public Course? Course { get; set; }
    public Guid? CourseId { get; set; }
    public GroupType? GroupType { get; set; }
    public Guid? GroupTypeId { get; set; }
    public Location? Location { get; set; }
    public Guid? LocationId { get; set; }
    public AgeType? AgeType { get; set; }
    public Guid? AgeTypeId { get; set; }

    public List<LessonTypePaymentCategory>? LessonTypePaymentCategory { get; set; }

    public List<Group>? Groups { get; set; }

    //public int CountScheduleDays { get; set; }

    public bool CheckGroupType { get; set; }
    public bool CheckAgeType { get; set; }
    public bool CheckCountScheduleDays { get; set; }
    [NotMapped]
    public bool IsUnlimitedLessonsCount { get; set; }


    #region NotMapped

    [NotMapped]
    public int PreparedLessonsCount
    {
        get
        {
            switch (DurationTypeName)
            {
                case "lesson": return DurationTypeValue;
            }
            return 1;
        }
    }

    [NotMapped]
    public string? PreparedDurationTextRu
    {
        get
        {
            if (IsUnlimitedLessonsCount) return "Пока не уйдет учитель :) ";
            switch (DurationTypeName)
            {
                case "lesson": return $"{DurationTypeValue} занятий ({LessonTimeInMinutes * DurationTypeValue} минут)";

            }
            return "Не определена";
        }
    }

    public string RuFrequencyName
    {
        get
        {
            if (FrequencyName == "week") return "неделя";
            else return "не определено";
        }
    }


    [NotMapped]
    public string? Check
    {
        get
        {
            var result = "";
            if (CheckGroupType || CheckAgeType || CheckCountScheduleDays) result = "Обязательно соблюдать: ";
            if (CheckGroupType) result += GroupType?.DisplayName?.Replace("(", "").Replace(")", "") + " ; ";

            if (CheckAgeType) result += " " + AgeType?.DisplayName?.Replace("(", "").Replace(")", "") + " ; ";
            if (CheckCountScheduleDays) result += " " + RuFrequencyName + $" - {FrequencyValue}  занятие  ";
            return result;
        }
    }
    [NotMapped]
    public override string? DisplayName
    {
        get
        {
            var res = $"{Status} - Название: {Name};  Описание: {Description}; Время занятия : {LessonTimeInMinutes} минут; {DurationText}; {Course?.DisplayName}; Тип группы: {GroupType?.DisplayName?.Replace("(", "").Replace(")", "")}; Местоположение: {Location?.DisplayName}; Возрастная категория:   {AgeType?.DisplayName}; {Check}";
            return res;
        }
    }

    public string? DurationText => $"Длительность в {(RuDurationType == "занятие" ? "занятиях" : RuDurationType)} : {DurationTypeValue}";

    public new string? Status
    {
        get
        {
            if (IsDeleted)
                return "Удален";
            else if (IsActive) return "Активен";
            else return "Не активен";
        }
    }



    [NotMapped]
    public int GroupsCount
    {
        get
        {
            if (Groups == null) return 0;
            else return Groups.Count;
        }
    }



    [NotMapped]
    public string? RuDurationType
    {
        get
        {
            if (DurationTypeName == "lesson") return "занятие";
            else return DurationTypeName;
        }
    }
    [NotMapped]
    public decimal StudentTotalCoursePrice { get => MinCountLessonsToPay * StudentPricePerLesson; }
    [NotMapped]
    public decimal TeacherTotalCoursePrice { get => MinCountLessonsToPay * TeacherPricePerLesson; }

    [NotMapped]
    public int MinCountLessonsToPay { get; set; }

    [NotMapped]
    public decimal StudentPricePerLesson { get; set; }
    [NotMapped]
    public decimal TeacherPricePerLesson { get; set; }

    [NotMapped]
    public Guid[] PaymentCategories { get; set; }

    #endregion

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        LessonType other = (LessonType)obj;
        return Name == other.Name ||
               CourseId == other.CourseId &&
               GroupTypeId == other.GroupTypeId &&
               LocationId == other.LocationId &&
               AgeTypeId == other.AgeTypeId &&
               DurationTypeName == other.DurationTypeName &&
               DurationTypeValue == other.DurationTypeValue;
    }




    /*
    public decimal StudentPricePerCource => StudentPrice * MinumumLessonsToPay;
    public decimal TeacherPricePerCource => TeacherPrice * MinumumLessonsToPay;
    */


}



//Относится к одному занятию
//Администратор создает курс.
//Курс
//Выбирает по чем занятие - английский для взрослых, программирование для детей и тд
//Выбирает онлайн или оффлайн. +++ доделать возможность добавить добавлять кабинеты
//Выбирает количество занятий, которое должно быть
//Выбирает дни недели, в которое проводится занятие
//Появляется Период 01.11, 02.11, 05.11. Можно редактировать
//Выбирает размер оплаты
//Выбирает преподавателя
//Выбирает учеников/группу

//Занятие
//Онлайн или оффлайн
//Тип