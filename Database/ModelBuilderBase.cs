using Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Model;

namespace Database
{
    public abstract class ModelBuilderBase<TEntity> : IModelBuilder where TEntity : EntityBase, new()
    {
        private readonly ModelBuilder _modelBuilder;

        public ModelBuilderBase(ModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public void Build()
        {
            OnModelCreating(_modelBuilder.Entity<TEntity>());
        }

        public abstract void OnModelCreating(EntityTypeBuilder<TEntity> entityTypeBuilder);
    }
}
