using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace DSoft.CacheManager
{
    public interface ICacheStorageBackend
    {
        bool Exists { get; }

        /// <summary>
        /// Does the cache exist in the storage backend
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        bool CacheEntryExists(string keyName);

        /// <summary>
        /// Gets the items from the cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        IList<T> GetItems<T>(string keyName);

        /// <summary>
        /// Finds an entry in the selected cache using the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        T Find<T>(string keyName, Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Updates the specified cache item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="data">The data.</param>
        void Update<T>(string keyName, T data);

        /// <summary>
        /// Inserts the an item into the specified cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="data">The data.</param>
        void Insert<T>(string keyName, T data);

        /// <summary>
        /// Ensures that an index is applied using the KeySelector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="keySelector">The key selector.</param>
        void EnsureIndexed<T, K>(string keyName, Expression<Func<T, K>> keySelector);

        /// <summary>
        /// Sets the items of the cache wih for the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="data">The data.</param>
        void SetItems<T>(string keyName, List<T> data);


        /// <summary>
        /// Prepares the backend location
        /// </summary>
        void Prepare();

        /// <summary>
        /// Resets this backend database.
        /// </summary>
        void Reset();
    }
}
