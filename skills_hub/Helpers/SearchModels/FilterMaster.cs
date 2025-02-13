
using Microsoft.EntityFrameworkCore.Query;
using skills_hub.domain.Models.Groups;
using skills_hub.domain.Models.User;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace skills_hub.Helpers.SearchModels;
public static class FilterMaster
{
    public static async Task<IQueryable<Student>> GetAllStudents(IQueryable<Student> items, StudentFilterModel filters, StudentOrderModel orders)
    {

        if (filters != null)
        {
            if (filters.ApplicationUserId != Guid.Empty)
                items = items.Where(x => x.Id == filters.ApplicationUserId);
            /*
            if (!string.IsNullOrEmpty(filters.ParentName))
                items = items.Where(x => x.ParentName.Contains(filters.ParentName));
            if (!string.IsNullOrEmpty(filters.ParentPhone))
                items = items.Where(x => x.ParentPhone.Contains(filters.ParentPhone));
            */
            if (filters.MinCountPayedLessons != 0)
                items = items.Where(x => x.CountPayedLessons != null).Where(x => x.CountPayedLessons >= filters.MinCountPayedLessons);
            if (filters.MinCountGroups != 0)
                items = items.Where(x => x.Groups != null).Where(x => x.Groups.Count >= filters.MinCountGroups);
            if (filters.GroupId != Guid.Empty)
                items = items.Where(x => x.Groups != null).Where(x => x.Groups.Select(x => x.Id).Contains(filters.GroupId));
            if (!string.IsNullOrEmpty(filters.WorkingDay))
                items = items.Where(x => x.WorkingDays != null)
                    .Where(x => x.WorkingDays.Contains(filters.WorkingDay));
            /*
            if (!string.IsNullOrEmpty(filters.PossibleCourse))
                items = items.Where(x => x.PossibleCources != null)
                    .Where(x => x.PossibleCources.Select(x => x.LessonType.Name).Contains(filters.PossibleCourse));
            */
            if (filters.IsDeleted != -100)
                items = items.Where(x => x.IsDeleted == Convert.ToBoolean(filters.IsDeleted));
            if (filters.MinDateCreated != null)
                items = items.Where(x => x.DateCreated >= filters.MinDateCreated);

        }
        if (orders != null)
        {
            if (orders.CountPayedLessons != -100)
            {
                if (orders.CountPayedLessons >= 0)
                    items.OrderBy(x => x.CountPayedLessons);
                else
                    items.OrderByDescending(x => x.CountPayedLessons);
            }
            /*
            if(orders.Groups != null)
            {
                if (orders.Groups.Count >= 0)
                    items.Where(x=>x.Groups!=null).OrderBy(x => x.Groups.Count);
                else
                    items.Where(x => x.Groups != null).OrderByDescending(x => x.Groups.Count);
            }
            

            if (orders.CountCources != -100)
            {
                if (orders.CountCources >= 0)
                    items.Where(x => x.PossibleCources != null).OrderBy(x => x.PossibleCources.Count);
                else
                    items.Where(x => x.PossibleCources != null).OrderByDescending(x => x.PossibleCources.Count);
            }
            */
            /*
            if (orders.DateCreated != null)
            {
                if (orders.DateCreated >= DateTime.Now)
                    items.OrderBy(x => x.DateCreated);
                else
                    items.OrderByDescending(x => x.DateCreated);
            }
            */

        }

        return items;
    }

    public static async Task<IQueryable<Teacher>> GetAllTeachers(IQueryable<Teacher> items, TeacherFilterModel filters, TeacherOrderModel orders)
    {
        if (filters != null)
        {
            if (filters.ApplicationUserId != Guid.Empty)
                items = items.Where(x => x.Id == filters.ApplicationUserId);
            /*
            if (filters.MinSalary != 0)
                items = items.Where(x => x.Salary >= filters.MinSalary);
            if (filters.MaxSalary != 0)
                items = items.Where(x => x.Salary <= filters.MaxSalary);
            */
            /*
            if (!string.IsNullOrEmpty(filters.PossibleCourse))
                items = items.Where(x => x.PossibleCources != null)
                             .Where(x => x.PossibleCources.Select(c => c.LessonType.Name).Contains(filters.PossibleCourse));
            */
            /*
            if (filters.GroupId != Guid.Empty)
                items = items.Where(x => x.Groups != null)
                             .Where(x => x.Groups.Select(g => g.Id).Contains(filters.GroupId));*/
            if (!string.IsNullOrEmpty(filters.WorkingDay))
                items = items.Where(x => x.WorkingDays != null)
                             .Where(x => x.WorkingDays.Contains(filters.WorkingDay));
            if (filters.IsDeleted != -100)
                items = items.Where(x => x.IsDeleted == Convert.ToBoolean(filters.IsDeleted));
        }

        return items;
    }

