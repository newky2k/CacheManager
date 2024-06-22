using LiteDB;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace DSoft.CacheManager.LiteDB
{
    /// <summary>
    /// LiteDB backend
    /// </summary>
    /// <seealso cref="DSoft.CacheManager.ICacheStorageBackend" />
    public class LiteDbBackend : ICacheStorageBackend
    {
        #region Fields

        
        private LiteDatabase _liteDatabase = null;
        private LiteDbStorageOptions _config = null;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the storage path.
        /// </summary>
        /// <value>
        /// The storage path.
        /// </value>
        public string StoragePath => Path.Combine(_config.Location, _config.FileName);

        private ConnectionString DatabaseConnectionString => new ConnectionString($"Filename={StoragePath};Password={_config.Password}");

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public LiteDatabase Database
        {
            get
            {
                if (_liteDatabase == null)
                    _liteDatabase = new LiteDatabase(DatabaseConnectionString);

                return _liteDatabase;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="LiteDbBackend"/> is exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if exists; otherwise, <c>false</c>.
        /// </value>
        public bool Exists => File.Exists(StoragePath);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteDbBackend"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LiteDbBackend(LiteDbStorageOptions config)
        {
            _config = config;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LiteDbBackend"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public LiteDbBackend(IOptions<LiteDbStorageOptions> config)
        {
            _config = config.Value;
        }

        #endregion

        /// <summary>
        /// Does the cache exist in the storage backend
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public bool CacheEntryExists(string keyName) => Database.CollectionExists(keyName);

        /// <summary>
        /// Gets the items from the cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <returns></returns>
        public IList<T> GetItems<T>(string keyName)
        {
            if (!CacheEntryExists(keyName))
                return new List<T>();

            var aCol = Database.GetCollection<T>(keyName);

            if (aCol.Count() == 0)
                return new List<T>();

            return aCol.FindAll().ToList();
        }

        /// <summary>
        /// Finds an entry in the selected cache using the predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        public T Find<T>(string keyName, Expression<Func<T, bool>> predicate)
        {
            if (!CacheEntryExists(keyName))
                return default(T);

            var col = Database.GetCollection<T>(keyName);

            return col.FindOne(predicate);

        }

        /// <summary>
        /// Updates the specified cache item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="data">The data.</param>
        public void Update<T>(string keyName, T data)
        {
            if (!CacheEntryExists(keyName))
                return;

            var col = Database.GetCollection<T>(keyName);

            col.Update(data);
        }

        /// <summary>
        /// Inserts the an item into the specified cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="data">The data.</param>
        public void Insert<T>(string keyName, T data)
        {
            var col = Database.GetCollection<T>(keyName);

            col.Insert(data);

        }

        /// <summary>
        /// Ensures that an index is applied using the KeySelector
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="K"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="keySelector">The key selector.</param>
        public void EnsureIndexed<T, K>(string keyName, Expression<Func<T, K>> keySelector)
        {
            if (!CacheEntryExists(keyName))
                return;

            var col = Database.GetCollection<T>(keyName);

            col.EnsureIndex(keySelector);
        }

        /// <summary>
        /// Sets the items of the cache wih for the specified key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="data">The data.</param>
        public void SetItems<T>(string keyName, List<T> data)
        {
            if (CacheEntryExists(keyName))
                Database.DropCollection(keyName);

            var ccol = Database.GetCollection<T>(keyName);

            ccol.InsertBulk(data);
        }

        /// <summary>
        /// Prepares the backend location
        /// </summary>
        public void Prepare()
        {
            if (!Directory.Exists(_config.Location))
                Directory.CreateDirectory(_config.Location);
        }

        /// <summary>
        /// Resets this backend database.
        /// </summary>
        public void Reset()
        {
            try
            {
                if (File.Exists(StoragePath))
                    File.Delete(StoragePath);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
