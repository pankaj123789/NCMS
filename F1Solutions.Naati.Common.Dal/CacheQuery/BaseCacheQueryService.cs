using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using F1Solutions.Global.Common;
using F1Solutions.Naati.Common.Contracts.Dal.QueryServices;

namespace F1Solutions.Naati.Common.Dal.CacheQuery
{
    public abstract class BaseCacheQueryService<TKeyType, TResultType, TDataType> : ICacheQueryService<TKeyType>
    { 
        public virtual void RefreshAllCache()
        {
            LoadAllItems(true);
        }

        private IDictionary<TKeyType, TResultType> _items;
        protected IDictionary<TKeyType, TResultType> Items => _items ?? (_items = LoadAllItems(false));

        public virtual TResultType GetItem(TKeyType key)
        {
            var transformedKey = TransformKey(key);
            if (transformedKey == null)
            {
                return default;
            }
            Items.TryGetValue(transformedKey, out var cachedItem);
            return cachedItem;
        }

        protected virtual TResultType SetItem(TKeyType key, TResultType value)
        {
            var transformedKey = TransformKey(key);
            if (transformedKey == null)
            {
                return default;
            }

            if (value == null)
            {
                return default;
            }

            Items[transformedKey] = value;
            return value;
        }

        protected abstract TKeyType TransformKey(TKeyType key); 
        protected abstract TDataType GetDbSingleValue(TKeyType key);
        protected abstract IReadOnlyList<KeyValuePair<TKeyType, TResultType>> GetAllDbValues();


        public virtual bool AddOrRefreshItem(TKeyType key)
        {
            if (RefreshItem(key))
            {
                return true;
            }

            var transformedKey = TransformKey(key);
            var value = GetDbSingleValue(transformedKey);
            if (value != null)
            {
                Items.Add(transformedKey, MapToTResultType(value));
                return true;
            }

            return false;
        }


        protected virtual bool RefreshItem(TKeyType key)
        {
            if (_items == null)
            {
                return false;
            }

            var transformedKey = TransformKey(key);
            if (transformedKey == null)
            {
                return false;
            }
            var value = GetDbSingleValue(transformedKey);

            if (value == null)
            {
                Items.Remove(transformedKey);
            }
            else
            {
                Items[transformedKey] = MapToTResultType(value);
            }

            return true;
        }

        protected virtual IDictionary<TKeyType, TResultType> LoadAllItems(bool force)
        {
            if (!force && _items != null)
            {
                return _items;
            }

            var items = GetAllDbValues();
            var transformedItems = items.Select(x => new KeyValuePair<TKeyType, TResultType>(TransformKey(x.Key), x.Value)).ToList();
            
            _items = new ConcurrentDictionary<TKeyType, TResultType>(transformedItems);
            return _items;
        }

        protected virtual bool HasItem(TKeyType key)
        {
            var transformedKey = TransformKey(key);
            if (transformedKey == null)
            {
                return false;
            }
        
            return Items.ContainsKey(transformedKey);
        }

        protected abstract TResultType MapToTResultType(TDataType item);
    }
}