    public static async Task<IQueryable<Lesson>> GetAllLessons(IQueryable<Lesson> items, LessonFilterModel filters, OrderModel orders)
    {
        if (items == null) return null;

        if (filters != null)
        {
            if (filters.GroupId != Guid.Empty)
                items = items.Where(x => x.GroupId == filters.GroupId);

            if (filters.TeacherId != Guid.Empty)
                items = items.Where(x => x.TeacherId != null && x.TeacherId == filters.TeacherId);//(x => x.Group!= null && x.Group.GroupTeachers != null).Where(x => x.Group.GroupTeachers.Select(x=>x.Id).Contains(filters.TeacherId));
            if (filters.StudentId != Guid.Empty)
                items = items.Where(x => x.ArrivedStudents != null && x.ArrivedStudents.Select(x => x.StudentId).Contains(filters.StudentId));//(x => x.Group.GroupStudents != null).Where(x => x.Group.GroupStudents.Select(x => x.Id).Contains(filters.StudentId));
            if (!string.IsNullOrEmpty(filters.Topic))
                items = items.Where(x => x.Topic.Contains(filters.Topic));
            if (!string.IsNullOrEmpty(filters.Category))
            {
                if (filters.Category == "Passed")
                    items = items.Where(x => x.EndTime < DateTime.Now);
                if (filters.Category == "Current")
                    items = items.Where(x => x.EndTime > DateTime.Now);
                if (filters.Category == "Deleted")
                    items = items.Where(x => x.IsDeleted == true);
                if (filters.Category == "Active")
                    items = items.Where(x => x.IsDeleted == false);
            }

            if (filters.Category != "Deleted")
                items = items.Where(x => x.IsDeleted == false);

            if (orders != null)
            {
                if (!string.IsNullOrEmpty(orders.OrderType))
                {
                    items = items.OrderByDynamic(orders.OrderColumn, orders.OrderType);
                }
            }
        }


        return items;
    }
    public static async Task<IQueryable<ApplicationUser>> FilterUsers(IQueryable<ApplicationUser> items, UserFilterModel filters, OrderModel orders)
    {
        if (filters != null)
        {
            if (!string.IsNullOrEmpty(filters.StudentWorkingDay))
            {
                items = items.Where(x => x.UserStudent != null).Where(x => x.UserStudent.WorkingDays.Contains(filters.StudentWorkingDay));
            }
            if (!string.IsNullOrEmpty(filters.TeacherWorkingDay))
            {
                items = items.Where(x => x.UserTeacher != null).Where(x => x.UserTeacher.WorkingDays.Contains(filters.TeacherWorkingDay));
            }

            //if (!string.IsNullOrEmpty(filters.StudentPossibleCource))
            //{
            //    var i = items.Where(x => x.UserStudent != null).Where(x => x.UserStudent.PossibleCources != null);
            //    var ki = i.ToList();
            //    items = i.Where(x => x.UserStudent.PossibleCources.Select(x => x.LessonType).Select(x => x.Id.ToString()).Contains(filters.StudentPossibleCource));
            //}


            //if (!string.IsNullOrEmpty(filters.TeacherPossibleCource))
            //{
            //    items = items.Where(x => x.UserTeacher != null).Where(x => x.UserTeacher.PossibleCources != null).Where(x => x.UserTeacher.PossibleCources.Select(x => x.LessonType).Select(x => x.Id.ToString()).Contains(filters.TeacherPossibleCource));
            //}
            if (!string.IsNullOrEmpty(filters.IsDeleted))
            {
                if (filters.IsDeleted == "Yes")
                {
                    items = items.Where(x => x.IsDeleted == true);
                }
                else items = items.Where(x => x.IsDeleted == false);
            }
            else
            {
                items = items.Where(x => x.IsDeleted == false);
            }

            if (orders != null)
            {
                if (!string.IsNullOrEmpty(orders.OrderType))
                {
                    items = items.OrderByDynamic(orders.OrderColumn, orders.OrderType);
                }
            }

            //BY FIO 
            if (!string.IsNullOrEmpty(filters.FIO))
            {
                items = items.Where(x =>
                x.UserName != null && x.UserName.ToLower().Contains(filters.FIO.ToLower()) ||
                x.Login != null && x.Login.ToLower().Contains(filters.FIO.ToLower()) ||
                x.FirstName != null && x.FirstName.ToLower().Contains(filters.FIO.ToLower()) ||
                x.MiddleName != null && x.MiddleName.ToLower().Contains(filters.FIO.ToLower()) ||
                x.Surname != null && x.Surname.ToLower().Contains(filters.FIO.ToLower())
                );

                var uuu = items.ToList();
            }
        }
        return items;
    }

