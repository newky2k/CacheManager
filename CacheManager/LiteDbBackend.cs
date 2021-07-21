using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace DSoft.CacheManager.LiteDB
{
    public class LiteDbBackend : IStorageBackend
    {
        private LiteDatabase _liteDatabase = null;

        public string Location { get; set; }

        public string FileName { get; set; }

        public string Password { get; set; }

        private string StoragePath => Path.Combine(Location, FileName);

        private ConnectionString DatabaseConnectionString => new ConnectionString($"Filename={StoragePath};Password={Password}");

        public LiteDatabase Database
        {
            get
            {
                if (_liteDatabase == null)
                    _liteDatabase = new LiteDatabase(DatabaseConnectionString);

                return _liteDatabase;
            }
        }

        public bool CacheEntryExists(string keyName) => Database.CollectionExists(keyName);

        public IList<T> GetItems<T>(string keyName)
        {
            if (!CacheEntryExists(keyName))
                return new List<T>();

            var aCol = Database.GetCollection<T>(keyName);

            if (aCol.Count() == 0)
                return new List<T>();

            return aCol.FindAll().ToList();
        }

        public T Find<T>(string keyName, Expression<Func<T, bool>> predicate)
        {
            if (!CacheEntryExists(keyName))
                return default(T);

            var col = Database.GetCollection<T>(keyName);

            return col.FindOne(predicate);

        }

        public void Update<T>(string keyName, T data)
        {
            if (!CacheEntryExists(keyName))
                return;

            var col = Database.GetCollection<T>(keyName);

            col.Update(data);
        }

        public void Insert<T>(string keyName, T data)
        {
            if (!CacheEntryExists(keyName))
                return;

            var col = Database.GetCollection<T>(keyName);

            col.Insert(data);

        }

        public void EnsureIndexed<T, K>(string keyName, Expression<Func<T, K>> keySelector)
        {
            if (!CacheEntryExists(keyName))
                return;

            var col = Database.GetCollection<T>(keyName);

            col.EnsureIndex(keySelector);
        }

        public void SetItems<T>(string keyName, List<T> data)
        {
            if (!CacheEntryExists(keyName))
                Database.DropCollection(keyName);

            var ccol = Database.GetCollection<T>(keyName);

            ccol.InsertBulk(data);
        }
    }
}
