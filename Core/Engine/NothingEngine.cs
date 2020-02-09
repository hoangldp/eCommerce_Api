using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Core.Finder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Engine
{
    /// <summary>
    /// Represents engine
    /// </summary>
    public class NothingEngine : IEngine
    {
        /// <summary>
        /// Gets or sets the default file provider
        /// </summary>
        public static ICoreFileProvider DefaultFileProvider { get; set; }

        /// <summary>
        /// Service provider
        /// </summary>
        public virtual IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <typeparam name="T">Type of resolved service</typeparam>
        /// <returns>Resolved service</returns>
        public T Resolve<T>() where T : class
        {
            return (T)GetServiceProvider().GetRequiredService(typeof(T));
        }

        /// <summary>
        /// Resolve dependency
        /// </summary>
        /// <param name="type">Type of resolved service</param>
        /// <returns>Resolved service</returns>
        public object Resolve(Type type)
        {
            return GetServiceProvider().GetRequiredService(type);
        }

        /// <summary>
        /// Resolve dependencies
        /// </summary>
        /// <typeparam name="T">Type of resolved services</typeparam>
        /// <returns>Collection of resolved services</returns>
        public IEnumerable<T> ResolveAll<T>()
        {
            return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        /// <summary>
        /// Get IServiceProvider
        /// </summary>
        /// <returns>IServiceProvider</returns>
        protected IServiceProvider GetServiceProvider()
        {
            return ServiceProvider;
        }

        /// <summary>
        /// Initialize engine
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="coreFileProvider">File provider</param>
        /// <param name="environmentName">Name of environment</param>
        public void Initialize(IServiceCollection services, ICoreFileProvider coreFileProvider, string environmentName)
        {
            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            DefaultFileProvider = coreFileProvider;
            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Resolve unregistered service
        /// </summary>
        /// <param name="type">Type of service</param>
        /// <returns>Resolved service</returns>
        public virtual object ResolveUnregistered(Type type)
        {
            Exception innerException = null;
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                            throw new Exception("Unknown dependency");
                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (Exception ex)
                {
                    innerException = ex;
                }
            }
            throw new Exception("No constructor was found that had all the dependencies satisfied.", innerException);
        }
    }
}
