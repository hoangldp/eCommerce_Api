using System;
using Core.Database;
using Core.Engine;
using Core.Finder;
using Microsoft.EntityFrameworkCore;

namespace Database
{
    public abstract class DatabaseContext : DbContext, IDataContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var listTypeBuilder = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IModelBuilder>();
            foreach (Type typeBuilder in listTypeBuilder)
            {
                IModelBuilder builder = (IModelBuilder)Activator.CreateInstance(typeBuilder, modelBuilder);
                builder.Build();
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