    public static async Task<IQueryable<Group>> FilterGroups(IQueryable<Group> items, GroupFilterModel filters, OrderModel orders)
    {
        if (filters != null)
        {
            if (!string.IsNullOrEmpty(filters.IsDeleted))
            {
                if (filters.IsDeleted == "Yes")
                {
                    items = items.Where(x => x.IsDeleted == true);
                }
                else if (filters.IsDeleted == "No") items = items.Where(x => x.IsDeleted == false);
            }
            else
            {
                items = items.Where(x => x.IsDeleted == false);
            }
            if (!string.IsNullOrEmpty(filters.IsLateDateStart))
            {
                if (filters.IsLateDateStart == "Yes")
                {
                    items = items.Where(x => x.DateStart == DateTime.MinValue);
                }
                else if (filters.IsLateDateStart == "No") items = items.Where(x => x.DateStart != DateTime.MinValue);
            }
            if (!string.IsNullOrEmpty(filters.IsPermanentStaffGroup))
            {
                if (filters.IsPermanentStaffGroup == "Yes")
                {
                    items = items.Where(x => x.IsPermanentStaffGroup == true);
                }
                else if (filters.IsPermanentStaffGroup == "No") items = items.Where(x => x.IsPermanentStaffGroup == false);
            }




            if (!string.IsNullOrEmpty(filters.Name))
            {
                items = items.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));

            }


        }
        if (orders != null)
        {
            if (!string.IsNullOrEmpty(orders.OrderType))
            {
                items = items.OrderByDynamic(orders.OrderColumn, orders.OrderType);
            }
            /*
            foreach (var item in items)
            {
                item.Lessons = item.Lessons.OrderBy(x => x.StartTime).ToList();
            }
            */
        }


        return items;

    }

}

public static class IQueryableExtensions
{
    public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string property, string orderType)
    {
        if (string.IsNullOrEmpty(property))
            return query;

        var type = typeof(T);
        var parameter = Expression.Parameter(type, "x");

        var propertyInfo = type.GetProperty(property);
        if (propertyInfo == null)
        {
            // Check if property is inherited
            propertyInfo = type.GetProperty(property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            if (propertyInfo == null)
            {
                // Handle case for inherited properties which may require more complex navigation
                throw new ArgumentException($"Property '{property}' not found on type '{type.Name}' or its base types.");
            }
        }

        var propertyAccess = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);

        if (orderType.ToLower() == "asc")
        {
            return Queryable.OrderBy(query, lambda);
        }
        else if (orderType.ToLower() == "desc")
        {
            return Queryable.OrderByDescending(query, lambda);
        }

        return query;
    }
}

public static class AsyncQueryable
{
    public static IQueryable<TEntity> AsAsyncQueryable<TEntity>(this IEnumerable<TEntity> source)
        => new AsyncQueryable<TEntity>(source ?? throw new ArgumentNullException(nameof(source)));
}
public class AsyncQueryable<TEntity> : EnumerableQuery<TEntity>, IAsyncEnumerable<TEntity>, IQueryable<TEntity>
{
    public AsyncQueryable(IEnumerable<TEntity> enumerable) : base(enumerable) { }
    public AsyncQueryable(Expression expression) : base(expression) { }
    public IAsyncEnumerator<TEntity> GetEnumerator() => new AsyncEnumerator(this.AsEnumerable().GetEnumerator());
    public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator(this.AsEnumerable().GetEnumerator());
    IQueryProvider IQueryable.Provider => new AsyncQueryProvider(this);

    class AsyncEnumerator : IAsyncEnumerator<TEntity>
    {
        private readonly IEnumerator<TEntity> inner;
        public AsyncEnumerator(IEnumerator<TEntity> inner) => this.inner = inner;
        public void Dispose() => inner.Dispose();
        public TEntity Current => inner.Current;
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(inner.MoveNext());
#pragma warning disable CS1998 // Nothing to await
        public async ValueTask DisposeAsync() => inner.Dispose();
#pragma warning restore CS1998
    }

    class AsyncQueryProvider : IAsyncQueryProvider
    {
        private readonly IQueryProvider inner;
        internal AsyncQueryProvider(IQueryProvider inner) => this.inner = inner;
        public IQueryable CreateQuery(Expression expression) => new AsyncQueryable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new AsyncQueryable<TElement>(expression);
        public object Execute(Expression expression) => inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new AsyncQueryable<TResult>(expression);
        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Execute<TResult>(expression);
    }
}

/*
 *         
 *         var p = HttpContext.Request.QueryString.Value ?? "";
        var result = ParseQueryString(p);
     static Dictionary<string, string> ParseQueryString(string url)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();

        int questionMarkIndex = url.IndexOf('?');
        if (questionMarkIndex != -1)
        {
            string queryString = url.Substring(questionMarkIndex + 1);
            string[] pairs = queryString.Split('&');

            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0];
                    string value = keyValue[1];
                    result[key] = value;
                }
            }
        }

        return result;
    }
*/