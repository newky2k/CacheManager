using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSoft.CacheManager
{
    internal class CacheManagerItemCollection : Collection<CacheManagerItem>
    {
        public bool ContainsKey(string key)
        {
            var item = this.Items.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            return (item != null);
        }

        public void AddRange(IEnumerable<CacheManagerItem> items)
        {
            if (items == null || !items.Any())
                return;

            foreach (var item in items)
                Add(item);
        }

        public CacheManagerItem this[string key]
        {
            get
            {
                var item = this.Items.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

                if (item == null)
                    throw new Exception($"Item with key {key} not found in CacheManagerItemCollection");

                return item;
            }
        }

        public void Remove(string key)
        {
            var item = this.Items.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (item == null)
                throw new Exception($"Item with key {key} not found in CacheManagerItemCollection");

            Items.Remove(item);
        }

    }
}
