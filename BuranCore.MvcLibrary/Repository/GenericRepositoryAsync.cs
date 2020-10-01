using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.Repository
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        protected readonly IValidationDictionary ValidationDictionary;
        protected DbContext Context;
        protected DbSet<T> DbSet;

        public GenericRepositoryAsync(IValidationDictionary validationDictionary, DbContext context)
        {
            ValidationDictionary = validationDictionary;
            Context = context;
            if (context != null)
            {
                DbSet = context.Set<T>();
            }
        }

        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = DbSet.AsNoTracking();
            if (filter != null)
                query = query.Where(filter);
            return query;
        }
        public virtual T GetItem(int id)
        {
            return DbSet.Find(id);
        }
        public virtual async Task<T> GetItemAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        protected void RaiseError(string error, string field)
        {
            if (ValidationDictionary != null)
                ValidationDictionary.AddError(field, error);
            else
                throw new Exception(error);
        }
        public virtual bool ValidateItem(T item)
        {
            return ValidationDictionary == null || ValidationDictionary.IsValid;
        }
        public virtual bool ValidateDeleteItem(T item)
        {
            return ValidationDictionary == null || ValidationDictionary.IsValid;
        }

        public virtual async Task<bool> CreateAsync(T item)
        {
            if (!ValidateItem(item))
                return false;
            DbSet.Add(item);
            await Context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> EditAsync(T item)
        {
            if (!ValidateItem(item))
                return false;
            Context.Entry(item).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            T entityToDelete = await DbSet.FindAsync(id);
            return await DeleteAsync(entityToDelete);
        }
        public virtual async Task<bool> DeleteAsync(T item)
        {
            if (!ValidateDeleteItem(item))
                return false;
            if (Context.Entry(item).State == EntityState.Detached)
                DbSet.Attach(item);
            DbSet.Remove(item);
            await Context.SaveChangesAsync();
            return true;
        }
    }
}