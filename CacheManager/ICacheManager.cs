using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DSoft.CacheManager
{
    /// <summary>
    /// Cache Manager interface
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Gets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loaded; otherwise, <c>false</c>.
        /// </value>
        bool IsLoaded { get; }

        /// <summary>
        /// Determines whether [is key registered] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if [is key registered] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyRegistered(string key);

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        List<T> GetItems<T>(string key);

        /// <summary>
        /// Sets the items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="content">The content.</param>
        /// <param name="lastUpdated">The last updated.</param>
        void SetItems<T>(string key, List<T> content, DateTime? lastUpdated = null);

        /// <summary>
        /// Gets the last updated.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        DateTime? GetLastUpdated(string key);

        /// <summary>
        /// Updates the contents last updated.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="lastUpdated">The last updated.</param>
        void UpdateContentsLastUpdated<T>(string key, DateTime? lastUpdated = null);

        /// <summary>
        /// Syncronises the asynchronous.
        /// </summary>
        /// <returns></returns>
        Task SyncroniseAsync();

        /// <summary>
        /// Synronises this instance.
        /// </summary>
        void Synronise();

        /// <summary>
        /// Loads the asynchronous.
        /// </summary>
        /// <returns></returns>
        Task LoadAsync();

        /// <summary>
        /// Resets the cache.
        /// </summary>
        void ResetCache();

    }
}
