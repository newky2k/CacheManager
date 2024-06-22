using DSoft.CacheManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServicesControllerExtensions
    {
        /// <summary>
        /// Registers the cache manager and sets the backend provider
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterCacheManager<T>(this IServiceCollection services) 
            where T : class, ICacheStorageBackend
        {    
            services.TryAddSingleton<ICacheManager, CacheManager>();
            services.TryAddSingleton<ICacheStorageBackend, T>();

            return services;
        }
    }
}
