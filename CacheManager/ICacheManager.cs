using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheManager
{
    public interface ICacheManager
    {
        bool IsKeyRegistered(string key);

        List<T> GetItems<T>(string key);

        void SetItems<T>(string key, List<T> content, DateTime? lastUpdated = null);

        DateTime? GetLastUpdated(string key);

        void UpdateContentsLastUpdated(string key, DateTime? lastUpdated = null);

        Task SyncroniseAsync();

        void Synronise();

        Task LoadAsync();

        void ResetCache();

    }
}
