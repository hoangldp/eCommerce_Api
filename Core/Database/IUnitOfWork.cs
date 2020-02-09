using System.Collections.Generic;
using System.Data;
using Model;

namespace Core.Database
{
    /// <summary>
    /// IUnitOfWork.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Start transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Start transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        void BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Save change into database.
        /// </summary>
        /// <returns></returns>
        int Commit();

        /// <summary>
        /// Get <see cref="IDataContext"/>.
        /// </summary>
        /// <returns></returns>
        IDataContext GetContext();

        /// <summary>
        /// Get repository of <see cref="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IRepository<TEntity> GetRepositoryFromEntity<TEntity>() where TEntity : EntityBase, new();

        /// <summary>
        /// Get repository <see cref="TRepository"/>.
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        TRepository GetRepository<TRepository>() where TRepository : class, IRepositoryBase;

        /// <summary>
        /// Register repository.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        IRepository<TEntity> Register<TEntity>(IRepository<TEntity> repository) where TEntity : EntityBase, new();

        /// <summary>
        /// Register repository
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        TRepository Register<TRepository>(TRepository repository) where TRepository : class, IRepositoryBase;

        /// <summary>
        /// Get list repository.
        /// </summary>
        /// <returns>List repository.</returns>
        IList<IRepositoryBase> GetAllRepository();
    }
}
