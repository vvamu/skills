global using FluentValidation;
global using skills_hub.core.Helpers;
global using skills_hub.domain.Models.Groups;
global using skills_hub.domain.Models.LessonTypes;
global using skills_hub.domain.Models.User;
global using skills_hub.persistence;
using Microsoft.EntityFrameworkCore;
using skills_hub.domain;
using System.ComponentModel.DataAnnotations;


namespace skills_hub.core.Helpers;

public abstract class AbstractLogModelService<T> : IAbstractLogModel<T> where T : LogModel<T>, new()
{
    protected ApplicationDbContext _context;
    protected DbSet<T> _contextModel;
    protected IValidator<T> _validator { get; set; }
    protected IQueryable<T>? _fullInclude;

    public async Task<T> GetLastValueAsync(Guid? itemId, bool withParents = false)
    {
        if (itemId == Guid.Empty) return new T();
        var res = await GetAsync(itemId, withParents);
        if (res.ParentId != null)
        {
            res = GetAllParents(res.Id).OrderBy(x => x.DateCreated).LastOrDefault();
            res = await GetAsync(res.Id, withParents);
        }
        return res;
    }
    public IQueryable<T> GetCurrentItems()
    {
        var items = _contextModel.Where(x => x.ParentId == null || x.ParentId == Guid.Empty);
        return items;
    }

    public IQueryable<T> GetItems() => _contextModel;

    public async Task<IQueryable<T>> GetCurrentItemsWithParents()
    {
        var items = _contextModel.Where(x => x.ParentId == null || x.ParentId == Guid.Empty);

        foreach (var item in items)
        {
            //item.Children = GetAllParents((Guid)item.Id).Where(x => x.Id != item.Id).ToList();
            item.Parents = GetAllChildren((Guid)item.Id).Where(x => x.Id != item.Id).ToList();
        }
        return items;
    }


    public async Task<T> CreateAsync(T item)
    {
        var itemId = Guid.Empty; if (item.Id != null) itemId = item.Id;

        var oldValue = _contextModel.Find(itemId) ?? new T();
        await Validate(oldValue, item);

        item.Id = Guid.Empty;
        item.DateCreated = DateTime.Now;
        var res = await _contextModel.AddAsync(item);


        var children = GetAllParents(oldValue.Id).OrderByDescending(x => x.DateCreated).ToList();
        var parents = GetAllChildren(oldValue.Id).OrderByDescending(x => x.DateCreated).ToList();

        if (oldValue.Id != Guid.Empty) parents.Insert(0, oldValue);
        if (children.Count() != 0)
        {
            res.Entity.DateRegistration = parents.LastOrDefault()?.DateCreated;
        }
        else
        {
            res.Entity.DateRegistration = res.Entity.DateCreated;
        }
        await _context.SaveChangesAsync();
        return res.Entity;
    }
    public virtual async Task<T> UpdateAsync(T item)
    {
        var itemDb = _contextModel.FirstOrDefault(x => x.Id == item.Id) ?? throw new Exception("No info in database");
        var resCreated = await CreateAsync(item);

        itemDb.ParentId = resCreated.Id;
        _contextModel.Update(itemDb);
        await _context.SaveChangesAsync();
        var items = GetCurrentItems().ToList();
        return await GetAsync(itemDb.Id);
    }

    public async Task<T> RemoveAsync(Guid itemId)
    {
        var itemDb = _contextModel.FirstOrDefault(x => x.Id == itemId) ?? throw new Exception("No info in database");
        var children = GetAllChildren(itemId).ToList();

        //var parents = GetAllChildren(itemId).ToList();
        var isHardDelete = await IsHardDelete(children.AsQueryable());
        if (isHardDelete)
        {
            _contextModel.RemoveRange(children);
            /*
            var payments = _context.LessonTypePaymentCategories.Where(x=>x.LessonTypeId == itemId).ToList();
            _context.LessonTypePaymentCategories.RemoveRange(payments);*/

        }
        else
        {
            itemDb.IsDeleted = true;
            await UpdateAsync(itemDb);
            //_contextModel.Update(itemDb);
        }

        await _context.SaveChangesAsync();
        return await GetAsync(itemDb.Id);
    }

    public async Task<T> RestoreAsync(Guid itemId)
    {
        var itemDb = _contextModel.FirstOrDefault(x => x.Id == itemId) ?? throw new Exception("No info in database");
        itemDb.IsDeleted = false;
        _contextModel.Update(itemDb);
        await _context.SaveChangesAsync();
        return await GetAsync(itemDb.Id);
    }

