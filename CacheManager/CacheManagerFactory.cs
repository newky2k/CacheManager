using System;
using System.Collections.Generic;
using System.Text;

namespace DSoft.CacheManager
{
    /// <summary>
    /// Factory for Cachce Manager
    /// </summary>
    public static class CacheManagerFactory
    {
        /// <summary>
        /// Creates the cache manager with the specified backend.
        /// </summary>
        /// <param name="backend">The backend.</param>
        /// <returns></returns>
        public static ICacheManager Create(ICacheStorageBackend backend)
        {
            return new CacheManager(backend);
        }
    }
}
