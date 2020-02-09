using System;
using System.Collections.Generic;
using System.Data;
using Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Database
{
    /// <summary>
    /// UnitOfWork.
    /// </summary>
    public class UnitOfWork : BaseUnitOfWork, IDisposable
    {
        private IDbContextTransaction _transaction;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataContext"></param>
        public UnitOfWork(IDataContext dataContext) : base(dataContext)
        {
            _transaction = null;
        }

        /// <summary>
        /// Start transaction.
        /// </summary>
        public override void BeginTransaction()
        {
            if (_transaction == null)
            {
                _transaction = ((DbContext)DataContext).Database.BeginTransaction();
            }

            IList<IRepositoryBase> listRepositoryBases = GetAllRepository();
            foreach (IRepositoryBase repositoryBase in listRepositoryBases)
            {
                repositoryBase.HasTransaction = true;
            }
        }

        /// <summary>
        /// Start transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        public override void BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_transaction == null)
            {
                _transaction = ((DbContext)DataContext).Database.BeginTransaction(isolationLevel);
            }

            IList<IRepositoryBase> listRepositoryBases = GetAllRepository();
            foreach (IRepositoryBase repositoryBase in listRepositoryBases)
            {
                repositoryBase.HasTransaction = true;
            }
        }

        /// <summary>
        /// Save change into database.
        /// </summary>
        /// <returns></returns>
        public override int Commit()
        {
            int result;
            try
            {
                //result = GetListRepository().Sum(repositoryBase => repositoryBase.Commit());
                result = DataContext.SaveChanges();
                _transaction?.Commit();
                _transaction?.Dispose();
                _transaction = null;
                IList<IRepositoryBase> listRepositoryBases = GetAllRepository();
                foreach (IRepositoryBase repositoryBase in listRepositoryBases)
                {
                    repositoryBase.HasTransaction = true;
                }
            }
            catch (System.Exception)
            {
                _transaction?.Rollback();
                _transaction?.Dispose();
                _transaction = null;
                IList<IRepositoryBase> listRepositoryBases = GetAllRepository();
                foreach (IRepositoryBase repositoryBase in listRepositoryBases)
                {
                    repositoryBase.HasTransaction = true;
                }
                throw;
            }
            return result;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }
    }
}
