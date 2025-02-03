namespace skills_hub.core.Helpers;
public interface IAbstractLogModel<T>
{
    public Task<T> GetAsync(Guid? id, bool withParents = false);
    public Task<T> GetLastValueAsync(Guid? itemId, bool withParents = false);

    public Task<IQueryable<T>> GetAllAsync();
    public IQueryable<T> GetAllItemsToList();

    public IEnumerable<T> GetAllParents(Guid childId);
    public IEnumerable<T> GetAllChildren(Guid parentId);
    public Task<T> UpdateAsync(T item);
    public Task<T> CreateAsync(T item);
    public Task<T> RemoveAsync(Guid itemId);
    public Task<T> RestoreAsync(Guid itemId);

}






