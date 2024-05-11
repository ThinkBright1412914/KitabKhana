using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KitabKhana.Data.Repository.IRepository
{ 
    public interface iRepository<T> where T : class
    {
        T GetById(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked = true);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string? includeProperties = null);

        void Add(T entity); 

        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entity);


    }
}