    public virtual async Task<bool> IsHardDelete(IQueryable<T> items)
    {
        return true;
    }
    public virtual async Task Validate(T oldValue, T newItem)
    {
        if (_validator != null)
        {
            var userValidationResult = await _validator.ValidateAsync(newItem);
            if (!userValidationResult.IsValid)
            {
                var errors = userValidationResult.Errors;
                var errorsString = string.Concat(errors);
                throw new Exception(errorsString);
            }
        }

        //Check unique rows without considering parent rows
        var resultSearching = await _contextModel.Where(x => x.Parent == null).ToListAsync();
        //resultSearching = resultSearching.Where(x => !children.Select(x => x.Id).Contains(x.Id)).ToList();
        resultSearching = resultSearching.Where(x => x.Equals(newItem)).ToList();
        if (resultSearching.Count() > 0  && resultSearching.FirstOrDefault().Id != oldValue.Id) throw new Exception("Entity with those properties already defined");

        if (oldValue == null) return;
        var children = GetAllChildren(oldValue.Id).OrderByDescending(x => x.DateCreated).ToList();
        if (children == null || children.Count == 0) return;
        if (!AreObjectsDifferent(oldValue, newItem)) throw new Exception("No changes");

    }

    #region Helpers

    protected virtual async Task<T> GetAsync(Guid? itemId, bool withParents = false)
    {
        if (itemId == Guid.Empty) return new T();
        T? item = null;
        if (_fullInclude != null)
        {
            item = await _fullInclude.FirstOrDefaultAsync(x => x.Id == itemId);
        }
        else
            item = await _contextModel.FirstOrDefaultAsync(x => x.Id == itemId);
        if (item == null) return new T();
        item.Children = GetAllParents((Guid)itemId).Where(x => x.Id != itemId).ToList();
        item.Parents = GetAllChildren((Guid)itemId).Where(x => x.Id != itemId).ToList();

        if (withParents && _fullInclude != null)
        {
            if (item.Parents != null)
            {
                List<T> parentResult = new List<T>();

                foreach (var par in item.Parents)
                {
                    parentResult.Add(await _fullInclude.FirstOrDefaultAsync(x => x.Id == (Guid)par.Id));
                }
                item.Parents = parentResult;
            }
        }


        return item;
    }
    protected bool AreObjectsDifferent(T oldValue, T newItem)
    {
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            var oldValuePropValue = property.GetValue(oldValue);
            var newItemPropValue = property.GetValue(newItem);

            if (oldValuePropValue == null && newItemPropValue == null)
            {
                continue; // Both values are null, move to the next property
            }

            if ((oldValuePropValue == null && newItemPropValue != null) || (oldValuePropValue != null && newItemPropValue == null) || !oldValuePropValue.Equals(newItemPropValue))
            {
                return true;
            }
        }

        return false;
    }
    public IEnumerable<T> GetAllParents(Guid childId)
    {
        var child = _contextModel.FirstOrDefault(i => i.Id == childId);

        if (child != null)
        {
            yield return child;
            if (child.ParentId != null)
            {
                foreach (var parent in GetParentsRecursive((Guid)child.ParentId))
                {
                    yield return parent;
                }
            }
        }
    }

    private IEnumerable<T> GetParentsRecursive(Guid? childParentId)
    {
        var parent = _contextModel.FirstOrDefault(i => i.Id == childParentId);

        if (parent != null)
        {
            yield return parent;
            if (parent.ParentId != null)
            {
                foreach (var grandParent in GetParentsRecursive((Guid)parent.ParentId))
                {
                    yield return grandParent;
                }
            }
        }
    }


    public IEnumerable<T> GetAllChildren(Guid parentId)
    {
        var parent = _contextModel.FirstOrDefault(i => i.Id == parentId);

        if (parent != null)
        {
            yield return parent;

            foreach (var child in GetChildrenRecursive(parent.Id))
            {
                yield return child;
            }
        }
    }
    private IEnumerable<T> GetChildrenRecursive(Guid parentId)
    {
        var children = _contextModel.Where(i => i.ParentId == parentId).ToList();

        foreach (var child in children)
        {
            yield return child;
            foreach (var grandChild in GetChildrenRecursive(child.Id))
            {
                yield return grandChild;
            }
        }
    }

    #endregion

}

