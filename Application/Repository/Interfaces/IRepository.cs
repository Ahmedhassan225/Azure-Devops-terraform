using Domain.Shared;
using System.Linq.Expressions;

namespace Application.Repository.Interfaces
{
    /// <summary>
    /// Basic Repository for Generic Data stores
    /// </summary>
    /// <typeparam name="T">Domain Model</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {

        T Add(T Entity);

        IEnumerable<T> AddRange(IEnumerable<T> entities);

        T Update(T Entity);

        IEnumerable<T> Update(IEnumerable<T> entities);

        void Delete(T Entity);

        void Delete(object KeyValue);

        void Delete(Expression<Func<T, bool>> Expression);

        T FindById(object KeyValue);

        T FirstOrDefault(Expression<Func<T, bool>> Expression);
        IEnumerable<T> FindByConditionWithNoTrack(Expression<Func<T, bool>> expression);


        T LastOrDefault(Expression<Func<T, bool>> Expression);

        IQueryable<T> All();

        IEnumerable<T> Where(Expression<Func<T, bool>> Expression);

        IEnumerable<T> Where(Expression<Func<T, bool>> Predicate, Expression<Func<T, string>> Order);

        IEnumerable<T> WhereAndIncludeMulti(System.Linq.Expressions.Expression<Func<T,bool>> expression, string[] include);

        int SaveChanges();
        Task<int> SaveChangesAsync();

        IEnumerable<T> Select(System.Linq.Expressions.Expression<Func<T, int, T>> selector);

        IEnumerable<T> SelectMany(System.Linq.Expressions.Expression<Func<T, IEnumerable<T>>> selector);

        int Count();

        int Count(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        int Count(Func<T, bool> predicate);

        long LongCount();

        long LongCount(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        long LongCount(Func<T, bool> predicate);
    }
}
