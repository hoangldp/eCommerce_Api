using System;
using System.Collections.Generic;
using Core.Finder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Engine
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the various services composing the engine.
    /// Edit functionality, modules and implementations access most functionality through this interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Initialize engine
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="coreFileProvider">File provider</param>
        /// <param name="environmentName">Name of environment</param>
        void Initialize(IServiceCollection services, ICoreFileProvider coreFileProvider, string environmentName);

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        object Resolve(Type type);

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        object ResolveUnregistered(Type type);
    }
}
