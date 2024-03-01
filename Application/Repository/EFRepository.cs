using Application.Repository.Interfaces;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
namespace Application.Repository
{
    /// <summary>
    /// Full Featured Repository implementation 
    /// </summary>
    /// <typeparam name="T">Domain Model</typeparam>
    public partial class EFRepository<T> : IRepository<T>, IRepositoryExtension<T>, IDisposable where T : BaseEntity
    {
        #region Data Members
        /// <summary>
        /// Entity Framework Database context Object
        /// </summary>
        private readonly DbContext _dbContext;
        /// <summary>
        /// indicator if Object disposed or not
        /// </summary>
        private readonly bool _disposed = false;
        private string currentUser = "unknown";

        #endregion

        #region CTOR
        public EFRepository(DbContext dbContext)
        {
            /// if Database context not initalized Through Exception
            this._dbContext = dbContext ?? throw new ArgumentNullException("dbContext is null");

            // We can use Lazy load for small amount of data
            //_DbContext.Configuration.LazyLoadingEnabled = false;


        }
        #endregion

        #region CRUD Operations
        public T Add(T Entity)
        {
            if (Entity == null) throw new ArgumentNullException("Entity is null");
            try
            {
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(Entity, new ValidationContext(Entity), validationResults))
                {
                    foreach (var validationError in validationResults)
                    {
                        Trace.TraceInformation("Properties: {0} Error: {1}", validationError.MemberNames.ToString(), validationError.ErrorMessage);
                    }
                }
                else
                {
                    Entity.Created = DateTime.UtcNow;
                    Entity.CreatedBy = currentUser;
                    _dbContext.Add<T>(Entity);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> Entities)
        {
            if (Entities == null) throw new ArgumentNullException("Entity is null");
            try
            {
                var validationResults = new List<ValidationResult>();
                foreach (var Entity in Entities)
                {
                    if (!Validator.TryValidateObject(Entity, new ValidationContext(Entity), validationResults))
                    {
                        foreach (var validationError in validationResults)
                        {
                            Trace.TraceInformation("Properties: {0} Error: {1}", validationError.MemberNames.ToString(), validationError.ErrorMessage);
                        }
                        validationResults = new List<ValidationResult>();
                    }
                    else
                    {
                        Entity.Created = DateTime.UtcNow;
                        Entity.CreatedBy = currentUser;
                        _dbContext.Set<T>().Add(Entity);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public T Update(T Entity)
        {
            if (Entity == null) throw new ArgumentNullException("Entity is null");
            try
            {

                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(Entity, new ValidationContext(Entity), validationResults))
                {
                    foreach (var validationError in validationResults)
                    {
                        Trace.TraceInformation("Properties: {0} Error: {1}", validationError.MemberNames.ToString(), validationError.ErrorMessage);
                    }
                }
                else
                {
                    Entity.Modified = DateTime.UtcNow;
                    Entity.ModifiedBy = currentUser;
                    _dbContext.Entry<T>(Entity).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public IEnumerable<T> Update(IEnumerable<T> Entities)
        {
            if (Entities == null) throw new ArgumentNullException("Entity is null");
            try
            {
                var validationResults = new List<ValidationResult>();
                foreach (var entity in Entities)
                {
                    if (!Validator.TryValidateObject(entity, new ValidationContext(entity), validationResults))
                    {
                        foreach (var validationError in validationResults)
                        {
                            Trace.TraceInformation("Properties: {0} Error: {1}", validationError.MemberNames.ToString(), validationError.ErrorMessage);
                        }
                        validationResults = new List<ValidationResult>();
                    }
                    else
                    {
                        entity.Modified = DateTime.UtcNow;
                        entity.ModifiedBy = currentUser;
                        _dbContext.Entry<T>(entity).State = EntityState.Modified;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;
        }

        public void Delete(T Entity)
        {
            if (Entity == null) throw new ArgumentNullException("Entity is null");
            try
            {
                var validationResults = new List<ValidationResult>();
                if (!Validator.TryValidateObject(Entity, new ValidationContext(Entity), validationResults))
                {
                    foreach (var validationError in validationResults)
                    {
                        Trace.TraceInformation("Properties: {0} Error: {1}", validationError.MemberNames.ToString(), validationError.ErrorMessage);
                    }
                }
                else
                {
                    Entity.Deleted = true;
                    Entity.ModifiedBy = currentUser;
                    Entity.Modified = DateTime.Now;
                    _dbContext.Entry<T>(Entity).State = EntityState.Modified;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(IEnumerable<T> Entities)
        {
            if (Entities == null) throw new ArgumentNullException("Entity is null");
            try
            {
                var validationResults = new List<ValidationResult>();
                foreach (var entity in Entities)
                {
                    if (!Validator.TryValidateObject(entity, new ValidationContext(entity), validationResults))
                    {
                        foreach (var validationError in validationResults)
                        {
                            Trace.TraceInformation("Properties: {0} Error: {1}", validationError.MemberNames.ToString(), validationError.ErrorMessage);
                        }
                        validationResults = new List<ValidationResult>();
                    }
                    else
                    {
                        entity.Deleted = true;
                        entity.ModifiedBy = currentUser;
                        entity.Modified = DateTime.Now;
                        _dbContext.Entry<T>(entity).State = EntityState.Modified;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(object KeyValue)
        {
            if (KeyValue == null) throw new ArgumentNullException("Entity is null");
            try
            {
                var Entity = _dbContext.Entry(_dbContext.Set<T>().Find(KeyValue));
                var validationResults = new List<ValidationResult>();
                if (Entity == null || !Validator.TryValidateObject(Entity, new ValidationContext(Entity), validationResults))
                {
                    foreach (var validationError in validationResults)
                    {
                        Trace.TraceInformation("Properties: {0} Error: {1}", validationError.MemberNames.ToString(), validationError.ErrorMessage);
                    }
                }
                else
                {
                    Entity.Entity.Deleted = true;
                    Entity.Entity.ModifiedBy = currentUser;
                    Entity.Entity.Modified = DateTime.Now;
                    _dbContext.Entry<T>(Entity.Entity).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> Expression)
        {
            try
            {
                var fetchedData = _dbContext.Set<T>().AsNoTracking().Where(Expression).ToList();
                foreach (var entity in fetchedData)
                {
                    entity.Deleted = true;
                    entity.ModifiedBy = currentUser;
                    entity.Modified = DateTime.Now;
                    _dbContext.Entry<T>(entity).State = EntityState.Modified;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Data retrival Operations 
        public T FindById(object KeyValue)
        {
            if (KeyValue == null) throw new ArgumentNullException("Entity is null");
            try
            {
                return _dbContext.Set<T>().Find(KeyValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T FirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> Expression)
        {
            if (Expression == null) throw new ArgumentNullException("Expression is null");
            try
            {
                return _dbContext.Set<T>().FirstOrDefault(Expression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> FindByConditionWithNoTrack(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return this._dbContext.Set<T>().Where(expression).AsNoTracking();
        }
        public T LastOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> Expression)
        {
            if (Expression == null) throw new ArgumentNullException("Expression is null");
            try
            {
                return _dbContext.Set<T>().LastOrDefault(Expression);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<T> AsNoTracking()
        {
            return _dbContext.Set<T>().AsNoTracking<T>();
        }

        public IEnumerable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> Expression)
        {
            return _dbContext.Set<T>().Where(Expression);
        }

        public IEnumerable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> Predicate, System.Linq.Expressions.Expression<Func<T, string>> Order)
        {
            return _dbContext.Set<T>().Where(Predicate).OrderBy(Order);
        }

        public IEnumerable<T> WhereAndIncludeMulti(System.Linq.Expressions.Expression<Func<T, bool>> expression, string[] include)
        {
            IQueryable<T> query = this._dbContext.Set<T>();

            foreach (string inc in include)
            {
                query = query.Include(inc);
            }

            return query.Where(expression);
        }

        public IQueryable<T> AllIncluding(params System.Linq.Expressions.Expression<Func<T, object>>[] IncludeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var includeProperty in IncludeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
        #endregion

        #region Stored Procedure Operations
        public IEnumerable<T> ExecWithStoredProcedure(string query, params object[] Parameters)
        {
            //try
            //{
            //    var resulte = _DbContext.Database.SqlQuery<T>(query, Parameters).ToList();
            //    return resulte;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
            throw new NotImplementedException();
        }

        public IEnumerable<T> ExecWithStoredProcedureNonParameters(string Query)
        {
            //try
            //{
            //    var resulte = _DbContext.Database.SqlQuery<T>(Query).ToList();
            //    return resulte;
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            throw new NotImplementedException();
        }

        public string ExecScalarWithStoredProcedure(string query, params object[] Parameters)
        {
            //try
            //{
            //    return _DbContext.Database.SqlQuery<string>(query, Parameters).FirstOrDefault();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            throw new NotImplementedException();
        }

        public void ExecWithStoredProcedureWithNoReturn(string query, params object[] Parameters)
        {
            try
            {
                _dbContext.Database.ExecuteSqlRaw(query, Parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int ExecStoredProcedureWithRowsAffected(string query, params object[] Parameters)
        {
            try
            {
                return _dbContext.Database.ExecuteSqlRaw(query, Parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Plain Query Operations
        public int ExecuteSql(string Sql)
        {
            DbConnection conn = _dbContext.Database.GetDbConnection();
            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();  // open connection if not already open
                using DbCommand cmd = conn.CreateCommand();
                cmd.CommandText = Sql;
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }
        }
        #endregion

        #region Get All
        public IQueryable<T> All()
        {
            return _dbContext.Set<T>().AsQueryable<T>();
        }

        /// <summary>
        /// Not Recommended for :arge Data Retrival
        /// </summary>
        /// <param name="AsNoTracking">Disable Tracking by Deafult</param>
        /// <returns></returns>
        public IEnumerable<T> All(bool AsNoTracking = true)
        {
            if (AsNoTracking)
                return _dbContext.Set<T>().AsNoTracking().ToList<T>();
            else
                return _dbContext.Set<T>().ToList<T>();
        }
        #endregion

        #region Save Data 
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
        #endregion

        #region Plain SQL Async Operations
        public async Task<int> ExecuteSqlAsync(string Sql)
        {
            DbConnection conn = _dbContext.Database.GetDbConnection();
            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();  // open connection if not already open
                using DbCommand cmd = conn.CreateCommand();
                cmd.CommandText = Sql;
                return await cmd.ExecuteNonQueryAsync();
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }
        }
        #endregion

        #region Queries 
        public IEnumerable<T> Select(System.Linq.Expressions.Expression<Func<T, int, T>> selector)
        {
            if (selector == null)
                throw new Exception("selector is null");

            return _dbContext.Set<T>().Select(selector).ToList();
        }


        public IEnumerable<T> SelectMany(System.Linq.Expressions.Expression<Func<T, IEnumerable<T>>> selector)
        {
            if (selector == null)
                throw new Exception("selector is null");

            return _dbContext.Set<T>().SelectMany(selector).ToList();
        }
        #endregion 

        #region Counts 
        public int Count()
        {
            return _dbContext.Set<T>().Count();
        }

        public int Count(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new Exception("expression is null");

            return _dbContext.Set<T>().Count(predicate);
        }

        public int Count(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new Exception("expression is null");

            return _dbContext.Set<T>().Count(predicate);
        }

        public long LongCount()
        {
            return _dbContext.Set<T>().LongCount();
        }

        public long LongCount(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new Exception("expression is null");

            return _dbContext.Set<T>().LongCount(predicate);
        }

        public long LongCount(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new Exception("expression is null");

            return _dbContext.Set<T>().LongCount(predicate);
        }
        #endregion

        #region Disposing 
        protected virtual void Dispose(bool disposing)
        {

            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose(); // Disposing the DB Context 
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Finalize for better claring orphans from the Memory
        }

        #endregion 
    }

}
