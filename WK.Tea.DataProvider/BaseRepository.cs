using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WK.Tea.DataModel;

namespace WK.Tea.DataProvider
{

    public class BaseRepository<TContext, T> : IRepository<T>
        where TContext : DbContext, new()
        where T : class
    {
        protected TContext context;

        public BaseRepository()
        {
            this.context = new TContext();
        }

        public T Update(T entity)
        {
            context.Set<T>().Attach(entity);

            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(entity, null) != null)
                {
                    if (prop.GetValue(entity, null).ToString() == "")
                        context.Entry(entity).Property(prop.Name).CurrentValue = null;
                    context.Entry(entity).Property(prop.Name).IsModified = true;
                }
            }

            //context.Entry<T>(entity).State = EntityState.Modified;
            context.SaveChanges();

            return entity;
        }

        public T Insert(T entity)
        {
            context.Set<T>().Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Delete(T entity)
        {
            context.Entry<T>(entity).State = EntityState.Deleted;
            context.SaveChanges();
        }

        public bool IsExists(Expression<Func<T, bool>> conditions = null)
        {
            if (conditions == null)
                return context.Set<T>().Any();
            else
                return context.Set<T>().Any(conditions);
        }

        public T Find(params object[] keyValues)
        {
            return context.Set<T>().Find(keyValues);
        }

        public T FindFirstOrDefault(Expression<Func<T, bool>> conditions = null)
        {
            if (conditions == null)
                return context.Set<T>().FirstOrDefault();
            else
                return context.Set<T>().FirstOrDefault(conditions);
        }

        public List<T> FindAll(Expression<Func<T, bool>> conditions = null)
        {
            if (conditions == null)
                return context.Set<T>().ToList();
            else
                return context.Set<T>().Where(conditions).ToList();
        }

        public PagedList<T> FindAllByPage<S>(Expression<Func<T, bool>> conditions, Expression<Func<T, S>> orderBy, int pageSize, int pageIndex, string orderType = "asc")
        {
            var queryList = conditions == null ? context.Set<T>() : context.Set<T>().Where(conditions) as IQueryable<T>;

            if (orderType == "desc")
            {
                queryList = queryList.OrderByDescending(orderBy);
            }
            else
            {
                queryList = queryList.OrderBy(orderBy);
            }
            return queryList.ToPagedList(pageIndex, pageSize);
        }

        public void Dispose()
        {
            this.context.Dispose();
        }
    }
}
