using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Buran.Core.MvcLibrary.Repository
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        IQueryable<T> GetList(Expression<Func<T, bool>> filter = null);

        Task<T> GetItemAsync(int id);

        bool ValidateItem(T item);
        bool ValidateDeleteItem(T item);

        Task<bool> CreateAsync(T item);
        Task<bool> EditAsync(T item);
     
        Task<bool> DeleteAsync(object id);
        Task<bool> DeleteAsync(T item);
    }
}