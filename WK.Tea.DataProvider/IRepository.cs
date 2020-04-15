using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel;

namespace WK.Tea.DataProvider
{
    public interface IRepository<T> : IDisposable
        where T : class
    {
        T Update(T entity);
        T Insert(T entity);
        void Delete(T entity);
        bool IsExists(Expression<Func<T, bool>> conditions = null);
        T Find(params object[] keyValues);
        T FindFirstOrDefault(Expression<Func<T, bool>> conditions = null);
        List<T> FindAll(Expression<Func<T, bool>> conditions = null);
        PagedList<T> FindAllByPage<S>(Expression<Func<T, bool>> conditions, Expression<Func<T, S>> orderBy, int pageSize, int pageIndex, string orderType = "asc");
    }
}
