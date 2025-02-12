using LT = skills_hub.domain.Models.LessonTypes.LessonType;

namespace skills_hub.core.Repository.LessonType.Interfaces;

public interface ILessonTypeService
{
    public Task<LT> GetLastValueAsync(Guid? itemId, bool withParents = false);
    public IQueryable<LT> GetItems();
    public IQueryable<LT> GetCurrentItems();
    public Task<LT> CreateAsync(LT item, Guid[] paymentCategories);
    public Task<LT> UpdateAsync(LT item, Guid[] paymentCategories);
    public Task<LT> RemoveAsync(Guid itemId);
    public Task<LT> RestoreAsync(Guid itemId);

}
