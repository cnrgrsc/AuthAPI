using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity: class 
    {
        Task<TEntity> GetByIdAsync(int id); //Listeleme
        Task<IEnumerable<TEntity>> GetAllAsync(); //Tüm datalar 
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate); 
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        TEntity Update(TEntity entity);

    }
}
