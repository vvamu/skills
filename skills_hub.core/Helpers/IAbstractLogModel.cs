namespace skills_hub.core.Helpers;
public interface IAbstractLogModel<T>
{
    public Task<T> GetLastValueAsync(Guid? itemId, bool withParents = false);
    public IQueryable<T> GetCurrentItems();
    public Task<IQueryable<T>> GetCurrentItemsWithParents();
    public Task<T> UpdateAsync(T item);
    public Task<T> CreateAsync(T item);
    public Task<T> RemoveAsync(Guid itemId);
    public Task<T> RestoreAsync(Guid itemId);
}






