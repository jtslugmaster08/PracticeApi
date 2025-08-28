using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PracticeModel.Interface.Repositories
{
    public interface IGenericRepository<T> where T:class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void Update(T entity);
        void UpdateRante(T entity);
        T Find(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        ICollection<T> FindAll(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);
        IQueryable<T> GetAllProperties();
        Task<IQueryable<T>> GetAllAsync();

        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> FindByIncluding(Expression<Func<T,bool>> predicate,params Expression<Func<T, object>>[] includeProperties);
        int Count();
        Task<int> CountAsync();
    }
}
