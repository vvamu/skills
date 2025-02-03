using Microsoft.EntityFrameworkCore;
using skills_hub.domain.Models.ManyToMany;
using System.ComponentModel.DataAnnotations.Schema;

namespace skills_hub.domain.Models.LessonTypes;

public class PaymentCategory : LogModel<PaymentCategory>
{
    [Precision(15, 4)]
    public decimal StudentPrice { get; set; }
    [Precision(15, 4)]
    public decimal TeacherPrice { get; set; }
    public int MinCountLessonsToPay { get; set; }

    public string? DurationTypeTeacherName { get; set; }
    public int DurationTypeTeacherValue { get; set; }
    public string? DurationTypeStudentName { get; set; }
    public int DurationTypeStudentValue { get; set; }

    public string? CurrencyTeacher { get; set; }
    public string? CurrencyStudent { get; set; }

    public List<LessonTypePaymentCategory>? LessonTypePaymentCategory { get; set; }


    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        PaymentCategory other = (PaymentCategory)obj;
        return Name == other.Name ||
               StudentPrice == other.StudentPrice &&
               TeacherPrice == other.TeacherPrice &&
               DurationTypeTeacherName == other.DurationTypeTeacherName
               && DurationTypeTeacherValue == other.DurationTypeTeacherValue &&
               DurationTypeStudentName == other.DurationTypeStudentName &&
               DurationTypeStudentValue == other.DurationTypeStudentValue
               ;
    }

    #region NotMapped

    [NotMapped]
    public override string DisplayName => $" {Name};  Цена ученика {StudentDisplayName} ; Цена учителя {TeacherDisplayName} ";


    [NotMapped]
    public decimal StudentPricePerLesson
    {
        get
        {
            if (DurationTypeStudentName == "lesson" && DurationTypeStudentValue > 1)
                return StudentPrice / DurationTypeStudentValue;
            else return StudentPrice;
        }
    }

    [NotMapped]
    public decimal TeacherPricePerLesson
    {
        get
        {
            if (DurationTypeTeacherName == "lesson" && DurationTypeTeacherValue > 1)
                return TeacherPrice / DurationTypeTeacherValue;
            else return TeacherPrice;
        }
    }



    [NotMapped]
    public string? FullDisplayName
    {
        get
        {
            if (DurationTypeStudentName == DurationTypeTeacherName && CurrencyStudent == CurrencyTeacher)
                return Name + " - Цена студента/учителя " + StudentPrice + "/" + TeacherPrice + $" {CurrencyStudent} за {RuDurationTypeStudentName}";
            else return $"Цена студента: {StudentDisplayName} ; Цена учителя: {TeacherDisplayName}";
        }
    }
    [NotMapped]
    public string? StudentDisplayName { get => $"{StudentPrice} {CurrencyStudent} {DurationTypeStudentValue} за {RuDurationTypeStudentName}"; }

    [NotMapped]
    public string? TeacherDisplayName { get => $"{TeacherPrice} {CurrencyTeacher} {DurationTypeTeacherValue} за {RuDurationTypeTeacherName}"; }


    [NotMapped]
    public string? RuDurationTypeStudentName
    {
        get
        {
            if (DurationTypeStudentName == "lesson") return "занятие";
            else return DurationTypeStudentName;
        }
    }

    [NotMapped]
    public string? RuDurationTypeTeacherName
    {
        get
        {
            if (DurationTypeTeacherName == "lesson") return "занятие";
            else return DurationTypeTeacherName;
        }
    }
    /*
    [NotMapped]
    public int DurationValue
    {
        get
        {
            if (DurationType != null)
            {
                return Convert.ToInt16(DurationType.Value);
            }
            return 0;
        }
        set { this.DurationValue = value; }
    }
    [NotMapped]
    public string? DurationName
    {
        get
        {
            if (DurationType != null)
            {
                return DurationType.Name;
            }
            return "Минуты";
        }
        set { this.DurationName = value; }
    }
    */
    #endregion





}