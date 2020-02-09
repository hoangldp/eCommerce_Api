using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core.Engine;
using Model;

namespace Core.Database
{
    /// <summary>
    /// BaseUnitOfWork.
    /// </summary>
    public abstract class BaseUnitOfWork : IUnitOfWork
    {
        protected readonly IDataContext DataContext;
        private readonly IDictionary<string, object> _listRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        protected BaseUnitOfWork(IDataContext dataContext)
        {
            DataContext = dataContext;

            _listRepository = new Dictionary<string, object>();
        }

        /// <summary>
        /// Start transaction
        /// </summary>
        public abstract void BeginTransaction();

        /// <summary>
        /// Start transaction
        /// </summary>
        /// <param name="isolationLevel"></param>
        public abstract void BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Save change into database.
        /// </summary>
        /// <returns></returns>
        public abstract int Commit();

        /// <summary>
        /// Get <see cref="IDataContext"/>.
        /// </summary>
        /// <returns></returns>
        public virtual IDataContext GetContext()
        {
            return DataContext;
        }

        /// <summary>
        /// Get repository of <see cref="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual IRepository<TEntity> GetRepositoryFromEntity<TEntity>() where TEntity : EntityBase, new()
        {
            Type typeRepository = typeof(IRepository<TEntity>);
            if (_listRepository.ContainsKey(typeRepository.FullName ?? throw new InvalidOperationException("typeRepository is been null")))
            {
                return (IRepository<TEntity>)_listRepository[typeRepository.FullName];
            }
            IRepository<TEntity> repository = EngineContext.Current.Resolve<IRepository<TEntity>>();
            return Register(repository);
        }

        /// <summary>
        /// Get repository <see cref="TRepository"/>.
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <returns></returns>
        public virtual TRepository GetRepository<TRepository>()
            where TRepository : class, IRepositoryBase
        {
            Type typeRepository = typeof(TRepository);
            if (_listRepository.ContainsKey(typeRepository.FullName ?? throw new InvalidOperationException("typeRepository is been null")))
            {
                return (TRepository)_listRepository[typeRepository.FullName];
            }
            TRepository repository = EngineContext.Current.Resolve<TRepository>();
            return Register(repository);
        }

        /// <summary>
        /// Register repository
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public virtual IRepository<TEntity> Register<TEntity>(IRepository<TEntity> repository) where TEntity : EntityBase, new()
        {
            bool useTransaction = false;
            if (_listRepository.Count > 0)
            {
                var repositoryFirst = (IRepositoryBase)_listRepository.First().Value;
                useTransaction = repositoryFirst.HasTransaction;
            }

            repository.HasTransaction = useTransaction;
            repository.SetContext(GetContext());

            Type typeRepository = typeof(IRepository<TEntity>);
            if (!_listRepository.ContainsKey(typeRepository.FullName ?? throw new InvalidOperationException("typeRepository is been null")))
            {
                _listRepository.Add(typeRepository.FullName, repository);
            }
            else
            {
                _listRepository[typeRepository.FullName] = repository;
            }
            return repository;
        }

        /// <summary>
        /// Register repository
        /// </summary>
        /// <typeparam name="TRepository"></typeparam>
        /// <param name="repository"></param>
        /// <returns></returns>
        public virtual TRepository Register<TRepository>(TRepository repository) where TRepository : class, IRepositoryBase
        {
            bool useTransaction = false;
            if (_listRepository.Count > 0)
            {
                var repositoryFirst = (IRepositoryBase)_listRepository.First().Value;
                useTransaction = repositoryFirst.HasTransaction;
            }

            repository.HasTransaction = useTransaction;
            repository.SetContext(GetContext());

            Type typeRepository = typeof(TRepository);
            if (!_listRepository.ContainsKey(typeRepository.FullName ?? throw new InvalidOperationException("typeRepository is been null")))
            {
                _listRepository.Add(typeRepository.FullName, repository);
            }
            else
            {
                _listRepository[typeRepository.FullName] = repository;
            }
            return repository;
        }

        /// <summary>
        /// Get list repository.
        /// </summary>
        /// <returns>List repository.</returns>
        public virtual IList<IRepositoryBase> GetAllRepository()
        {
            return _listRepository.Values.Cast<IRepositoryBase>().ToList();
        }
    }
}
